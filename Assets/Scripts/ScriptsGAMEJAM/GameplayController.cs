using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

namespace GJ18
{
    using AggregatedUnitsInfo = Dictionary<BoardObjectType, int>;
    
    public class GameplayController : MonoBehaviour
    {
        [SerializeField] private bool __debugCopyDarkTroops = false;
        [SerializeField] private LevelConfig __debugLevelConfig;
        public static LevelConfig PlayLevelConfig = null;
        
        [SerializeField] private BoardInput _boardInput;
        [SerializeField] private RectTransform _boardContainer;
        [SerializeField] private GameObject _tilePrefab;

        private BoardTile[,] _tiles;
        
        private LevelConfig _levelConfig;
        [SerializeField] private CoreConfig _coreConfig;
        [SerializeField] private int _deployUnitFactor = 1;
        
        [SerializeField] private CombatUnit[] PlayerCombatUnits;
        [SerializeField] private CombatUnit[] EnemyCombatUnits;

        [SerializeField] private Image _powerBarForegroud;
        [SerializeField] private MovesCounter _movesCounter;
        [SerializeField] private SoftPopup _softPopup;
        [SerializeField] private PopupManager _popupManager;

        private int _remainingMoves = 1;

        private BattleController _battleController;
        
        public bool ExtendedMove;

        BoardConfig BoardConfig()
        {
            return _levelConfig.BoardConfig;
        }

        private void Awake()
        {
            Core.Init();

            _levelConfig = PlayLevelConfig != null ? PlayLevelConfig : __debugLevelConfig;

            var baseSize7 = _boardContainer.rect.width;
            var newSize = baseSize7 / 7.0f * BoardConfig().SizeX;
            Utils.SetSize(_boardContainer, newSize, newSize);

            var boardConfig = BoardConfig();
            
            _boardInput.Init(boardConfig);
            _boardInput.OnSwipedCallback = OnInputDrag;
            _boardInput.OnClickCallback = OnInputClick;
            
            _tiles = new BoardTile[boardConfig.SizeY, boardConfig.SizeX];

            CleanCombatUnits(PlayerCombatUnits);
            CleanCombatUnits(EnemyCombatUnits);

            _battleController = GetComponent<BattleController>();
            _battleController.OnPowerBalanceChanged = UpdatePowerBar;
            _battleController.OnBattleEndedCallback = OnBattleEnded;
            _battleController.OnNewTurnCallback = OnNewTurn;

            _remainingMoves = _levelConfig.MovesBeforeBattle;
            UpdateMoves();
        }

        private void OnNewTurn(int turn)
        {
            _movesCounter.SetTurn(turn);
        }

        private void UpdateMoves()
        {
            _movesCounter.SetMoves(_remainingMoves);
        }

        private void CleanCombatUnits(CombatUnit[] units)
        {
            foreach (var u in units)
            {
                u.SetUnitConfig(null, false, false);
                u.UnitsAmount = 0;
            }
        }

        private CombatUnit FindEmptyUnit(CombatUnit[] combatUnits)
        {
            foreach (var cu in combatUnits)
            {
                if (cu.IsEmpty())
                {
                    return cu;
                }
            }

            return null;
        }
        
        private CombatUnit FindUnitByType(BoardObjectType type, CombatUnit[] combatUnits)
        {
            foreach (var cu in combatUnits)
            {
                if (!cu.IsEmpty() && cu.GetConfig().type == type)
                {
                    return cu;
                }
            }

            return null;
        }
        
        private void DeployCombatUnits(AggregatedUnitsInfo info, CombatUnit[] combatUnits, bool asEnemies)
        {
            foreach (var typeAmountKey in info)
            {
                var type = typeAmountKey.Key;
                var amount = typeAmountKey.Value;
                DeployCombatUnit(type, amount, combatUnits, asEnemies);
            }
        }

        private void DeployCombatUnit(BoardObjectType type, int amount, CombatUnit[] combatUnits, bool asEnemy)
        {
            var combatUnit = FindUnitByType(type, combatUnits);
            if (combatUnit == null)
            {
                combatUnit = FindEmptyUnit(combatUnits);
                combatUnit.SetUnitConfig(_coreConfig.FindUnitConfig(type), asEnemy, false);
                combatUnit.UnitsAmount = 0;
            }
                
            combatUnit.UnitsAmount += amount;
        }

        private AggregatedUnitsInfo GetAggregatedEmenyUnits()
        {
            var aggregatedUnits = new AggregatedUnitsInfo();
            
            foreach (var enemyUnits in _levelConfig.EnemyUnits)
            {
                aggregatedUnits.Add(enemyUnits.unit.type, enemyUnits.amount);
            }

            return aggregatedUnits;
        }

        private AggregatedUnitsInfo GetAggregateBoardUnits(bool clear, int unitsFactor)
        {
            var aggregatedUnits = new AggregatedUnitsInfo();
            
            ForEachTile((tile, i, j) =>
            {
                var tileConfig = tile.GetConfig();
                if (tile.IsEmpty() || tileConfig.category != Category.Troop)
                {
                    return;
                }

                if (!aggregatedUnits.ContainsKey(tileConfig.type))
                {
                    aggregatedUnits.Add(tileConfig.type, 0);
                }

                aggregatedUnits[tileConfig.type] += unitsFactor;

                if (clear)
                {
                    tile.SetUnitConfig(null);    
                }
            });

            return aggregatedUnits;
        }

        private void OnInputClick(BoardInput.Coord coord)
        {
            var tile = GetTile(coord.i, coord.j);
            if (tile.IsEmpty() || tile.GetConfig().category != Category.Troop)
            {
                return;
            }
            
            DeployCombatUnit(tile.GetConfig().type, _deployUnitFactor, PlayerCombatUnits, false);
            tile.SetUnitConfig(null);
            UpdatePowerBar();
        }
         
        private void OnInputDrag(int icoord, int jcoord, int dragIDir, int dragJDir)
        {
            if (!MovesAllowed())
            {
                return;
            }
            
            var movedTile = GetTile(icoord, jcoord);
            if (movedTile.IsEmpty() || movedTile.GetConfig().category == Category.Obstacle)
            {
                return;
            }
            
            var targetTile = GetMoveTile(icoord, jcoord, dragIDir, dragJDir, ExtendedMove, movedTile.GetObjType());
            if (targetTile == null)
            {
                return;
            }

            BoardObjectType? newType = null;
            if (targetTile.IsEmpty())
            {
                if (!ExtendedMove)
                {
                    return;    
                }
            }
            else
            {
                var type1 = movedTile.GetObjType();
                var type2 = targetTile.GetObjType();
                newType = Core.GetCraftObjectType(type1, type2);    
            }

            var movedTileInitialPos = movedTile.transform.position; 
            movedTile.transform.DOMove(targetTile.transform.position, 0.2f).SetEase(Ease.InQuint).OnComplete(() =>
            {
                if (newType != null)
                {
                    var newUnitConfig = _coreConfig.FindUnitConfig(newType.GetValueOrDefault());
                    targetTile.SetUnitConfig(newUnitConfig);

                    if (newUnitConfig.category == Category.Troop)
                    {
                        _softPopup.printSoftPopup(newUnitConfig.unitLongName + " created!");    
                    }
                    
                    targetTile.PlayMergeEffect();
                }
                else
                {
                    targetTile.SetUnitConfig(movedTile.GetConfig());
                }
                
                movedTile.SetUnitConfig(null);
                movedTile.transform.position = movedTileInitialPos;

                OnMoveCompleted();
            });
        }
        
        private void Start()
        {
            var boardConfig = BoardConfig();
            
            GenerateBoard(boardConfig);
            
            PopuplateBoardObstacles(boardConfig);
            PopulateBoard(boardConfig);
            
            DeployCombatUnits(GetAggregatedEmenyUnits(), EnemyCombatUnits, true);
            
            if (__debugCopyDarkTroops)
            {
                DeployCopyEnemyUnits();
            }

            _powerbarTotalWidth = _powerBarForegroud.GetComponent<RectTransform>().rect.width;
            UpdatePowerBar();
            
            _softPopup.printSoftPopup(_levelConfig.name);
        }

        private void DeployCopyEnemyUnits()
        {
            DeployCombatUnits(GetAggregatedEmenyUnits(), PlayerCombatUnits, false);
        }

        private void DeployBoardUnits()
        {
            DeployCombatUnits(GetAggregateBoardUnits(true, _deployUnitFactor), PlayerCombatUnits, false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                DeployCombatUnits(GetAggregateBoardUnits(true, _deployUnitFactor), PlayerCombatUnits, false);
                UpdatePowerBar();
            }
            
            if (Input.GetKeyDown(KeyCode.A))
            {
                PlayReadyToStartBattle();
            }
        }

        private void StartBattle()
        {
            var whiteUnits = GetNonEmptyUnits(PlayerCombatUnits);
            var darkUnits = GetNonEmptyUnits(EnemyCombatUnits);
            
            float attackSuccessFactor = 1.0f / _levelConfig.BattleLenghtFactor;
            _battleController.StartBattleSimulation(whiteUnits, darkUnits, attackSuccessFactor);
        }

        private void OnBattleEnded(bool victory)
        {
            StartCoroutine(OnBattleEnded_coroutine(victory));
        }

        private IEnumerator OnBattleEnded_coroutine(bool victory)
        {
            yield return new WaitForSeconds(2.0f);
            _popupManager.showLevelEndPopup(victory);
            if (victory)
            {
                MenuManager.SetNextLevel();   
            }
            
            var troopsPower = victory ? CalculatePowerOfUnits(PlayerCombatUnits) : 0;
            var troopsLeft = victory ? CountAliveUnits(PlayerCombatUnits) : 0;
            
            FindObjectOfType<LevelEndPopup>().SetValues(troopsLeft, troopsPower);
        }

        private static List<CombatUnit> GetNonEmptyUnits(CombatUnit[] units)
        {
            var unitsList = new List<CombatUnit>();
            foreach (var unit in units)
            {
                if (!unit.IsEmpty())
                {
                    unitsList.Add(unit);
                }
            }

            return unitsList;
        }
        
        float _tileSizeX;
        float _tileSizeY;
        float _tileOffsetX;
        float _tileOffsetY;

        private void GenerateBoard(BoardConfig config)
        {   
            _tileSizeX = _boardContainer.rect.width / config.SizeX;
            _tileSizeY = _boardContainer.rect.height / config.SizeY;
            _tileOffsetX = -_boardContainer.rect.width * 0.5f;
            _tileOffsetY = -_boardContainer.rect.height * 0.5f;
            
            _tiles = new BoardTile[config.SizeY,config.SizeX];
            
            for (var i = 0; i < config.SizeX; i++)
            {
                for (var j = config.SizeY-1; j >= 0 ; j--)
                {
                    var pos = new Vector3 {
                        x = i * _tileSizeX + _tileOffsetX,
                        y = j * _tileSizeY + _tileOffsetY
                    };

                    SetTile(CreateTile(pos, i, j), i, j);
                }
            }
        }

        private BoardTile CreateTile(Vector3 pos, int i, int j)
        {
            var obj = Instantiate(_tilePrefab, _boardContainer);
            obj.GetComponent<RectTransform>().localPosition = pos;
            obj.name = "board_tile_i" + i + "_j" + j;
                    
            var tile = obj.GetComponent<BoardTile>();
            tile.Init(i,j, _tileSizeX, _tileSizeY);
            
            return tile;
        }

        private void DestroyTile(int i, int j)
        {
            Destroy(GetTile(i, j).gameObject);
            SetTile(null, i, j);
        }

        private BoardTile GetMoveTile(int i, int j, int iDir, int jDir, bool extendedMove, BoardObjectType moveType)
        {
            int prevIIt = i;
            int prevJIt = j;
           
            BoardTile tile = null;
            
            if (extendedMove)
            {
                int iIt = i + iDir;
                int jIt = j + jDir;

                BoardObjectType? newType = null;
                bool valid = validCoords(iIt, jIt);
                
                while (valid)
                {
                    tile = GetTile(iIt, jIt);

                    if (!tile.IsEmpty())
                    {
                        newType = Core.GetCraftObjectType(moveType, tile.GetObjType());
                        break;
                    }
                    
                    prevIIt = iIt;
                    prevJIt = jIt;
                    iIt += iDir;
                    jIt += jDir;
                    
                    valid = validCoords(jIt, iIt);
                }

                if (tile != null && !tile.IsEmpty() && newType == null)
                {
                    if (prevIIt == i && prevJIt == j)
                    {
                        tile = null;
                    }
                    else
                    {
                        tile = GetTile(prevIIt, prevJIt);
                    }
                }
            }
            else
            {
                int iIt = i + iDir;
                int jIt = j + jDir;
                tile = validCoords(iIt, jIt) ? GetTile(iIt, jIt) : null;
            }

            return tile;
        }

        private void OnMoveCompleted()
        {   
            bool populate = UnityEngine.Random.Range(0.0f, 1.0f) < BoardConfig().PopulateProbability;
            if (populate)
            {
                List<BoardTile> emptyTiles = new List<BoardTile>();
                ForEachTile((tile, i, j) =>
                {
                    if (tile.IsEmpty())
                    {
                        emptyTiles.Add(tile);
                    }
                });

                var populateTile = emptyTiles[UnityEngine.Random.Range(0, emptyTiles.Count)];
                SetWeightedRandomUnit(BoardConfig(), populateTile, true);
            }
            
            _remainingMoves--;
            UpdateMoves();
            UpdatePowerBar();

            if (!MovesAllowed())
            {
                PlayReadyToStartBattle();
            }
        }

        private void PlayReadyToStartBattle()
        {
            StartCoroutine(PlayReadyToStartBattle_coroutine());
        }

        private IEnumerator PlayReadyToStartBattle_coroutine()
        {
            OnNewTurn(0);
            
            yield return new WaitForSeconds(0.5f);
            DeployBoardUnits();

            const float stayDuration = 0.65f;
            const float fadeDuration = 0.5f;
            
            _softPopup.printSoftPopup("Ready to battle in 3...", fadeDuration, stayDuration, null);
            yield return new WaitForSeconds(1.0f);
            
            _softPopup.printSoftPopup("Ready to battle in 2...", fadeDuration, stayDuration, null);
            yield return new WaitForSeconds(1.0f);
            
            _softPopup.printSoftPopup("Ready to battle in 1...", fadeDuration, stayDuration, null);
            yield return new WaitForSeconds(1.0f);
            
            _softPopup.printSoftPopup("Start!", fadeDuration, stayDuration, null);
            yield return new WaitForSeconds(1.0f);
            
            StartBattle();
        }

        private bool MovesAllowed()
        {
            return _remainingMoves > 0;
        }

        private bool validCoords(int i, int j)
        {
            return i >= 0 && j >= 0 && i < BoardConfig().SizeY && j < BoardConfig().SizeX;
        }

        private BoardTile GetTile(int i, int j)
        {
            return _tiles[j, i];
        }
        
        private void SetTile(BoardTile tile, int i, int j)
        {
            _tiles[j, i] = tile;
        }
        
        private void PopuplateBoardObstacles(BoardConfig config)
        {
            int boardSize = config.SizeX * config.SizeY;
            int tileCount = 0;

            var configString = config.BoardObstacles.ToString();
            for(int i = 0; i < configString.Length || i < boardSize; i++)
            {
                char c = configString[i];
                bool validChar = c == '.' || c == 'R';
                if(!validChar)
                {
                    continue;
                }

                int x = tileCount % config.SizeX;
                int y = config.SizeX - tileCount / config.SizeX - 1;

                UnitConfig unitConfig = null;
                if (c == 'R')
                {
                    unitConfig = _coreConfig.FindUnitConfig(BoardObjectType.Rock);
                }

                if (unitConfig != null)
                {
                    GetTile(x, y).SetUnitConfig(unitConfig);    
                }
                
                tileCount++;
            }
        }

        private void PopulateBoard(BoardConfig config, bool animate = false)
        {
            ForEachTile((tile, i ,j) =>
            {
                if (!tile.IsEmpty() && tile.GetConfig().category == Category.Obstacle)
                {
                    return;
                }
                
                SetWeightedRandomUnit(config, tile, animate);
            });
        }

        private void SetWeightedRandomUnit(BoardConfig config, BoardTile tile, bool animate = false)
        {
            var totalWeight = config.TotalWeigh();
            
            int rnd = UnityEngine.Random.Range(0, totalWeight);
            int usedWeight = 0;
            int unitIndex = 0;
                
            while (rnd >= usedWeight)
            {
                usedWeight += config.Units[unitIndex].weight;
                unitIndex++;
            }
                
            tile.SetUnitConfig(config.Units[unitIndex - 1].unit, animate);
        }

        private void ForEachTile(Action<BoardTile, int, int> action)
        {
            for (var j = 0; j < BoardConfig().SizeY; j++)
            {
                for (var i = 0; i < BoardConfig().SizeX; i++)
                {
                    action(_tiles[j, i], i, j);
                }
            }
        }

        private float _powerbarTotalWidth;
        private void SetPowerBar(float value)
        {
            float newWidth = _powerbarTotalWidth * value;
            _powerBarForegroud.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
        }

        private static int CalculatePowerOfUnits(CombatUnit[] units)
        {
            int totalPower = 0;
            foreach (var unit in units)
            {
                totalPower += unit.GetPower();
            }

            return totalPower;
        }

        private int CalculatePowerOfBoard()
        {
            int power = 0;
            ForEachTile((tile, i ,j) =>
            {
                if (tile.IsEmpty() || tile.GetConfig().category != Category.Troop)
                {
                    return;
                }

                power += tile.GetConfig().unitPower * _deployUnitFactor;
            });

            return power;
        }

        private static int CalculatePowerOfUnits(List<CombatUnit> units)
        {
            int totalPower = 0;
            foreach (var unit in units)
            {
                totalPower += unit.GetPower();
            }

            return totalPower;
        }
        
        private void UpdatePowerBar()
        {
            var whitePower = CalculatePowerOfUnits(PlayerCombatUnits) + CalculatePowerOfBoard();
            var darkPower = CalculatePowerOfUnits(EnemyCombatUnits);
            float powerBalance = whitePower / (float)(whitePower + darkPower);
            SetPowerBar(powerBalance);
        }
        
        private static int CountAliveUnits(CombatUnit[] units)
        {
            int count = 0;
            foreach (var u in units)
            {
                count += (u.IsAlive() ? 1 : 0) * u.UnitsAmount;
            }

            return count;
        }
    }
}
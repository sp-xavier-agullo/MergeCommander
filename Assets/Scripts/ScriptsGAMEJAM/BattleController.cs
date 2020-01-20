using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;

namespace GJ18
{
    public class BattleController : MonoBehaviour
    {
        [SerializeField] private GameObject _dustParticlesContainer;
        
        [SerializeField] private GameObject _visualAttackPoints;
        [SerializeField] private GameObject _visualAttackBackPoints;

        [SerializeField] private Transform _skullsContainer;
        public Action OnPowerBalanceChanged;
        public Action<bool> OnBattleEndedCallback;
        public Action<int> OnNewTurnCallback;

        private bool _showLog = true;
        
        public class VisualAttack
        {
            public CombatUnit attacker;
            public CombatUnit defender;
            public int damage;
        }
        public class TurnResult
        {
            public readonly Dictionary<CombatUnit, int> damagedUnits = new Dictionary<CombatUnit, int>();
            public readonly List<VisualAttack> visualAttacks = new List<VisualAttack>();
        }

        private List<CombatUnit> _whiteUnits;
        private List<CombatUnit> _darkUnits;
        private bool _runningBattle;
        private float _nextTurnTime;

        private void Awake()
        {
            StopDust();
        }
        
        public static TurnResult SimulateTurn(List<CombatUnit> whiteUnits_, List<CombatUnit> darkUnits_, float attackSuccessFactor)
        {
            var results = new TurnResult();
            
            List<CombatUnit> battlingWhiteUnits = new List<CombatUnit>();
            List<CombatUnit> battlingDarkUnits = new List<CombatUnit>();
            
            foreach (var unit in whiteUnits_)
            {
                if (unit.IsAlive())
                {
                    battlingWhiteUnits.Add(unit);
                }
            }
            
            foreach (var unit in darkUnits_)
            {
                if (unit.IsAlive())
                {
                    battlingDarkUnits.Add(unit);
                }
            }
            
            ExecuteAttack(battlingWhiteUnits, battlingDarkUnits, attackSuccessFactor, ref results);
            ExecuteAttack(battlingDarkUnits, battlingWhiteUnits, attackSuccessFactor, ref results);

            return results;
        }

        private List<CombatUnit> GetAliveList(List<CombatUnit> units)
        {
            var alives = new List<CombatUnit>();
            foreach (var u in units)
            {
                if (u.IsAlive())
                {
                    alives.Add(u);
                }
            }

            return alives;
        }
        
        private static int CountAliveUnits(List<CombatUnit> units)
        {
            int count = 0;
            foreach (var u in units)
            {
                count += u.IsAlive() ? 1 : 0;
            }

            return count;
        }

        private static void ExecuteAttack(List<CombatUnit> attackUnits, List<CombatUnit> defenseUnits, float attackSuccessFactor, ref TurnResult turnResult)
        {
            foreach (var attackUnit in attackUnits)
            {
                var attackConfig = attackUnit.GetConfig();
                int numAttacks = attackUnit.UnitsAmount * attackConfig.unitNumAttacks;

                for (int i = 0; i < numAttacks; i++)
                {
                    var defenseUnit = defenseUnits[Random.Range(0, defenseUnits.Count)];
                    float chanceOfSuccess = attackConfig.attackChance * (1 - defenseUnit.GetConfig().defenseChance);
                    chanceOfSuccess *= attackSuccessFactor;

                    bool successfullAttack = Random.Range(0.0f, 1.0f) < chanceOfSuccess;
                    if (!successfullAttack)
                    {
                        continue;
                    }

                    if (!turnResult.damagedUnits.ContainsKey(defenseUnit))
                    {
                        turnResult.damagedUnits.Add(defenseUnit, 0);
                    }
                    
                    const int deadUnitsPerSuccess = 1;
                    turnResult.damagedUnits[defenseUnit] += deadUnitsPerSuccess;
                }
            }
        }
        
        
        private int _turnCounter;
        private float _attackSuccessFactor = 1.0f;
        
        public void StartBattleSimulation(List<CombatUnit> whiteUnits, List<CombatUnit> darkUnits, float attackSuccessFactor)
        {
            _turnCounter = 0;
            _whiteUnits = whiteUnits;
            _darkUnits = darkUnits;

            _attackSuccessFactor = attackSuccessFactor;

            RunBattleTurn();
        }

        private TurnResult _pendingTurnResult = null;
        private void RunBattleTurn()
        {
            int numAliveWhiteUtnis = CountAliveUnits(_whiteUnits);
            int numAliveDarkUtnis = CountAliveUnits(_darkUnits);
                    
            bool nextTurn = numAliveWhiteUtnis > 0 && numAliveDarkUtnis > 0;
            if (nextTurn)
            {
                _turnCounter++;
                OnNewTurnCallback(_turnCounter);
                
                _pendingTurnResult = SimulateTurn(_whiteUnits, _darkUnits, _attackSuccessFactor);
                _nextTurnTime = Time.time + 1.0f;
                PlayBattleAnimation(RunBattleTurn);
            }
            else
            {
                bool playerVictory = numAliveWhiteUtnis > 0;
                _runningBattle = false;
                PlayHappyAnimations(playerVictory ? _whiteUnits : _darkUnits);
                OnBattleEndedCallback(playerVictory);
            }
        }

        private int _pendingSkullAnimatinos;
        private void ApplyTurnResult(TurnResult result, bool animate, Action OnEndedCallback)
        {
            foreach (var damagedUnit in result.damagedUnits)
            {
                var combatUnit = damagedUnit.Key;
                var losedTroops = Mathf.Min(damagedUnit.Value, combatUnit.UnitsAmount);

                if (animate)
                {
                    _pendingSkullAnimatinos++;
                    
                    combatUnit.PlayDeads(losedTroops, _skullsContainer, () =>
                    {
                        combatUnit.UnitsAmount -= 1;
                        OnPowerBalanceChanged();

                    }, () =>
                    {
                        _pendingSkullAnimatinos--;
                        if (_pendingSkullAnimatinos == 0)
                        {
                            OnEndedCallback();           
                        }
                    });    
                }
                else
                {
                    combatUnit.UnitsAmount -= losedTroops;
                    OnPowerBalanceChanged();
                    OnEndedCallback();
                }
            }
        }
        
        private void PlayBattleAnimation(Action onFinishedAnimationCallback)
        {
            const float battleDuration = 1.0f;
            StartCoroutine(StartBattleAnimation(battleDuration, onFinishedAnimationCallback));
        }
        
        
        [SerializeField] private bool _DebugSkipAnimation = false;
        [SerializeField] private float _DebugSkipDuration = 0.2f;
        private bool _playingDeads;
        private IEnumerator StartBattleAnimation(float battleDuration, Action onFinishedAnimationCallback)
        {
            bool playAnimations = _pendingTurnResult.damagedUnits.Count > 0 && !_DebugSkipAnimation;
            if (playAnimations)
            {
                DebugLog("==================================");
                DebugLog("-- start battle --");
                GetAliveList(_whiteUnits);

                StartDust();

                DebugLog("-- start attacks animations --");
                SetAttacksAnimations(_darkUnits, true);
                SetAttacksAnimations(_whiteUnits, true);

                yield return new WaitForSeconds(battleDuration);

                DebugLog("-- end attacks animations --");
                SetAttacksAnimations(_darkUnits, false);
                SetAttacksAnimations(_whiteUnits, false);

                StopDust();

                DebugLog("-- little pause --");
                yield return new WaitForSeconds(1.0f);

                DebugLog("-- play deaths --");
                _playingDeads = true;
            }
            
            ApplyTurnResult(_pendingTurnResult, playAnimations, () =>
            {
                _playingDeads = false;
                DebugLog("-- play deaths (ended) --");
            });
            
            if (playAnimations)
            {
                while (_playingDeads)
                {
                    yield return new WaitForSeconds(0.1f);
                }

                DebugLog("-- little pause --");
                yield return new WaitForSeconds(0.3f);
            
                DebugLog("-- finished turn --");
            }
            else
            {
                yield return new WaitForSeconds(_DebugSkipDuration);
            }
            
            onFinishedAnimationCallback();
        }

        private void SetAttacksAnimations(List<CombatUnit> units, bool start)
        {
            var alives = GetAliveList(units);
            float minDelay = 0.0f;
            float maxDelay = 0.1f * Mathf.Sqrt(alives.Count);
            
            foreach (var unit in alives)
            {
                if (start)
                {
                    unit.PlayRandomAttacks(_visualAttackPoints, _visualAttackBackPoints, true, minDelay, maxDelay);    
                }
                else
                {
                    unit.StopRandomAttacks();
                }
            }
        }

        private void StartDust()
        {
            for (int i = 0; i < _dustParticlesContainer.transform.childCount; i++)
            {
                StartDust(_dustParticlesContainer.transform.GetChild(i).GetComponent<ParticleSystem>());
            }
        }
        
        private void StopDust()
        {
            for (int i = 0; i < _dustParticlesContainer.transform.childCount; i++)
            {
                StopDust(_dustParticlesContainer.transform.GetChild(i).GetComponent<ParticleSystem>());
            }
        }

        private static void StartDust(ParticleSystem dustPs)
        {
            var emission = dustPs.emission;
            emission.rateOverTime = 30;
        }
        private static void StopDust(ParticleSystem dustPs)
        {
            var emission = dustPs.emission;
            emission.rateOverTime = 0;
        }

        private void PlayHappyAnimations(List<CombatUnit> units)
        {
            foreach (var unit in units)
            {
                if (!unit.IsEmpty() && unit.IsAlive())
                {
                    unit.PlayHappyAnimation();    
                }
            }
        }

        private void DebugLog(string str)
        {
            if (_showLog)
            {
                Debug.Log(str);    
            }
        }
    }
}
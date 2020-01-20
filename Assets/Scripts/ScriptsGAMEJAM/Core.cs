using System;

namespace GJ18
{
    public enum BoardObjectType
    {
        //Material
        Metal,
        Wood,
        Crystal,
        
        //Objects
        Sword,
        Bow,
        MagicRod,
        Shield,
        Ring,
        Orb,
        
        //Soldiers
        Farmer,
        Swordman,
        AdvSwordman,
        Paladin,
        Bowman,
        AdvBowman,
        Crossbowman,
        Wizard,
        AdvWizard,
        Archmage,
        Dragon,
        
        //Obstacles
        Rock
    }
    
    public enum Category
    {
        Material,
        Object,
        Troop,
        Obstacle
    }
    
    public static class Core
    {
        public static BoardObjectType? GetCraftObjectType(BoardObjectType obj1, BoardObjectType obj2)
        {
            return _craftMatrix[(int) obj1, (int) obj2];
        }
        
        private static BoardObjectType?[,] _craftMatrix;

        public static void Init()
        {
            var numObjects = Enum.GetNames(typeof(BoardObjectType)).Length;
            _craftMatrix = new BoardObjectType?[numObjects,numObjects];

            SetCraftMatrix(BoardObjectType.Metal,BoardObjectType.Metal, BoardObjectType.Sword);
            SetCraftMatrix(BoardObjectType.Wood,BoardObjectType.Wood, BoardObjectType.Bow);
            SetCraftMatrix(BoardObjectType.Crystal,BoardObjectType.Wood, BoardObjectType.MagicRod);
            SetCraftMatrix(BoardObjectType.Metal,BoardObjectType.Wood, BoardObjectType.Shield);
            SetCraftMatrix(BoardObjectType.Crystal,BoardObjectType.Metal, BoardObjectType.Ring);
            SetCraftMatrix(BoardObjectType.Crystal,BoardObjectType.Crystal, BoardObjectType.Orb);
            SetCraftMatrix(BoardObjectType.Farmer,BoardObjectType.Sword, BoardObjectType.Swordman);
            SetCraftMatrix(BoardObjectType.Swordman,BoardObjectType.Shield, BoardObjectType.AdvSwordman);
            SetCraftMatrix(BoardObjectType.Swordman,BoardObjectType.Ring, BoardObjectType.Paladin);
            SetCraftMatrix(BoardObjectType.Farmer,BoardObjectType.Bow, BoardObjectType.Bowman);
            SetCraftMatrix(BoardObjectType.Bowman,BoardObjectType.Shield, BoardObjectType.AdvBowman);
            SetCraftMatrix(BoardObjectType.Bowman,BoardObjectType.Ring, BoardObjectType.Crossbowman);
            SetCraftMatrix(BoardObjectType.Farmer,BoardObjectType.MagicRod, BoardObjectType.Wizard);
            SetCraftMatrix(BoardObjectType.Wizard,BoardObjectType.Shield, BoardObjectType.AdvWizard);
            SetCraftMatrix(BoardObjectType.Wizard,BoardObjectType.Ring, BoardObjectType.Archmage);
            
            SetCraftMatrix(BoardObjectType.Swordman,BoardObjectType.Orb, BoardObjectType.Dragon);
            SetCraftMatrix(BoardObjectType.Bowman,BoardObjectType.Orb, BoardObjectType.Dragon);
            SetCraftMatrix(BoardObjectType.Wizard,BoardObjectType.Orb, BoardObjectType.Dragon);
            
            SetCraftMatrix(BoardObjectType.AdvSwordman,BoardObjectType.Orb, BoardObjectType.Dragon);
            SetCraftMatrix(BoardObjectType.AdvBowman,BoardObjectType.Orb, BoardObjectType.Dragon);
            SetCraftMatrix(BoardObjectType.AdvWizard,BoardObjectType.Orb, BoardObjectType.Dragon);
        }

        private static void SetCraftMatrix(BoardObjectType baseObj1, BoardObjectType baseObj2, BoardObjectType craftObj)
        {
            _craftMatrix[(int)baseObj1, (int)baseObj2] = craftObj;
            _craftMatrix[(int)baseObj2, (int)baseObj1] = craftObj;
        }
        
        
    }
}
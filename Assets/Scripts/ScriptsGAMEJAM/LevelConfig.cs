using System;
using UnityEngine;

namespace GJ18
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "SPGameJam18/LevelConfig", order = 1)]
    public class LevelConfig : ScriptableObject
    {
        [Serializable]
        public class EnemyUnit
        {
            public UnitConfig unit;
            public int amount = 1;
        }

        public string name;
        public int MovesBeforeBattle = 1;
        public float BattleLenghtFactor = 1.0f;
        
        public BoardConfig BoardConfig;
        public EnemyUnit[] EnemyUnits;
    }
}
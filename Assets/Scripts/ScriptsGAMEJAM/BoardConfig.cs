using System;
using UnityEngine;

namespace GJ18
{
    [CreateAssetMenu(fileName = "BoardConfig", menuName = "SPGameJam18/BoardConfig", order = 1)]
    public class BoardConfig : ScriptableObject
    {
        [Serializable]
        public class Unit
        {
            public UnitConfig unit;
            public int weight = 1;
        }

        public int SizeX;
        public int SizeY;
        [SerializeField] public Unit[] Units;
        [SerializeField] public TextAsset BoardObstacles;

        public float PopulateProbability = 0.5f; 

        public int TotalWeigh()
        {
            var total = 0;
            foreach (var u in Units)
            {
                total += u.weight;
            }

            return total;
        }
    }
}
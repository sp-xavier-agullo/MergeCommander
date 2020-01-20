using System;
using UnityEngine;

namespace GJ18
{
    [CreateAssetMenu(fileName = "CoreConfig", menuName = "SPGameJam18/CoreConfig", order = 1)]
    public class CoreConfig : ScriptableObject
    {
        public UnitConfig[] UnitConfigs;

        public UnitConfig FindUnitConfig(BoardObjectType objType)
        {
            foreach (var u in UnitConfigs)
            {
                if (u.type == objType)
                {
                    return u;
                }
            }

            return null;
        }
    }
}
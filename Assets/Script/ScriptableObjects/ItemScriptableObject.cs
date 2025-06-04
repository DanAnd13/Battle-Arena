using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleArena.Parameters
{
    [CreateAssetMenu(menuName = "Configs/ItemSettings")]
    public class ItemScriptableObject : ScriptableObject
    {
        public float HealthRestore = 35;

        public float ShieldHealth = 25;

        public float ShieldTimeDuration = 5;
    }
}

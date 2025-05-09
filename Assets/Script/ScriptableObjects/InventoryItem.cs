using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleArena.Parameters
{
    [System.Serializable]
    public class InventoryItem
    {
        public NamesOfItems Name;
        public enum NamesOfItems
        {
            FastWeapon,
            PowerWeapon,
            Granade,
            Medkit,
            Shield
        }

        public bool IsSingleUse;
        public int Count;
    }
}

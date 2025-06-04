using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleArena.Inventory
{
    [System.Serializable]
    public class InventoryItem
    {
        public NamesOfItems Name;
        public enum NamesOfItems
        {
            FastWeapon,
            PowerWeapon,
            Medkit,
            Shield
        }

        public bool IsSingleUse;
        public int Count;
    }
}

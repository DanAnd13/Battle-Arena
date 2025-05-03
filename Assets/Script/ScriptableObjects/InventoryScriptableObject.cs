using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleArena.Parameters
{
    [CreateAssetMenu(menuName = "Configs/Inventory")]
    public class InventoryScriptableObject : ScriptableObject
    {
        public List<InventoryItem> Items;
    }
}

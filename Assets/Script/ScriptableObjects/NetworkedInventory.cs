using BattleArena.Parameters;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleArena.Inventory
{
    public class NetworkedInventory : NetworkBehaviour
    {
        public List<InventoryItem> Items = new List<InventoryItem>();

        public void Initialize(int size)
        {
            Items = new List<InventoryItem>(new InventoryItem[size]);
        }

        public void SetItem(int index, InventoryItem item)
        {
            if (index < 0 || index >= Items.Count) return;
            Items[index] = new InventoryItem
            {
                Name = item.Name,
                Count = item.Count,
                IsSingleUse = item.IsSingleUse
            };
        }

        public InventoryItem GetItem(int index)
        {
            if (index < 0 || index >= Items.Count) return default;
            var item = Items[index];
            return new InventoryItem
            {
                Name = item.Name,
                Count = item.Count,
                IsSingleUse = item.IsSingleUse
            };
        }

        public void ClearInventory()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i] = new InventoryItem();
            }
        }
    }
}


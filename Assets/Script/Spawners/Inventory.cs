using BattleArena.Parameters;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace BattleArena.Parameters
{
    public class Inventory : MonoBehaviour
    {
        public InventoryScriptableObject InventoryScriptableObject;

        public void LoadInventory(string typeOfItem)
        {
            InventoryItem.NamesOfItems itemName;

            // Спроба конвертувати назву у enum
            if (System.Enum.TryParse(typeOfItem, out itemName))
            {
                // Перевіряємо, чи вже є такий предмет
                InventoryItem existingItem = InventoryScriptableObject.Items.Find(i => i.Name == itemName);
                if (existingItem == null)
                {
                    InventoryItem newItem = new InventoryItem();
                    newItem.Name = itemName;
                    newItem.Count = 1;
                    if (itemName == InventoryItem.NamesOfItems.FastWeapon ||
                        itemName == InventoryItem.NamesOfItems.PowerWeapon)
                    {
                        newItem.IsSingleUse = false;
                    }
                    else
                    {
                        newItem.IsSingleUse = true;
                    }

                    InventoryScriptableObject.Items.Add(newItem);
                }
            }

        }

        public string GetItemFromInventory(string name)
        {
            InventoryItem.NamesOfItems itemName;

            // Спроба конвертувати назву у enum
            if (System.Enum.TryParse(name, out itemName))
            {
                InventoryItem existingItem = InventoryScriptableObject.Items.Find(i => i.Name == itemName);
                return existingItem.Name.ToString();
            }
            return null;
        }

        public void ClearInventory()
        {
            InventoryScriptableObject.Items.Clear();
        }
    }
}

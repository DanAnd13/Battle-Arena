using BattleArena.Loader;
using BattleArena.Parameters;
using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BattleArena.Inventory
{
    public class InventorySelection : NetworkBehaviour
    {
        public GameObject SelectionUI;
        public Button SaveInventoryButton;
        public TextMeshProUGUI WeaponDropdawn;
        public TextMeshProUGUI ItemDropdawn;

        private NetworkedInventory _inventory;

        public override void Spawned()
        {
            _inventory = GetComponent<NetworkedInventory>();
            if (HasInputAuthority)
            {
                SaveInventoryButton.onClick.AddListener(SelectItem);
                SaveInventoryButton.onClick.AddListener(GameBootstrapper.Instance.StartGame);
                ShowUI(true);
            }
            else
            {
                ShowUI(false);
            }
        }

        public void ShowUI(bool value) => SelectionUI.SetActive(value);

        public void SelectItem()
        {
            InventoryItem.NamesOfItems weaponName, itemName;

            Enum.TryParse(WeaponDropdawn.text, out weaponName);
            Enum.TryParse(ItemDropdawn.text, out itemName);

            RPC_SetInventory(weaponName, itemName);
            ShowUI(false);
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_SetInventory(InventoryItem.NamesOfItems weapon, InventoryItem.NamesOfItems item)
        {
            _inventory.SetItem(0, new InventoryItem
            {
                Name = weapon,
                Count = 1,
                IsSingleUse = false
            });

            _inventory.SetItem(1, new InventoryItem
            {
                Name = item,
                Count = 1,
                IsSingleUse = true
            });
        }
    }
}

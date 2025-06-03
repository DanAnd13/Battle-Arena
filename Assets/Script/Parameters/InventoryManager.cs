using BattleArena.InputSynchronize;
using BattleArena.Loader;
using BattleArena.Movement;
using BattleArena.Parameters;
using BattleArena.UI;
using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BattleArena.Inventory
{
    public class InventoryManager : NetworkBehaviour
    {
        public GameObject SelectionWindow;
        public Button SaveInventoryButton;
        public TextMeshProUGUI WeaponDropdawnTMP;
        public TextMeshProUGUI ItemDropdawnTMP;


        private NetworkedInventory _inventory;
        private PlayerHealth _playerHealth;
        private PlayerScriptableObject _playerSettings;
        private bool _isShieldActive = false;
        private float _shieldTimer = 0f;
        private Color _ownColor = Color.yellow;
        private Color _enemyColor = Color.red;

        public override void Spawned()
        {
            _inventory = GetComponent<NetworkedInventory>();
            _inventory.Initialize(2);

            _playerSettings = GetComponent<PlayerMovement>().PlayerSettings;
            _playerHealth = GetComponent<PlayerHealth>();

            if (HasInputAuthority)
            {
                SaveInventoryButton.onClick.AddListener(SelectItem);
                SaveInventoryButton.onClick.AddListener(GameBootstrapper.Instance.StartGame);
                if (_playerHealth.DeathCount == 0)
                {
                    ShowUI(true);
                }
            }
            else
            {
                ShowUI(false);
            }
        }

        public void ShowUI(bool value) => SelectionWindow.SetActive(value);

        public void SelectItem()
        {
            InventoryItem.NamesOfItems weaponName, itemName;

            Enum.TryParse(WeaponDropdawnTMP.text, out weaponName);
            Enum.TryParse(ItemDropdawnTMP.text, out itemName);

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
            Rpc_UpdateInventoryUI(_inventory.Items[0].Name.ToString(), _inventory.Items[1].Name.ToString(), _inventory.Items[1].Count);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
        public void Rpc_UpdateInventoryUI(string weaponName, string itemName, int itemCount)
        {
            if (!HasInputAuthority) return;

            UIManager.Instance.UpdateInventory(weaponName, itemName, itemCount);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void Rpc_SetShieldColor(bool active)
        {
           _playerHealth.FillImage.color = active ? Color.cyan : (Object.HasInputAuthority ? _ownColor : _enemyColor);
        }

        public void HandleItemInput(NetworkInputData data)
        {
            if (!HasStateAuthority || _inventory == null) return;

            if (data.buttons.IsSet(NetworkInputData.USEITEM) && _inventory.Items[1].Count > 0)
            {
                UseItem();
            }
            if (_isShieldActive)
            {
                _shieldTimer -= Runner.DeltaTime;
                if (_shieldTimer <= 0)
                {
                    _isShieldActive = false;
                    Rpc_SetShieldColor(false);
                   _playerHealth.CurrentHealth -= _playerHealth.ItemSettings.ShieldHealth;
                }
            }
        }

        private void UseItem()
        {
            if (!HasStateAuthority) return;

            InventoryItem activeItem = _inventory.Items[1];

            switch (activeItem.Name)
            {
                case InventoryItem.NamesOfItems.Medkit:
                    if (_playerHealth.CurrentHealth < _playerSettings.MaxHealth)
                    {
                        _playerHealth.Heal(_playerHealth.ItemSettings.HealthRestore);
                        _inventory.Items[1].Count = 0;
                        Rpc_UpdateInventoryUI(_inventory.Items[0].Name.ToString(), _inventory.Items[1].Name.ToString(), _inventory.Items[1].Count);
                    }
                    break;

                case InventoryItem.NamesOfItems.Shield:
                    if (!_isShieldActive)
                    {
                        _playerHealth.CurrentHealth += _playerHealth.ItemSettings.ShieldHealth;
                        _shieldTimer = 3f;
                        _isShieldActive = true;
                        Rpc_SetShieldColor(true);
                        _inventory.Items[1].Count = 0;
                        Rpc_UpdateInventoryUI(_inventory.Items[0].Name.ToString(), _inventory.Items[1].Name.ToString(), _inventory.Items[1].Count);
                    }
                    break;

                default:
                    Debug.Log("No usable item");
                    break;
            }
        }

        public void ResetItemAmount()
        {
            foreach (var item in _inventory.Items)
            {
                item.Count = 1;
            }
        }
    }
}

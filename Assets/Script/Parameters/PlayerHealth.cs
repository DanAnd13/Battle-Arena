using BattleArena.InputSynchronize;
using BattleArena.Loader;
using BattleArena.Movement;
using BattleArena.Parameters;
using Fusion;
using System;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BattleArena.Parameters
{
    public class PlayerHealth : NetworkBehaviour
    {
        [Networked]
        public float CurrentHealth { get; set; }
        [Networked]
        public string PlayerNickname { get; set; }
        public Image fillImage;
        public TextMeshProUGUI PlayerName;
        public ItemScriptableObject ItemSettings;

        private NetworkedInventory _networkInventory;
        private PlayerMovement _playerMovement;
        private PlayerScriptableObject _playerSettings;
        private Color _ownColor = Color.yellow;
        private Color _enemyColor = Color.red;
        private float _maxHealth;
        private bool _isShieldActive = false;
        private float _shieldTimer = 0f;
        private void Awake()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _playerSettings = _playerMovement.PlayerSettings;
            //_networkInventory = GetComponent<NetworkedInventory>();
        }

        private void Update()
        {
            if (CurrentHealth <= 0)
            {
                gameObject.SetActive(false);
            }

            if (CurrentHealth != null)
            {
                float normalizedHealth = CurrentHealth / _maxHealth;
                fillImage.fillAmount = Mathf.Clamp01(normalizedHealth);
            }
        }
        public override void Spawned()
        {
            _networkInventory = GameBootstrapper.Instance.GetComponent<NetworkedInventory>();
            _networkInventory.Initialize(2);
            _maxHealth = _playerSettings != null ? _playerSettings.MaxHealth : 100;

            if (HasStateAuthority)
            {
                CurrentHealth = _maxHealth;
                PlayerNickname = "Player" + Object.InputAuthority.PlayerId;
            }

            fillImage.color = Object.HasInputAuthority ? _ownColor : _enemyColor;

            PlayerName.text = PlayerNickname;
        }


        public void HandleItemInput(NetworkInputData data)
        {
            if (!HasStateAuthority || _networkInventory == null) return;

            if (data.buttons.IsSet(NetworkInputData.USEITEM) && _networkInventory.Items[1].Count > 0)
            {
                UseItem();
            }
            if (_isShieldActive)
            {
                _shieldTimer -= Runner.DeltaTime;
                if (_shieldTimer <= 0)
                {
                    _isShieldActive = false;
                    fillImage.color = Object.HasInputAuthority ? _ownColor : _enemyColor;
                    CurrentHealth -= ItemSettings.ShieldHealth;
                }
            }
        }

        private void UseItem()
        {
            if (!HasStateAuthority) return;

            InventoryItem activeItem = _networkInventory.Items[1];

            switch (activeItem.Name)
            {
                case InventoryItem.NamesOfItems.Medkit:
                    if (CurrentHealth < _maxHealth)
                    {
                        Heal(ItemSettings.HealthRestore);
                        _networkInventory.Items[1].Count = 0;
                        Debug.Log("Used Medkit");
                    }
                    break;

                case InventoryItem.NamesOfItems.Shield:
                    if (!_isShieldActive)
                    {
                        CurrentHealth += ItemSettings.ShieldHealth;
                        _shieldTimer = 3f;
                        _isShieldActive = true;
                        fillImage.color = Color.cyan;
                        _networkInventory.Items[1].Count = 0;
                        Debug.Log("Shield Activated");
                    }
                    break;

                default:
                    Debug.Log("No usable item");
                    break;
            }
        }
        public void TakeDamage(float amount)
        {
            Debug.Log($"TakeDamage called, amount = {amount}, HasStateAuthority = {HasStateAuthority}");
            if (HasStateAuthority)
            {
                CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
                Debug.Log("New health: " + CurrentHealth);
            }
        }
        
        public void Heal(float amount)
        {
            if (HasStateAuthority)
            {
                CurrentHealth = Mathf.Min(_maxHealth, CurrentHealth + amount);
                //_networkInventory.Items[1].Count = 0;
            }
        }
    }
}

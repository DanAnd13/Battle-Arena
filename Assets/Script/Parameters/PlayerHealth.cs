using BattleArena.InputSynchronize;
using BattleArena.Movement;
using BattleArena.Inventory;
using Fusion;
using System;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BattleArena.Loader;
using System.Collections;

namespace BattleArena.Parameters
{
    public class PlayerHealth : NetworkBehaviour
    {
        [Networked]
        public float CurrentHealth { get; set; }
        [Networked]
        public string PlayerNickname { get; private set; }
        [Networked]
        public bool IsPlayerDead { get; set; }

        [Networked]
        public int DeathCount { get; set; }

        public Image FillImage;
        public TextMeshProUGUI PlayerName;
        public ItemScriptableObject ItemSettings;

        private PlayerMovement _playerMovement;
        private PlayerScriptableObject _playerSettings;
        private Color _ownColor = Color.yellow;
        private Color _enemyColor = Color.red;
        private float _maxHealth;

        private void Awake()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _playerSettings = _playerMovement.PlayerSettings;
        }

        private void Update()
        {
            if (CurrentHealth <= 0)
            {
                gameObject.SetActive(false);
                IsPlayerDead = true;
                DeathCount ++;
                GameBootstrapper.Instance.RespawnPlayer(this);
            }
            else
            {
                float normalizedHealth = CurrentHealth / _maxHealth;
                FillImage.fillAmount = Mathf.Clamp01(normalizedHealth);
                gameObject.SetActive(true);
            }
        }
        public override void Spawned()
        {
            _maxHealth = _playerSettings != null ? _playerSettings.MaxHealth : 100;
            IsPlayerDead = false;
            if (HasStateAuthority)
            {
                CurrentHealth = _maxHealth;
                PlayerNickname = "Player" + Object.InputAuthority.PlayerId;
            }

            FillImage.color = Object.HasInputAuthority ? _ownColor : _enemyColor;

            PlayerName.text = PlayerNickname;
        }

        public void TakeDamage(float amount)
        {
            if (HasStateAuthority)
            {
                CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            }
        }
        
        public void Heal(float amount)
        {
            if (HasStateAuthority)
            {
                CurrentHealth = Mathf.Min(_maxHealth, CurrentHealth + amount);
            }
        }
    }
}

using BattleArena.InputSynchronize;
using BattleArena.Movement;
using BattleArena.UI;
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
        [Networked]
        public int KillCount { get; set; }

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
            float normalizedHealth = CurrentHealth / _maxHealth;
            FillImage.fillAmount = Mathf.Clamp01(normalizedHealth);
        }

        public override void Spawned()
        {
            _maxHealth = _playerSettings != null ? _playerSettings.MaxHealth : 100;
            IsPlayerDead = false;
            DeathCount = 0;
            KillCount = 0;
            if (HasStateAuthority)
            {
                CurrentHealth = _maxHealth;
                PlayerNickname = "Player" + Object.InputAuthority.PlayerId;
            }

            FillImage.color = Object.HasInputAuthority ? _ownColor : _enemyColor;
            PlayerName.text = PlayerNickname;
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void Rpc_SetAliveState(bool isAlive)
        {
            SetAliveState(isAlive);
        }

        public void SetAliveState(bool isAlive)
        {
            // Наприклад, візуальні об'єкти моделі
            var renderers = GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
                r.enabled = isAlive;

            var colliders = GetComponentsInChildren<Collider>();
            foreach (var c in colliders)
                c.enabled = isAlive;
            var controller = GetComponent<NetworkCharacterController>();
            if (controller != null)
                controller.enabled = isAlive;
            transform.position = Vector3.down * 100f;
            _playerMovement.enabled = isAlive;
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

        public void PlayerDeath()
        {
            IsPlayerDead = true;
            DeathCount++;
            GameBootstrapper.Instance.RespawnPlayer(this);
            Rpc_SetAliveState(false);
        }

        public void RegisterKill(PlayerHealth killer)
        {
            if (killer != null && killer.HasStateAuthority)
            {
                killer.KillCount++;
            }
        }
    }
}

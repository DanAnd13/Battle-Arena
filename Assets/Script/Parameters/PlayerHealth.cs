using BattleArena.Movement;
using BattleArena.Parameters;
using Fusion;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    private PlayerMovement _playerMovement;
    private PlayerScriptableObject _playerSettings;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerSettings = _playerMovement.PlayerSettings;

    }
    [Networked]
    public float CurrentHealth { get; set; }

    private float _maxHealth;

    private void Update()
    {
        if (CurrentHealth <=0)
        {
            gameObject.SetActive(false);
        }
    }

    public override void Spawned()
    {
        if (_playerSettings != null)
            _maxHealth = _playerSettings.MaxHealth;
        else
            _maxHealth = 100; // fallback

        if (HasStateAuthority)
        {
            CurrentHealth = _maxHealth;
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
        }
    }
}

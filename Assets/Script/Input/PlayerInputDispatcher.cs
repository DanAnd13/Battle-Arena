using BattleArena.InputSynchronize;
using BattleArena.Movement;
using BattleArena.Parameters;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputDispatcher : NetworkBehaviour
{
    private PlayerHealth _playerHealth;
    private WeaponController _weaponController;

    public void Init(PlayerHealth health, WeaponController weapon)
    {
        _playerHealth = health;
        _weaponController = weapon;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            _weaponController?.HandleFireInput(data);
            _playerHealth?.HandleItemInput(data);
        }
    }
}

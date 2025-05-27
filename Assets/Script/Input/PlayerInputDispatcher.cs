using BattleArena.InputSynchronize;
using BattleArena.Inventory;
using BattleArena.Movement;
using BattleArena.Parameters;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputDispatcher : NetworkBehaviour
{
    private InventoryManager _inventoryManager;
    private WeaponController _weaponController;

    public void Init(InventoryManager inventory, WeaponController weapon)
    {
        _inventoryManager = inventory;
        _weaponController = weapon;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            _weaponController?.HandleFireInput(data);
            _inventoryManager?.HandleItemInput(data);
        }
    }
}

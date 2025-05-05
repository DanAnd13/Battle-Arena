using Fusion;
using UnityEngine;
using BattleArena.Parameters;
using BattleArena.InputSynchronize;

namespace BattleArena.Movement
{
    public class PlayerMovement : NetworkBehaviour
    {
        public PlayerScriptableObject PlayerSettings;

        private NetworkCharacterController _cc;
        private WeaponController _weapon;

        private void Awake()
        {
            _cc = GetComponent<NetworkCharacterController>();
        }

        public void Init(WeaponController weapon)
        {
            _weapon = weapon;
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData data))
            {
                data.Movement.Normalize();
                _cc.Move(data.Movement * PlayerSettings.MoveSpeed * Runner.DeltaTime);
                _weapon.HandleFireInput(data);
            }
        }
    }
}

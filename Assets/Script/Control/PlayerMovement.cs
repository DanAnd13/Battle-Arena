using Fusion;
using UnityEngine;
using BattleArena.Parameters;
using BattleArena.InputSynchronize;
using BattleArena.Loader;

namespace BattleArena.Movement
{
    public class PlayerMovement : NetworkBehaviour
    {
        [Networked]
        public Vector3 PlayerSpawnPosition { get; set; }

        public PlayerScriptableObject PlayerSettings;

        private NetworkCharacterController _cc;

        private void Awake()
        {
            _cc = GetComponent<NetworkCharacterController>();
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData data))
            {
                data.Movement.Normalize();
                _cc.Move(data.Movement * PlayerSettings.MoveSpeed * Runner.DeltaTime);
            }
        }
    }
}

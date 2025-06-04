using Fusion;
using UnityEngine;
using BattleArena.Parameters;
using BattleArena.InputSynchronize;
using BattleArena.Loader;

namespace BattleArena.Movement
{
    public class PlayerMovement : NetworkBehaviour
    {
        public PlayerScriptableObject PlayerSettings;

        private NetworkCharacterController _cc;

        public override void Spawned()
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

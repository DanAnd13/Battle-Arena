using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleArena.InputSynchronize
{
    public class InputManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        public NetworkInputData NetworkInputData;
        public void Update()
        {
            Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            Vector2 mouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            NetworkInputData.Movement = move;
            NetworkInputData.MouseDelta = mouse;
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            var data = new NetworkInputData
            {
                Movement = NetworkInputData.Movement,
                MouseDelta = NetworkInputData.MouseDelta,
            };

            input.Set(data);
        }
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
        {
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }
    }
}

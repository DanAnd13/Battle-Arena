using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine;
using BattleArena.Parameters;
using System.Collections.Generic;

public class GameBootstrapper : MonoBehaviour, INetworkRunnerCallbacks
{  
    [SerializeField] private GameObject bulletPrefab; // Префаб кулі
    [SerializeField] private NetworkRunner runnerPrefab;

    private GameObject _playerPrefab;
    private GameObject _weapon;
    private NetworkRunner _runner;

    private void Awake()
    {
        _playerPrefab = Resources.Load<GameObject>("Player");
        _weapon = Resources.Load<GameObject>("FastWeapon");
    }

    private void Start()
    {
        _runner = Instantiate(runnerPrefab);
        _runner.ProvideInput = true;
        _runner.AddCallbacks(this);

        _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "TestSession",
            Scene = new NetworkSceneInfo(),
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (player == runner.LocalPlayer)
        {
            Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(-5f, 5f), 1f, UnityEngine.Random.Range(-5f, 5f));
            NetworkObject playerInstance = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);

            Transform cameraPosition = playerInstance.transform.GetChild(0);
            NetworkObject weaponInstance = runner.Spawn(_weapon, Vector3.zero, Quaternion.identity);

            weaponInstance.transform.SetParent(cameraPosition, worldPositionStays: false);
            weaponInstance.transform.localPosition = new Vector3(0f, -0.8f, 1.6f);
            weaponInstance.transform.localRotation = Quaternion.identity;
        }
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }
}

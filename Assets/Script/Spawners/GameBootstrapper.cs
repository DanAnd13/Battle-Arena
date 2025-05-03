using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine;
using BattleArena.Parameters;
using BattleArena.Movement;
using System.Collections.Generic;
using UnityEngine.Pool;
using System.Collections;

public class GameBootstrapper : MonoBehaviour, INetworkRunnerCallbacks
{
    public ObjectPool BulletPool;
    public NetworkRunner RunnerPrefab;
    public NetworkObject BulletPrefab;
    public Transform[] SpawnPoints;

    private int _bulletsToPreload = 30;
    private GameObject _playerPrefab;
    private GameObject _weapon;
    private NetworkRunner _runner;

    private void Awake()
    {
        _playerPrefab = Resources.Load<GameObject>("Player");
        _weapon = Resources.Load<GameObject>("PowerWeapon");
    }

    private void Start()
    {
        _runner = Instantiate(RunnerPrefab);
        _runner.ProvideInput = true;
        _runner.AddCallbacks(this);

        _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "TestSession",
            Scene = new NetworkSceneInfo(),
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
        StartCoroutine(WaitForRunnerAndPreload());
    }

    private IEnumerator WaitForRunnerAndPreload()
    {
        while (_runner.IsRunning == false)
            yield return null;

        if (_runner.IsServer) // тільки сервер спавнить кулі
        {
            PreloadBullets();
        }
    }

    private void PreloadBullets()
    {
        for (int i = 0; i < _bulletsToPreload; i++)
        {
            NetworkObject bullet = _runner.Spawn(BulletPrefab, Vector3.zero, Quaternion.identity, inputAuthority: null, onBeforeSpawned: (runner, obj) =>
            {
                obj.transform.position = Vector3.down * 100f; // тимчасово сховати
            });
            bullet.GetComponent<NetworkObject>().gameObject.GetComponent<MeshRenderer>().enabled = false;
            bullet.GetComponent<Collider>().enabled = false;
            bullet.GetComponent<BulletController>().enabled = false;
            BulletPool.AddBullet(bullet);       // додаємо до пулу
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (player == runner.LocalPlayer)
        {
            int randomIndex = UnityEngine.Random.Range(0, SpawnPoints.Length);
            Vector3 spawnPosition = new Vector3(SpawnPoints[randomIndex].position.x, 
                                    SpawnPoints[randomIndex].position.y + 0.35f, SpawnPoints[randomIndex].position.z);
            NetworkObject playerInstance = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);

            Transform cameraPosition = playerInstance.transform.GetChild(0);
            NetworkObject weaponInstance = runner.Spawn(_weapon, Vector3.zero, Quaternion.identity);

            var weaponController = weaponInstance.GetComponent<WeaponController>();
            if (weaponController != null)
            {
                weaponController.SetObjectPool(BulletPool);
            }

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

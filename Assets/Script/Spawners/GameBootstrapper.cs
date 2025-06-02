using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine;
using BattleArena.Parameters;
using BattleArena.Movement;
using BattleArena.Inventory;
using BattleArena.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;

namespace BattleArena.Loader
{
    public class GameBootstrapper : MonoBehaviour, INetworkRunnerCallbacks
    {
        public ObjectPool ObjectPool;
        public NetworkRunner RunnerPref;
        public Transform[] SpawnPoints;
        [HideInInspector]
        public bool IsPalyerLoading = true;

        public static GameBootstrapper Instance { get; private set; }

        private GameObject _bulletPref;
        private GameObject _playerPref;
        private GameObject _fastWeaponPref;
        private GameObject _powerWeaponPref;
        private GameObject _gameManagerPref;
        private int _preloadCount = 30;
        private float _respawnDelay = 5f;
        private NetworkRunner _runner;
        private GameManager _gameManager;

        private void Awake()
        {
            _bulletPref = Resources.Load<GameObject>("Bullet");
            _playerPref = Resources.Load<GameObject>("Player");
            _fastWeaponPref = Resources.Load<GameObject>("FastWeapon");
            _powerWeaponPref = Resources.Load<GameObject>("PowerWeapon");
            _gameManagerPref = Resources.Load<GameObject>("GameManager");

            Instance = this;
        }
        public void SetRunner(NetworkRunner runner)
        {
            _runner = runner;
            if (_runner.IsServer)
            {
                var manager = _runner.Spawn(_gameManagerPref, Vector3.zero, Quaternion.identity, _runner.LocalPlayer);
                _gameManager = manager.GetComponent<GameManager>();
            }
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
            for (int i = 0; i < _preloadCount; i++)
            {
                NetworkObject bullet = _runner.Spawn(_bulletPref, Vector3.zero, Quaternion.identity, inputAuthority: null, onBeforeSpawned: (runner, obj) =>
                {
                    obj.transform.position = Vector3.down * 100f; // тимчасово сховати
                });
                ObjectPool.AddObject(bullet);
            }
        }

        public void StartGame()
        {
            if (_runner.IsServer)
            {
                foreach (var palyer in _gameManager.SpawnedCharacters.ToList())
                {
                    var playerRef = palyer.Key;
                    LoadPlayersPref(_runner, playerRef);
                }
                _gameManager.TryStartMatch();
            }
        }

        public void RespawnPlayer(PlayerHealth playerHealth)
        {
            if (!playerHealth.Object.HasStateAuthority) return;

            StartCoroutine(RespawnAfterDelay(_respawnDelay, playerHealth));
        }

        private IEnumerator RespawnAfterDelay(float delay, PlayerHealth playerHealth)
        {
            yield return new WaitForSeconds(delay);

            Vector3 spawnPoint = GetRandomSpawnpoint();

            playerHealth.IsPlayerDead = false;
            playerHealth.CurrentHealth = 100;
            playerHealth.Rpc_SetAliveState(true);
            var controller = playerHealth.GetComponent<NetworkCharacterController>();
            controller.Teleport(spawnPoint);
            playerHealth.GetComponent<InventoryManager>().ResetItemAmount();
        }

        public void RespawnAllPlayers()
        {
            foreach (var key in _gameManager.SpawnedCharacters)
            {
                var player = key.Key;
                DespawnPlayer(_runner, player);
                SpawnPlayer(_runner, player);
            }
        }

        public void SpawnPlayer(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                // Спавнимо гравця за межами карти
                Vector3 spawnPosition = GetRandomSpawnpoint();
                NetworkObject playerInstance = runner.Spawn(_playerPref, spawnPosition, Quaternion.identity, player);

                _gameManager.WriteNewPlayer(player, playerInstance);
                playerInstance.GetComponent<NetworkedInventory>().ClearInventory();
            }
        }

        public void DespawnAllPlayers()
        {
            foreach (var key in _gameManager.SpawnedCharacters)
            {
                var player = key.Key;
                DespawnPlayer(_runner, player);
               _gameManager.SpawnedCharacters.Set(player, null);
            }
        }
        
        public void DespawnPlayer(NetworkRunner runner, PlayerRef player) 
        {
            if (_gameManager.SpawnedCharacters.ContainsKey(player))
            {
                NetworkObject networkObject = _gameManager.SpawnedCharacters[player];

                if (networkObject != null)
                {
                    UIManager.Instance.RemovePlayerFromList(networkObject);
                    runner.Despawn(networkObject);
                }
            }
        }

        public Vector3 GetRandomSpawnpoint()
        {
            int randomIndex = UnityEngine.Random.Range(0, SpawnPoints.Length);
            Vector3 spawnPosition = new Vector3(SpawnPoints[randomIndex].position.x,
                                        SpawnPoints[randomIndex].position.y + 0.35f, SpawnPoints[randomIndex].position.z);
            return spawnPosition;
        }

        public void LoadPlayersPref(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                NetworkObject playerInstance = _gameManager.SpawnedCharacters[player];

                InventoryItem weaponName = playerInstance.GetComponent<NetworkedInventory>().GetItem(0);
                NetworkObject weaponInstance = null;
                if (weaponName.Name == InventoryItem.NamesOfItems.FastWeapon)
                {
                    weaponInstance = runner.Spawn(_fastWeaponPref, Vector3.zero, Quaternion.identity, player);
                }
                else if (weaponName.Name == InventoryItem.NamesOfItems.PowerWeapon)
                {
                    weaponInstance = runner.Spawn(_powerWeaponPref, Vector3.zero, Quaternion.identity, player);
                }

                weaponInstance.GetComponent<WeaponController>().RPC_SetPlayer(playerInstance);
                weaponInstance.GetComponent<WeaponController>().Init(playerInstance, ObjectPool);
                var dispatcher = playerInstance.GetComponent<PlayerInputDispatcher>();
                dispatcher.Init(playerInstance.GetComponent<InventoryManager>(), weaponInstance.GetComponent<WeaponController>());
                playerInstance.GetComponent<InventoryManager>().ResetItemAmount();
            }
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            SpawnPlayer(runner, player);
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            DespawnPlayer(runner, player);
            _gameManager.SpawnedCharacters.Remove(player);
        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
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
}

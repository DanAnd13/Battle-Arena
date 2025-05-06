using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine;
using BattleArena.Parameters;
using BattleArena.Movement;
using System.Collections.Generic;
using System.Collections;
using BattleArena.InputSynchronize;
using System.Linq;

namespace BattleArena.Loader
{
    public class GameBootstrapper : MonoBehaviour, INetworkRunnerCallbacks
    {
        public ParticleObjectPool ShootingParticlePool;
        public ObjectPool ObjectPool;
        public Inventory Inventory;
        public NetworkRunner RunnerPref;
        public Transform[] SpawnPoints;
        [HideInInspector]
        public bool IsPlayerJoin = false;
        [HideInInspector]
        public bool IsPalyerLoading = true;
        [HideInInspector]
        public bool IsLocalPlayer = false;
        [HideInInspector]
        public string enteredWeponName;
        [HideInInspector]
        public string enteredItem;

        private GameObject _bulletPref;
        private GameObject _playerPref;
        private GameObject _fastWeaponPref;
        private GameObject _powerWeaponPref;
        private PlayerRef _playerToSpawn;
        private int _preloadCount = 30;
        private NetworkRunner _runner;
        private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new();
        private Dictionary<PlayerRef, string> _playerWeapons = new();

        private void Awake()
        {
            _bulletPref = Resources.Load<GameObject>("Bullet");
            _playerPref = Resources.Load<GameObject>("Player");
            _fastWeaponPref = Resources.Load<GameObject>("FastWeapon");
            _powerWeaponPref = Resources.Load<GameObject>("PowerWeapon");
        }
        public void SetRunner(NetworkRunner runner)
        {
            _runner = runner;
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
            IsPlayerJoin = false;
            Inventory.LoadInventory(enteredWeponName);
            Inventory.LoadInventory(enteredItem);
            //RegisterPlayerWeapon(_playerToSpawn, enteredWeponName);

            if (_runner.IsServer)
            {
                foreach (var palyer in _spawnedCharacters.ToList())
                {
                    var playerRef = palyer.Key;
                    LoadPlayersPref(_runner, playerRef);
                }
                _playerToSpawn = PlayerRef.None; // очистити
            }
        }

        public void LoadPlayersPref(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                int randomIndex = UnityEngine.Random.Range(0, SpawnPoints.Length);
                Vector3 spawnPosition = new Vector3(SpawnPoints[randomIndex].position.x,
                                        SpawnPoints[randomIndex].position.y + 0.35f, SpawnPoints[randomIndex].position.z);
                NetworkObject playerInstance = runner.Spawn(_playerPref, spawnPosition, Quaternion.identity, player);
                _spawnedCharacters[player] = playerInstance;

                string weaponName = _playerWeapons.ContainsKey(player) ? _playerWeapons[player] : "FastWeapon"; // дефолт

                NetworkObject weaponInstance = null;
                if (weaponName == InventoryItem.NamesOfItems.FastWeapon.ToString())
                {
                    weaponInstance = runner.Spawn(_fastWeaponPref, Vector3.zero, Quaternion.identity);
                }
                else if (weaponName == InventoryItem.NamesOfItems.PowerWeapon.ToString())
                {
                    weaponInstance = runner.Spawn(_powerWeaponPref, Vector3.zero, Quaternion.identity);
                }

                weaponInstance.GetComponent<WeaponController>().Init(playerInstance, ObjectPool, ShootingParticlePool);
                playerInstance.GetComponent<PlayerMovement>().Init(weaponInstance.GetComponent<WeaponController>());
            }
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {

            if (runner.LocalPlayer == player)
            {
                Inventory.ClearInventory();
            }
            _spawnedCharacters.Add(player, null);
            IsPlayerJoin = true;
            _playerToSpawn = player;

        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (runner.LocalPlayer == player)
            {
                IsPlayerJoin = false;
                Inventory.ClearInventory();
            }

            if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
            {
                runner.Despawn(networkObject);
                _spawnedCharacters.Remove(player);
            }
            _playerWeapons.Remove(player);
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

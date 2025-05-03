using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BattleArena.Parameters
{
    public class ObjectPool : Fusion.Behaviour, INetworkObjectProvider
    {
        [InlineHelp]
        public bool DelayIfSceneManagerIsBusy = true;

        private Dictionary<NetworkPrefabId, Queue<NetworkObject>> _free = new Dictionary<NetworkPrefabId, Queue<NetworkObject>>();

        private int _maxPoolCount = 30;
        private NetworkObject _bulletPrefab;
        private NetworkPrefabId _bulletPrefabId;

        public NetworkObject BulletPrefab => _bulletPrefab;
        public NetworkPrefabId BulletPrefabId => _bulletPrefabId;

        public void Initialize(NetworkRunner runner)
        {
            _bulletPrefab = Resources.Load<NetworkObject>("Bullet");

            if (_bulletPrefab == null)
            {
                Debug.LogError("Не вдалося завантажити префаб кулі з Resources/Bullet");
                return;
            }

            //_bulletPrefabId = runner.GetPrefabId(_bulletPrefab);
        }

        protected NetworkObject InstantiatePrefab(NetworkRunner runner, NetworkPrefabId contextPrefabId)
        {
            if (_free.TryGetValue(contextPrefabId, out var freeQ) && freeQ.Count > 0)
            {
                var result = freeQ.Dequeue();
                result.gameObject.SetActive(true);
                return result;
            }

            if (!_free.ContainsKey(contextPrefabId))
            {
                _free.Add(contextPrefabId, new Queue<NetworkObject>());
            }

            var instance = Instantiate(_bulletPrefab);
            return instance;
        }

        protected void DestroyPrefabInstance(NetworkRunner runner, NetworkPrefabId prefabId, NetworkObject instance)
        {
            if (!_free.TryGetValue(prefabId, out var freeQ))
            {
                Destroy(instance.gameObject);
                return;
            }

            if (_maxPoolCount > 0 && freeQ.Count >= _maxPoolCount)
            {
                Destroy(instance.gameObject);
                return;
            }

            freeQ.Enqueue(instance);
            instance.gameObject.SetActive(false);
        }

        public NetworkObjectAcquireResult AcquirePrefabInstance(NetworkRunner runner, in NetworkPrefabAcquireContext context, out NetworkObject instance)
        {
            instance = null;

            if (DelayIfSceneManagerIsBusy && runner.SceneManager.IsBusy)
                return NetworkObjectAcquireResult.Retry;

            try
            {
                instance = InstantiatePrefab(runner, context.PrefabId);
                runner.Prefabs.AddInstance(context.PrefabId);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Не вдалося створити інстанцію префабу: {ex}");
                return NetworkObjectAcquireResult.Failed;
            }

            if (context.DontDestroyOnLoad)
                runner.MakeDontDestroyOnLoad(instance.gameObject);
            else
                runner.MoveToRunnerScene(instance.gameObject);

            return NetworkObjectAcquireResult.Success;
        }

        public void ReleaseInstance(NetworkRunner runner, in NetworkObjectReleaseContext context)
        {
            var instance = context.Object;

            if (!context.IsBeingDestroyed)
            {
                if (context.TypeId.IsPrefab)
                    DestroyPrefabInstance(runner, context.TypeId.AsPrefabId, instance);
                else
                    Destroy(instance.gameObject);
            }

            if (context.TypeId.IsPrefab)
                runner.Prefabs.RemoveInstance(context.TypeId.AsPrefabId);
        }

        public void SetMaxPoolCount(int count)
        {
            _maxPoolCount = count;
        }

        public NetworkPrefabId GetPrefabId(NetworkRunner runner, NetworkObjectGuid prefabGuid)
        {
            throw new NotImplementedException();
        }
    }
}

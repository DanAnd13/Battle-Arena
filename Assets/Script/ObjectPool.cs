using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;
using BattleArena.Movement;

namespace BattleArena.Parameters
{
    public class ObjectPool : Fusion.Behaviour, INetworkObjectProvider
    {
        /// <summary>
        /// If enabled, the provider will delay acquiring a prefab instance if the scene manager is busy.
        /// </summary>
        [InlineHelp]
        public bool DelayIfSceneManagerIsBusy = true;

        private Dictionary<NetworkPrefabId, Queue<NetworkObject>> _free = new Dictionary<NetworkPrefabId, Queue<NetworkObject>>();

        /// <summary>
        /// How many objects are going to be kept on the pools, 0 or negative means to pool all released objects.
        /// </summary>
        private int _maxPoolCount = 0;

        /// The base <see cref="NetworkObjectProviderDefault"/> by default simply instantiate a new game object.
        /// Let's create a method to use a custom logic that will pool objects.
        protected NetworkObject InstantiatePrefab(NetworkRunner runner, NetworkObject prefab,
            NetworkPrefabId contextPrefabId)
        {
            var result = default(NetworkObject);

            // Found free queue for prefab AND the queue is not empty. Return free object.
            if (_free.TryGetValue(contextPrefabId, out var freeQ))
            {
                if (freeQ.Count > 0)
                {
                    result = freeQ.Dequeue();
                    result.gameObject.SetActive(true);
                    return result;
                }
            }
            else
            {
                _free.Add(contextPrefabId, new Queue<NetworkObject>());
            }

            // -- At this point a free queue was not yet created or was empty. Create new object.
            result = Instantiate(prefab);

            return result;
        }

        protected void DestroyPrefabInstance(NetworkRunner runner, NetworkPrefabId prefabId, NetworkObject instance)
        {
            if (_free.TryGetValue(prefabId, out var freeQ) == false)
            {
                // No free queue for this prefab. Should be destroyed.
                Destroy(instance.gameObject);
                return;
            }
            else if (_maxPoolCount > 0 && freeQ.Count >= _maxPoolCount)
            {
                // The pool already have the max amount of object we defined. Should be destroyed.
                Destroy(instance.gameObject);
                return;
            }

            // Free queue found. Should cache.
            freeQ.Enqueue(instance);

            // Make objects inactive.
            instance.gameObject.SetActive(false);
        }

        public NetworkObjectAcquireResult AcquirePrefabInstance(NetworkRunner runner, in NetworkPrefabAcquireContext context,
            out NetworkObject instance)
        {

            instance = null;

            if (DelayIfSceneManagerIsBusy && runner.SceneManager.IsBusy)
            {
                return NetworkObjectAcquireResult.Retry;
            }

            NetworkObject prefab;
            try
            {
                prefab = runner.Prefabs.Load(context.PrefabId, isSynchronous: context.IsSynchronous);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to load prefab: {ex}");
                return NetworkObjectAcquireResult.Failed;
            }

            if (!prefab)
            {
                // this is ok, as long as Fusion does not require the prefab to be loaded immediately;
                // if an instance for this prefab is still needed, this method will be called again next update
                return NetworkObjectAcquireResult.Retry;
            }

            instance = InstantiatePrefab(runner, prefab, context.PrefabId);
            Assert.Check(instance);

            if (context.DontDestroyOnLoad)
            {
                runner.MakeDontDestroyOnLoad(instance.gameObject);
            }
            else
            {
                runner.MoveToRunnerScene(instance.gameObject);
            }

            runner.Prefabs.AddInstance(context.PrefabId);
            return NetworkObjectAcquireResult.Success;
        }

        public void ReleaseInstance(NetworkRunner runner, in NetworkObjectReleaseContext context)
        {
            var instance = context.Object;

            // Only pool prefabs.
            if (!context.IsBeingDestroyed)
            {
                if (context.TypeId.IsPrefab)
                {
                    DestroyPrefabInstance(runner, context.TypeId.AsPrefabId, instance);
                }
                else
                {
                    Destroy(instance.gameObject);
                }
            }

            if (context.TypeId.IsPrefab)
            {
                runner.Prefabs.RemoveInstance(context.TypeId.AsPrefabId);
            }
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

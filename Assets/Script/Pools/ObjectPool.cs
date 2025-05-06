using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;
using BattleArena.Movement;
using UnityEngine.Pool;

namespace BattleArena.Parameters
{
    public class ObjectPool : MonoBehaviour
    {
        private readonly Stack<NetworkObject> _bulletPool = new Stack<NetworkObject>();

        public void AddObject(NetworkObject poolObject)
        {
            poolObject.transform.position = Vector3.down * 100f;
            poolObject.GetComponent<SphereCollider>().enabled = false;
            poolObject.GetComponent<BulletController>().enabled = false;
            _bulletPool.Push(poolObject);
        }

        public NetworkObject GetObject()
        {
            if (_bulletPool.Count > 0)
            {
                var bullet = _bulletPool.Pop();
                bullet.GetComponent<SphereCollider>().enabled = true;
                bullet.GetComponent<BulletController>().enabled = true;
                
                return bullet;
            }
            Debug.LogWarning("Bullet pool exhausted!");
            return null;
        }

        public void ReturnObject(NetworkObject poolObj)
        {
            AddObject(poolObj);
        }
    }
}

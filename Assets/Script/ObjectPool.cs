using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;
using BattleArena.Movement;

namespace BattleArena.Parameters
{
    public class ObjectPool : MonoBehaviour
    {
        private readonly Stack<NetworkObject> _bulletPool = new Stack<NetworkObject>();

        public void AddBullet(NetworkObject bullet)
        {
            bullet.transform.position = Vector3.down * 100f;
            bullet.GetComponent<SphereCollider>().enabled = false;
            bullet.GetComponent<BulletController>().enabled = false;
            _bulletPool.Push(bullet);
        }

        public NetworkObject GetBullet()
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

        public void ReturnBullet(NetworkObject bullet)
        {
            AddBullet(bullet);
        }
    }
}

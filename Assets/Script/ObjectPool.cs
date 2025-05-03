using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;
using BattleArena.Movement;

namespace BattleArena.Parameters
{
    public class ObjectPool : NetworkBehaviour
    {
        public NetworkObject BulletPrefab;

        private Queue<NetworkObject> _bulletPool = new Queue<NetworkObject>();

        // Спавн кулі
        public NetworkObject GetBullet()
        {
            NetworkObject bullet;
            if (_bulletPool.Count > 0)
            {
                bullet = _bulletPool.Dequeue();  // Виймаємо кулю з пулу
                bullet.GetComponent<NetworkObject>().gameObject.GetComponent<MeshRenderer>().enabled = true;
                bullet.GetComponent<Collider>().enabled = true;
                bullet.GetComponent<BulletController>().enabled = true;
            }
            else
            {
                bullet = Runner.Spawn(BulletPrefab, Vector3.zero, Quaternion.identity); // Спавн нової кулі
            }

            return bullet;
        }

        public void AddBullet(NetworkObject bullet)
        {
            _bulletPool.Enqueue(bullet);
        }

        public void ReturnBullet(NetworkObject bullet)
        {
            bullet.GetComponent<NetworkObject>().gameObject.GetComponent<MeshRenderer>().enabled = false;
            bullet.GetComponent<Collider>().enabled = false;
            bullet.GetComponent<BulletController>().enabled = false;
            _bulletPool.Enqueue(bullet); // Додавання в пул
        }
    }
}

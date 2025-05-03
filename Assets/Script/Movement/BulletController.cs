using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleArena.Parameters;

namespace BattleArena.Movement
{
    public class BulletController : NetworkBehaviour
    {
        private float speed;
        private float lifeTime;
        private float timer;
        private Vector3 direction;
        private ObjectPool objectPool; // посилання на пул

        public void Init(Vector3 direction, float speed, float lifeTime, ObjectPool pool)
        {
            this.direction = direction.normalized;
            this.speed = speed;
            this.lifeTime = lifeTime;
            this.objectPool = pool;
            timer = 0f;

            // Увімкнути компоненти
            GetComponent<MeshRenderer>().enabled = true;
            GetComponent<Collider>().enabled = true;
            enabled = true; // увімкнути цей скрипт
        }

        private void Update()
        {
            if (Object.HasStateAuthority)
            {
                timer += Time.deltaTime;
                if (timer >= lifeTime)
                {
                    ReturnToPool();
                    return;
                }

                transform.Translate(direction * speed * Time.deltaTime, Space.World);
            }
        }

        private void ReturnToPool()
        {
            // Вимкнути всі компоненти
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            enabled = false;

            transform.position = Vector3.down * 100f;

            objectPool?.ReturnBullet(GetComponent<NetworkObject>());
        }
    }
}

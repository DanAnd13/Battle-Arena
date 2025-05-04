using Fusion;
using UnityEngine;
using BattleArena.Parameters;

namespace BattleArena.Movement
{
    public class BulletController : NetworkBehaviour
    {
        private float speed;
        private float timer;
        private Vector3 direction;
        private ObjectPool objectPool;
        [Networked] 
        private TickTimer lifeTime { get; set; }

        public void Init(Vector3 direction, float speed, float lifeTime, ObjectPool pool)
        {
            this.direction = direction.normalized;
            this.speed = speed;
            this.lifeTime = TickTimer.CreateFromSeconds(Runner, lifeTime);
            this.objectPool = pool;
            timer = 0f;

            //gameObject.SetActive(true);
            //GetComponent<SphereCollider>().enabled = true;
            enabled = true;
        }

        public override void FixedUpdateNetwork()
        {
            if (lifeTime.Expired(Runner))
                ReturnToPool();
                //Runner.Despawn(Object);
            else
                transform.position += direction * speed * Runner.DeltaTime;
        }

        private void ReturnToPool()
        {
            GetComponent<SphereCollider>().enabled = false;
            enabled = false;

            // Приховуємо об'єкт або повертаємо до початкової позиції
            transform.position = Vector3.down * 100f;

            objectPool?.ReturnBullet(GetComponent<NetworkObject>());
            //gameObject.SetActive(false);
        }
    }
}

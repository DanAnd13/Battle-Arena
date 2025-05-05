using Fusion;
using UnityEngine;
using BattleArena.Parameters;

namespace BattleArena.Movement
{
    public class BulletController : NetworkBehaviour
    {
        private float _speed;
        private Vector3 _direction;
        private ObjectPool _objectPool;
        [Networked] 
        private TickTimer _lifeTime { get; set; }

        public void Init(Vector3 direction, float speed, float lifeTime, ObjectPool pool)
        {
            _direction = direction.normalized;
            _speed = speed;
            _lifeTime = TickTimer.CreateFromSeconds(Runner, lifeTime);
            _objectPool = pool;

            enabled = true;
        }

        public override void FixedUpdateNetwork()
        {
            if (_lifeTime.Expired(Runner))
                ReturnToPool();
            else
                transform.position += _direction * _speed * Runner.DeltaTime;
        }

        private void ReturnToPool()
        {
            GetComponent<SphereCollider>().enabled = false;

            enabled = false;
            transform.position = Vector3.down * 100f;

            _objectPool?.ReturnObject(GetComponent<NetworkObject>());
        }
    }
}

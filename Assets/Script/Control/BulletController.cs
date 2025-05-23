using Fusion;
using UnityEngine;
using BattleArena.Parameters;

namespace BattleArena.Movement
{
    public class BulletController : NetworkBehaviour
    {
        private float _speed;
        private float _damage;
        private Vector3 _direction;
        private ObjectPool _objectPool;

        [Networked] 
        private TickTimer _lifeTime { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            if (!HasStateAuthority)
                return;

            var health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                Debug.Log("Found PlayerHealth on " + other.name);
                RPC_ApplyDamage(health.Object, _damage);
                //ReturnToPool();
            }
        }

       [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_ApplyDamage(NetworkObject targetPlayer, float damage)
        {
            Debug.Log($"[RPC_ApplyDamage] Called for {targetPlayer}, damage = {damage}");
        
            var health = targetPlayer.GetComponent<PlayerHealth>();
            if (health != null)
            {
                Debug.Log($"Applying damage to {targetPlayer} ({targetPlayer.name})");
                health.TakeDamage(damage);
            }
            else
            {
                Debug.LogWarning("PlayerHealth not found on player object!");
            }
        }


        public void Init(Vector3 direction, float speed, float damage, float lifeTime, ObjectPool pool)
        {
            _direction = direction.normalized;
            _speed = speed;
            _damage = damage;
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

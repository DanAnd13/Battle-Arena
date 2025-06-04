using Fusion;
using UnityEngine;
using BattleArena.Parameters;
using BattleArena.Loader;
using System.Threading;

namespace BattleArena.Movement
{
    public class BulletController : NetworkBehaviour
    {
        public PlayerHealth Shooter { get; set; }

        private float _speed;
        private float _damage;
        private Vector3 _direction;
        private ObjectPool _objectPool;

        [Networked] private TickTimer _lifeTime { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            if (!HasStateAuthority)
                return;

            var health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                RPC_ApplyDamage(health, _damage);
            }
            RPC_PlayParticle(other.transform.position, other.transform.forward);
            Rpc_PlaySoundEffects(health);
            ReturnToPool();
        }

       [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_ApplyDamage(PlayerHealth health, float damage)
        {
            health.TakeDamage(damage);
            if (!health.IsPlayerDead && health.CurrentHealth <= 0)
            {
                health.PlayerDeath();
                Shooter.AddKill();
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void Rpc_PlaySoundEffects(PlayerHealth player)
        {
            var audio = player.GetComponent<AudioSource>();
            audio.clip = GameBootstrapper.Instance.HitSoundEffect;
            audio.Play();
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
            if (!Object || !Object.IsValid) return;

            GetComponent<SphereCollider>().enabled = false;
            enabled = false;
            transform.position = Vector3.down * 100f;

            _lifeTime = TickTimer.None;

            if (_objectPool != null)
            {
                _objectPool.ReturnObject(Object);
            }
            else
            {
                Debug.LogWarning("Object pool not assigned!");
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_PlayParticle(Vector3 targetPos, Vector3 targetRotation)
        {
            PlayParticle(targetPos, targetRotation);
        }

        private void PlayParticle(Vector3 target, Vector3 targetRotation)
        {
            ParticleSystem shootingParticle = GameBootstrapper.Instance.HitEffects.GetPooledObject();
            shootingParticle.transform.position = target;
            shootingParticle.transform.rotation = Quaternion.LookRotation(targetRotation);
            shootingParticle.Play();
        }
    }
}

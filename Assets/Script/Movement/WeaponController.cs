using BattleArena.InputSynchronize;
using BattleArena.Parameters;
using Fusion;
using System.Collections;
using UnityEngine;

namespace BattleArena.Movement
{
    public class WeaponController : NetworkBehaviour
    {
        public WeaponScriptableObject WeaponSettings;
        public Transform Barrel;
        public BulletController BulletPref;

        [Networked] private TickTimer _fireCooldown { get; set; }
        [Networked] private NetworkObject _playerObject { get; set; }

        private ObjectPool _pool;
        private ParticleObjectPool _shootParticle;

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_SetPlayer(NetworkObject playerObj)
        {
            _playerObject = playerObj;

            Transform weaponPosition = _playerObject.transform.GetChild(0);
            transform.SetParent(weaponPosition, false);
            transform.localPosition = new Vector3(0f, 0f, 1f);
            transform.localRotation = Quaternion.identity;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_PlayParticle()
        {
            PlayParticle();
        }

        public override void Spawned()
        {
            _shootParticle = FindFirstObjectByType<ParticleObjectPool>();
        }

        public void Init(NetworkObject playerObj, ObjectPool pool)
        {
            _playerObject = playerObj;
            _pool = pool;
        }

        public void HandleFireInput(NetworkInputData data)
        {
            if (_fireCooldown.ExpiredOrNotRunning(Runner) && data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
            {
                try
                {
                    var bullet = _pool.GetObject();
                    if (bullet == null) return;

                    bullet.transform.position = Barrel.position;
                    bullet.transform.rotation = Quaternion.LookRotation(Barrel.forward);

                    var bulletCtrl = bullet.GetComponent<BulletController>();
                    bulletCtrl.Init(Barrel.forward, WeaponSettings.BulletSpeed, 1f, _pool);

                    RPC_PlayParticle();

                    _fireCooldown = TickTimer.CreateFromSeconds(Runner, WeaponSettings.ReloadTime);
                }
                catch { }
            }
        }
        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData data))
            {
                HandleFireInput(data);
            }          
        }

        private void PlayParticle()
        {
            ParticleSystem shootingParticle = _shootParticle.GetPooledObject();
            shootingParticle.transform.position = Barrel.position;
            shootingParticle.transform.rotation = Quaternion.LookRotation(Barrel.forward);
            shootingParticle.Play();
        }
    }
}

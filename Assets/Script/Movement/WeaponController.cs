using BattleArena.InputSynchronize;
using BattleArena.Parameters;
using Fusion;
using UnityEditor.SceneManagement;
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
        public override void Spawned()
        {
            if (_playerObject != null)
            {
                Transform weaponPosition = _playerObject.transform.GetChild(0);
                transform.SetParent(weaponPosition, false);
                transform.localPosition = new Vector3(0f, 0f, 1f);
                transform.localRotation = Quaternion.identity;
            }
        }

        public void Init(NetworkObject playerObj, ObjectPool pool, ParticleObjectPool shootParticle)
        {
            _playerObject = playerObj;
            _pool = pool;
            _shootParticle = shootParticle;

            if (_playerObject != null)
            {
                Transform weaponPosition = _playerObject.transform.GetChild(0);
                transform.SetParent(weaponPosition, false);
                transform.localPosition = new Vector3(0f, 0f, 1f);
                transform.localRotation = Quaternion.identity;
            }
        }

        public void HandleFireInput(NetworkInputData data)
        {
            if (_fireCooldown.ExpiredOrNotRunning(Runner) && data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
            {
                var bullet = _pool.GetObject();
                if (bullet == null) return;

                bullet.transform.position = Barrel.position;
                bullet.transform.rotation = Quaternion.LookRotation(Barrel.forward);

                var bulletCtrl = bullet.GetComponent<BulletController>();
                bulletCtrl.Init(Barrel.forward, WeaponSettings.BulletSpeed, 1f, _pool);

                ParticleSystem shootingParticle = _shootParticle.GetPooledObject();
                shootingParticle.transform.position = Barrel.position;
                shootingParticle.transform.rotation = Quaternion.LookRotation(Barrel.forward);
                shootingParticle.Play();

                _fireCooldown = TickTimer.CreateFromSeconds(Runner, WeaponSettings.ReloadTime);
            }
        }
    }
}

using BattleArena.InputSynchronize;
using BattleArena.Parameters;
using ExitGames.Client.Photon;
using Fusion;
using UnityEngine;
using UnityEngine.Pool;

namespace BattleArena.Movement
{
    public class WeaponController : NetworkBehaviour
    {
        public WeaponScriptableObject WeaponSettings;
        public Transform Barrel;
        public BulletController BulletPref;

        [Networked] private TickTimer fireCooldown { get; set; }
        [Networked] private NetworkObject PlayerObject { get; set; }
        private ObjectPool _pool;

        public override void Spawned()
        {
            if (PlayerObject != null)
            {
                Transform weaponPosition = PlayerObject.transform.GetChild(0); // як у спавнері
                transform.SetParent(weaponPosition, false);
                transform.localPosition = new Vector3(0f, 0f, 1f);
                transform.localRotation = Quaternion.identity;
            }
        }

        public void Init(NetworkObject playerObj, ObjectPool pool)
        {
            PlayerObject = playerObj;
            _pool = pool;
            if (PlayerObject != null)
            {
                Transform weaponPosition = PlayerObject.transform.GetChild(0); // як у спавнері
                transform.SetParent(weaponPosition, false);
                transform.localPosition = new Vector3(0f, 0f, 1f);
                transform.localRotation = Quaternion.identity;
            }
        }

        public void HandleFireInput(NetworkInputData data)
        {
            if (fireCooldown.ExpiredOrNotRunning(Runner) && data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
            {
                var bullet = _pool.GetBullet();
                if (bullet == null) return;

                bullet.transform.position = Barrel.position;
                bullet.transform.rotation = Quaternion.LookRotation(Barrel.forward);

                var bulletCtrl = bullet.GetComponent<BulletController>();
                bulletCtrl.Init(Barrel.forward, WeaponSettings.BulletSpeed, 1f, _pool); // передаємо pool

                fireCooldown = TickTimer.CreateFromSeconds(Runner, WeaponSettings.ReloadTime);
            }
        }
    }

}

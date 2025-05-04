using BattleArena.InputSynchronize;
using BattleArena.Parameters;
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

        public void Init(NetworkObject playerObj)
        {
            PlayerObject = playerObj;
        }

        public void HandleFireInput(NetworkInputData data)
        {
            if (fireCooldown.ExpiredOrNotRunning(Runner) && data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
            {
                Vector3 spawnPos = Barrel.position;
                Vector3 direction = Barrel.forward;
                Quaternion rotation = Quaternion.LookRotation(direction);

                Runner.Spawn(BulletPref, spawnPos, rotation, Object.InputAuthority,
                    (runner, o) =>
                    {
                        o.GetComponent<BulletController>().Init(direction, WeaponSettings.BulletSpeed, 1f);
                    });

                fireCooldown = TickTimer.CreateFromSeconds(Runner, WeaponSettings.ReloadTime);
            }
        }
    }

}

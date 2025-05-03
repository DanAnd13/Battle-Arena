using BattleArena.Parameters;
using Fusion;
using UnityEngine;
using UnityEngine.Pool;

namespace BattleArena.Movement
{
    public class WeaponController : NetworkBehaviour
    {
        public WeaponScriptableObject BulletSettings; // Налаштування кулі
        public Transform FirePoint;                   // Точка вогню

        private ObjectPool _objectPool;

        private void Update()
        {
            if (Runner == null || Runner.LocalPlayer == null) return;

            if (Input.GetMouseButtonDown(0)) // Якщо натиснута ліва кнопка миші
            {
                SpawnBullet();
            }
        }
        public void SetObjectPool(ObjectPool pool)
        {
            _objectPool = pool;
        }

        // Метод для спавну кулі
        private void SpawnBullet()
        {
            var bullet = _objectPool.GetBullet();
            if (bullet == null) return;

            bullet.transform.position = FirePoint.position;
            bullet.transform.rotation = Quaternion.LookRotation(FirePoint.forward);

            var bulletController = bullet.GetComponent<BulletController>();
            if (bulletController != null)
            {
                bulletController.Init(FirePoint.forward, BulletSettings.BulletSpeed, 1f, _objectPool);
            }
        }
    }
}

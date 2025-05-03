using BattleArena.Parameters;
using Fusion;
using UnityEngine;

public class WeaponController : NetworkBehaviour
{
    public WeaponScriptableObject bulletSettings; // Налаштування кулі
    public Transform firePoint;                   // Точка вогню

    private NetworkObject bulletPrefab;

    private void Start()
    {
        // Завантаження префабу кулі з Resources
        bulletPrefab = Resources.Load<NetworkObject>("Bullet");
    }

    private void Update()
    {
        if (Runner == null || Runner.LocalPlayer == null) return;

        if (Input.GetMouseButtonDown(0)) // Якщо натиснута ліва кнопка миші
        {
            SpawnBullet();
        }
    }

    // Метод для спавну кулі
    private void SpawnBullet()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet prefab not assigned!");
            return;
        }

        // Отримуємо позицію камери або зброї для визначення місця спавну
        Vector3 spawnPosition = firePoint.position;
        Vector3 shootDirection = firePoint.forward;

        // Спавнимо кулю
        Runner.Spawn(bulletPrefab, spawnPosition, Quaternion.identity, Runner.LocalPlayer, (runner, obj) =>
        {
            Debug.Log("Bullet spawned!");

            // Ініціалізуємо кулю після спавну
            var bulletController = obj.GetComponent<BulletController>();
            if (bulletController != null)
            {
                bulletController.Init(shootDirection, bulletSettings.BulletSpeed, 1f); // Задаємо параметри кулі
            }
        });
    }
}

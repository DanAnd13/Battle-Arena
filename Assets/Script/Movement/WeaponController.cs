using BattleArena.Parameters;
using Fusion;
using UnityEngine;

public class WeaponController : NetworkBehaviour
{
    public WeaponScriptableObject bulletSettings; // ������������ ���
    public Transform firePoint;                   // ����� �����

    private NetworkObject bulletPrefab;

    private void Start()
    {
        // ������������ ������� ��� � Resources
        bulletPrefab = Resources.Load<NetworkObject>("Bullet");
    }

    private void Update()
    {
        if (Runner == null || Runner.LocalPlayer == null) return;

        if (Input.GetMouseButtonDown(0)) // ���� ��������� ��� ������ ����
        {
            SpawnBullet();
        }
    }

    // ����� ��� ������ ���
    private void SpawnBullet()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet prefab not assigned!");
            return;
        }

        // �������� ������� ������ ��� ���� ��� ���������� ���� ������
        Vector3 spawnPosition = firePoint.position;
        Vector3 shootDirection = firePoint.forward;

        // �������� ����
        Runner.Spawn(bulletPrefab, spawnPosition, Quaternion.identity, Runner.LocalPlayer, (runner, obj) =>
        {
            Debug.Log("Bullet spawned!");

            // ���������� ���� ���� ������
            var bulletController = obj.GetComponent<BulletController>();
            if (bulletController != null)
            {
                bulletController.Init(shootDirection, bulletSettings.BulletSpeed, 1f); // ������ ��������� ���
            }
        });
    }
}

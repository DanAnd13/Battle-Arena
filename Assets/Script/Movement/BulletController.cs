using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : NetworkBehaviour
{
    private float speed;
    private float lifeTime;
    private float timer;

    public void Init(Vector3 direction, float speed, float lifeTime)
    {
        this.speed = speed;
        this.lifeTime = lifeTime;
        timer = 0f;
        GetComponent<Rigidbody>().velocity = direction * speed;
    }

    private void Update()
    {
        if (Object.HasStateAuthority)
        {
            timer += Time.deltaTime;
            if (timer >= lifeTime)
            {
                Runner.Despawn(Object); // або віддати назад у пул
            }
        }
    }
}

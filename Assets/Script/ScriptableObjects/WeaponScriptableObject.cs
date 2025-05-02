using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Configs/WeaponSettings")]
public class WeaponScriptableObject : ScriptableObject
{
    public float DamageByShot = 10;
    public float ReloadTime = 0.5f;
    public float BulletSpeed = 10;
}

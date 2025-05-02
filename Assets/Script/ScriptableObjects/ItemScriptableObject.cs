using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/ItemSettings")]
public class ItemScriptableObject : ScriptableObject
{
    public float HealthRestore = 35;

    public float ShieldHealth = 25;

    public float GranadeDamage = 25;
    public float GranadeSpeed = 10;
    public float GranadeRadius = 15;
    public float GranadeDelay = 2;
}

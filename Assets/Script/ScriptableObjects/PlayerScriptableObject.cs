using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/PlayerSettings")]
public class PlayerScriptableObject : ScriptableObject
{
    public float MaxHealth = 100;
    public float MoveSpeed = 5;
    public float RotationSpeed = 180;
}

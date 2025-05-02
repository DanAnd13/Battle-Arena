using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Inventory")]
public class InventoryScriptableObject : ScriptableObject
{
    public List<InventoryItem> Items;
}

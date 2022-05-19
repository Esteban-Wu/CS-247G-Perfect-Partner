using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Default Object", menuName = "Inventory System/Items/Default")]
public class ItemObject : ScriptableObject
{
    public string name;
    [TextArea(25, 20)]
    public string description;
    public GameObject icon;
    public GameObject prefab;
}

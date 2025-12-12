using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/ObjectDescription")]
public class ObjDescClass : ScriptableObject
{
    public string objDescID;
    [TextArea(3, 10)]
    public string objDescText;
}

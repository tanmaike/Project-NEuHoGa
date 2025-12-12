using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjDescManager : MonoBehaviour
{
    public static ObjDescManager Instance;

    [Header("ObjDesc List")]
    public List<ObjDescClass> allDescriptions = new List<ObjDescClass>();

    private Dictionary<string, string> descLookup = new Dictionary<string, string>();

    private void Awake()
    {
        Instance = this;

        foreach (var desc in allDescriptions)
        {
            if (desc != null && !descLookup.ContainsKey(desc.objDescID))
            {
                descLookup.Add(desc.objDescID, desc.objDescText);
            }
        }
    }

    public string GetDesc(string id)
    {
        if (descLookup.TryGetValue(id, out string text)) return text;

        Debug.LogWarning($"Object Description ID '{id}' not found!");
        return "Nothing to see here."; 
    }
}

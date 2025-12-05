using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// extremely basic script to spawn game objects whenever a player enters a trigger
public class TriggerScript : MonoBehaviour
{
    //triggered item must be set inactive/active
    public GameObject triggeredObject;
    public bool deleteObject = false;
    private void OnTriggerEnter(Collider other)
    {
        if (deleteObject) Destroy(triggeredObject);
        else triggeredObject.SetActive(true);
        Destroy(gameObject);
    }
}

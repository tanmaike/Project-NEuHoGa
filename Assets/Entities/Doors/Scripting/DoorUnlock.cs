using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorUnlock : MonoBehaviour
{
    // simple script to unlock a specified door
    public Door triggeredDoor;
    private void OnTriggerEnter(Collider other)
    {
        triggeredDoor.Unlock();
        Destroy(gameObject);
    }
}

using UnityEngine;
using System.Collections;

public class DoorTrigger : MonoBehaviour
{
    public Door door; 
    public float distance = 3f;
    public LayerMask layerMask;
    private bool? isOpen = null; 

    private void Start()
    {
        CheckTrigger();
    }

    private void FixedUpdate()
    {
        CheckTrigger();
    }

    private void CheckTrigger()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, distance, layerMask);
        bool currentlyOpen = colliders.Length > 0;

        if (isOpen == null || currentlyOpen != isOpen.Value)
        {
            isOpen = currentlyOpen;

            if (isOpen.Value)
            {
                Vector3 userPos = GetClosestPosition(colliders);
                door.Open(userPos);
            }
            else
            {
                door.Close();
            }
        }
    }

    private Vector3 GetClosestPosition(Collider[] colliders)
    {
        if (colliders.Length == 0) return transform.position + transform.forward;

        Vector3 closest = colliders[0].transform.position;
        float minDist = Vector3.Distance(transform.position, closest);

        foreach (var c in colliders)
        {
            float d = Vector3.Distance(transform.position, c.transform.position);
            if (d < minDist)
            {
                closest = c.transform.position;
                minDist = d;
            }
        }

        return closest;
    }
}
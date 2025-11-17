using UnityEngine;
using System;

public class PortalNavigationHandler : MonoBehaviour
{
    public event Action<Portal, Portal, Vector3, Quaternion> OnPortalTeleport;
    
    private PortalTraveller portalTraveller;
    private UnityEngine.AI.NavMeshAgent navAgent;

    private void Awake()
    {
        portalTraveller = GetComponent<PortalTraveller>();
        navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    public void HandlePortalTeleport(Portal sender, Portal destination, Vector3 newPosition, Quaternion newRotation)
    {
        OnPortalTeleport?.Invoke(sender, destination, newPosition, newRotation);
    }
}
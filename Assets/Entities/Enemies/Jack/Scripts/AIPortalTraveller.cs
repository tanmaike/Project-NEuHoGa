using UnityEngine;
using UnityEngine.AI;

public class AIPortalTraveller : PortalTraveller
{
    private NavMeshAgent navAgent;
    private HostileAI hostileAI;
    private bool isTeleporting;
    
    public System.Action<Portal, Portal, Vector3, Quaternion> OnAITeleported;

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        hostileAI = GetComponent<HostileAI>();
        inPortal = false;
    }

    public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
    {
        if (isTeleporting) return;
        
        isTeleporting = true;
        
        // Store the current destination before teleporting
        Vector3? currentDestination = null;
        if (hostileAI != null)
        {
            currentDestination = hostileAI.GetCurrentDestination();
        }
        
        // Disable NavMeshAgent during teleport
        if (navAgent != null)
        {
            navAgent.enabled = false;
        }
        
        // Perform the teleportation
        transform.position = pos;
        transform.rotation = rot;
        
        // Re-enable NavMeshAgent after teleport
        if (navAgent != null)
        {
            // Use Warp to update NavMeshAgent position
            navAgent.enabled = true;
            if (!navAgent.Warp(pos))
            {
                Debug.LogWarning($"NavMeshAgent warp failed for {gameObject.name}, using transform position");
                transform.position = pos;
            }
        }
        
        // Notify about teleportation
        OnAITeleported?.Invoke(fromPortal.GetComponent<Portal>(), toPortal.GetComponent<Portal>(), pos, rot);
        
        // Restore path after a frame
        StartCoroutine(RestorePathAfterTeleport(currentDestination));
        
        isTeleporting = false;
    }
    
    private System.Collections.IEnumerator RestorePathAfterTeleport(Vector3? destination)
    {
        yield return new WaitForEndOfFrame();
        
        if (destination.HasValue && hostileAI != null && navAgent != null && navAgent.isOnNavMesh)
        {
            hostileAI.Goto(destination.Value);
        }
    }

    public override void EnterPortalThreshold()
    {
        if (inPortal) return;
        base.EnterPortalThreshold();
        
        // Additional AI-specific portal entry logic
        if (navAgent != null)
        {
            navAgent.autoTraverseOffMeshLink = false;
        }
    }

    public override void ExitPortalThreshold()
    {
        base.ExitPortalThreshold();
        
        if (navAgent != null)
        {
            navAgent.autoTraverseOffMeshLink = true;
        }
    }
}
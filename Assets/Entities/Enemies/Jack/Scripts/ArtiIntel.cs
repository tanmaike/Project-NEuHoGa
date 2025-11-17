using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class HostileAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent navAgent;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;

    [Header("Layers")]
    [SerializeField] private LayerMask terrainLayer;
    [SerializeField] private LayerMask playerLayerMask;

    [Header("Patrol Settings")]
    [SerializeField] private float patrolRadius = 10f;
    private Vector3 currentPatrolPoint;
    private bool hasPatrolPoint;

    [Header("Combat Settings")]
    [SerializeField] private float attackCooldown = 1f;
    private bool isOnAttackCooldown;
    [SerializeField] private float forwardShotForce = 10f;
    [SerializeField] private float verticalShotForce = 5f;

    [Header("Detection Ranges")]
    [SerializeField] private float visionRange = 20f;
    [SerializeField] private float engagementRange = 10f;

    [Header("Portal Navigation")]
    [SerializeField] private float portalApproachDistance = 1f;
    
    // Portal navigation variables
    private AIPortalTraveller aiPortalTraveller;
    private Vector3? currentDestination;
    private Portal currentApproachingPortal;

    private bool isPlayerVisible;
    private bool isPlayerInRange;

    // Stuck detection
    private Vector3 lastPosition;
    private float stuckTimer;
    private const float STUCK_TIME_THRESHOLD = 2f;

    private void Awake()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
        }

        if (navAgent == null)
        {
            navAgent = GetComponent<NavMeshAgent>();
        }
        
        // Get or add AIPortalTraveller component
        aiPortalTraveller = GetComponent<AIPortalTraveller>();
        if (aiPortalTraveller == null)
        {
            aiPortalTraveller = gameObject.AddComponent<AIPortalTraveller>();
        }
    }

    private void Start()
    {
        // Subscribe to portal teleport events
        aiPortalTraveller.OnAITeleported += HandlePortalTeleport;
        
        // Configure NavMeshAgent for portal navigation
        if (navAgent != null)
        {
            navAgent.autoTraverseOffMeshLink = false;
        }
    }

    private void OnDestroy()
    {
        if (aiPortalTraveller != null)
        {
            aiPortalTraveller.OnAITeleported -= HandlePortalTeleport;
        }
    }

    private void Update()
    {
        DetectPlayer();
        CheckForStuck();
        UpdateBehaviourState();
        
        // Only check for portals when not attacking
        if (!isPlayerInRange || !isPlayerVisible)
        {
            CheckForPortalProximity();
        }
    }

    private void FixedUpdate()
    {
        HandleOffMeshLinkMovement();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, engagementRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        // Draw current destination if set
        if (currentDestination.HasValue)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(currentDestination.Value, 0.5f);
            Gizmos.DrawLine(transform.position, currentDestination.Value);
        }

        // Draw portal approach distance
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, portalApproachDistance);
    }

    // Portal Navigation Methods
    private void HandlePortalTeleport(Portal sender, Portal destination, Vector3 newPosition, Quaternion newRotation)
    {
        Debug.Log($"{gameObject.name} teleported through portal");
        
        // Clear the current approaching portal
        currentApproachingPortal = null;
        
        // Re-enable NavMeshAgent if it was disabled
        if (navAgent != null && !navAgent.enabled)
        {
            navAgent.enabled = true;
        }
    }

    private void HandleOffMeshLinkMovement()
    {
        if (navAgent.isOnOffMeshLink)
        {
            OffMeshLinkData linkData = navAgent.currentOffMeshLinkData;
            
            // Move towards the end position
            Vector3 direction = (linkData.endPos - transform.position).normalized;
            float moveDistance = navAgent.speed * Time.fixedDeltaTime;
            
            // Only move if we have a valid direction
            if (direction.magnitude > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, linkData.endPos, moveDistance);
            }

            // Complete the link when close enough
            if (Vector3.Distance(transform.position, linkData.endPos) < 0.3f)
            {
                navAgent.CompleteOffMeshLink();
                Debug.Log($"{gameObject.name} completed OffMeshLink");
            }
        }
    }

    private void CheckForPortalProximity()
    {
        // Only check for portals if we're moving and close to our destination
        if (!navAgent.hasPath || navAgent.remainingDistance > 3f) return;

        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, portalApproachDistance);
        foreach (var collider in nearbyColliders)
        {
            Portal portal = collider.GetComponent<Portal>();
            if (portal != null && portal != currentApproachingPortal)
            {
                currentApproachingPortal = portal;
                Debug.Log($"{gameObject.name} detected portal: {portal.name}");
                break;
            }
        }
    }

    private void CheckForStuck()
    {
        if (navAgent.hasPath && currentDestination.HasValue)
        {
            float distanceMoved = Vector3.Distance(transform.position, lastPosition);
            
            if (distanceMoved < 0.1f && navAgent.velocity.magnitude < 0.1f)
            {
                stuckTimer += Time.deltaTime;
                
                if (stuckTimer > STUCK_TIME_THRESHOLD)
                {
                    RecoverFromStuck();
                    stuckTimer = 0f;
                }
            }
            else
            {
                stuckTimer = 0f;
            }
        }
        
        lastPosition = transform.position;
    }

    private void RecoverFromStuck()
    {
        Debug.Log($"{gameObject.name} is stuck, attempting recovery...");
        
        // Clear current path
        navAgent.ResetPath();
        
        // Small random movement to unstick
        Vector3 randomDirection = Random.insideUnitSphere * 2f;
        randomDirection.y = 0;
        transform.position += randomDirection.normalized * 0.5f;
        
        // Try to recalculate path after a short delay
        if (currentDestination.HasValue)
        {
            StartCoroutine(RetryPathAfterDelay(0.5f));
        }
    }

    private IEnumerator RetryPathAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (currentDestination.HasValue)
        {
            Goto(currentDestination.Value);
        }
    }

    // Public method to get current destination (for AIPortalTraveller)
    public Vector3? GetCurrentDestination()
    {
        return currentDestination;
    }

    public void Goto(Vector3 destination)
    {
        currentDestination = destination;
        
        if (navAgent != null && navAgent.isOnNavMesh)
        {
            navAgent.SetDestination(destination);
        }
    }

    // AI Behaviour Methods (keep your existing logic with minor updates)
    private void DetectPlayer()
    {
        isPlayerVisible = Physics.CheckSphere(transform.position, visionRange, playerLayerMask);
        isPlayerInRange = Physics.CheckSphere(transform.position, engagementRange, playerLayerMask);
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null) return;

        Rigidbody projectileRb = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity).GetComponent<Rigidbody>();
        projectileRb.AddForce(transform.forward * forwardShotForce, ForceMode.Impulse);
        projectileRb.AddForce(transform.up * verticalShotForce, ForceMode.Impulse);

        Destroy(projectileRb.gameObject, 3f);
    }

    private void FindPatrolPoint()
    {
        float randomX = Random.Range(-patrolRadius, patrolRadius);
        float randomZ = Random.Range(-patrolRadius, patrolRadius);

        Vector3 potentialPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (NavMesh.SamplePosition(potentialPoint, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            currentPatrolPoint = hit.position;
            hasPatrolPoint = true;
        }
    }

    private IEnumerator AttackCooldownRoutine()
    {
        isOnAttackCooldown = true;
        yield return new WaitForSeconds(attackCooldown);
        isOnAttackCooldown = false;
    }

    private void PerformPatrol()
    {
        if (!hasPatrolPoint)
            FindPatrolPoint();

        if (hasPatrolPoint)
            Goto(currentPatrolPoint);

        if (hasPatrolPoint && Vector3.Distance(transform.position, currentPatrolPoint) < 1f)
            hasPatrolPoint = false;
    }

    private void PerformChase()
    {
        if (playerTransform != null)
        {
            Goto(playerTransform.position);
        }
    }

    private void PerformAttack()
    {
        // Stop movement when attacking
        if (navAgent.hasPath)
            navAgent.ResetPath();

        if (playerTransform != null)
        {
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            if (directionToPlayer.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }

        if (!isOnAttackCooldown)
        {
            FireProjectile();
            StartCoroutine(AttackCooldownRoutine());
        }
    }

    private void UpdateBehaviourState()
    {
        if (!isPlayerVisible && !isPlayerInRange)
        {
            PerformPatrol();
        }
        else if (isPlayerVisible && !isPlayerInRange)
        {
            PerformChase();
        }
        else if (isPlayerVisible && isPlayerInRange)
        {
            PerformAttack();
        }
    }
}
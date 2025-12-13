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
    private Animator animator;

    [Header("Layers")]
    [SerializeField] private LayerMask terrainLayer;
    [SerializeField] private LayerMask playerLayerMask;

    [Header("Patrol Settings")]
    [SerializeField] private float patrolRadius = 10f;
    [SerializeField] private float waitTimeAtPoint = 3f;
    private Vector3 currentPatrolPoint;
    private bool hasPatrolPoint;
    private bool isWaitingAtPoint;
    private float waitTimer;

    [Header("Combat Settings")]
    [SerializeField] private float attackCooldown = 1.5f;
    private bool isOnAttackCooldown;
    public Transform FirePoint;
    public GameObject Fire;
    public AudioClip gunshot;
    [SerializeField] private int attackDamage = 25;
    [SerializeField] private float muzzleFlashDuration = 0.05f;

    [Header("Detection Ranges")]
    [SerializeField] private float visionRange = 20f;
    [SerializeField] private float engagementRange = 10f;

    private bool isPlayerVisible;
    private bool isPlayerInRange;

    [Header("Collider Reference")]
    [SerializeField] private Collider aiCollider;

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        animator.SetBool("isPatroling", true);

        if (Fire != null) Fire.SetActive(false);

        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.Find("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
        }

        if (navAgent == null)
        {
            navAgent = GetComponent<NavMeshAgent>();
        }
        
        if (aiCollider == null)
        {
            aiCollider = GetComponent<Collider>();
        }
        
        if (aiCollider != null)
        {
            aiCollider.isTrigger = false;
        }
    }

    private void Update()
    {
        DetectPlayer();
        UpdateBehaviourState();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, engagementRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }

    private void DetectPlayer()
    {
        isPlayerVisible = Physics.CheckSphere(transform.position, visionRange, playerLayerMask);
        isPlayerInRange = Physics.CheckSphere(transform.position, engagementRange, playerLayerMask);
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null) return;
        
        RaycastHit hit; 
        float maxRange = 100f;

        if(Physics.Raycast(firePoint.position, transform.forward, out hit, maxRange))
        {
            StartCoroutine(MuzzleFlashCoroutine());
            AudioSource.PlayClipAtPoint(gunshot, transform.position);
            Debug.DrawRay(firePoint.position, transform.forward * hit.distance, Color.yellow, 0.5f);
            
            HealthSystem targetHealth = hit.collider.GetComponent<HealthSystem>();

            if (targetHealth != null)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    targetHealth.TakeDamage(attackDamage);
                }
            }
            HUDNotification.Instance.displayMessage("You feel a sharp pain in your abdomen.");
        }
        else
        {
            Debug.DrawRay(firePoint.position, transform.forward * maxRange, Color.green, 0.5f);
            StartCoroutine(MuzzleFlashCoroutine());
            AudioSource.PlayClipAtPoint(gunshot, transform.position);
        }
    }

    private IEnumerator MuzzleFlashCoroutine()
    {
        if (Fire != null) Fire.SetActive(true);
        yield return new WaitForSeconds(muzzleFlashDuration);
        if (Fire != null) Fire.SetActive(false);
    }

    private void FindPatrolPoint()
    {
        if (isWaitingAtPoint) return; 
        
        float randomX = Random.Range(-patrolRadius, patrolRadius);
        float randomZ = Random.Range(-patrolRadius, patrolRadius);

        Vector3 potentialPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (NavMesh.SamplePosition(potentialPoint, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            currentPatrolPoint = hit.position;
            hasPatrolPoint = true;
            isWaitingAtPoint = false; 
        }
        else
        {
            if (Physics.Raycast(potentialPoint, -transform.up, 2f, terrainLayer))
            {
                currentPatrolPoint = potentialPoint;
                hasPatrolPoint = true;
                isWaitingAtPoint = false;
            }
        }
    }

    private IEnumerator AttackCooldownRoutine()
    {
        yield return new WaitForSeconds(attackCooldown);
    }

    private IEnumerator ResetAttackAnimation()
    {
        yield return new WaitForSeconds(0.4f);
    }

    private void PerformPatrol()
    {
        if (isWaitingAtPoint)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint)
            {
                isWaitingAtPoint = false;
                waitTimer = 0f;
                hasPatrolPoint = false;
                navAgent.isStopped = false;
                FindPatrolPoint();
            }
            return;
        }

        if (!hasPatrolPoint)
        {
            FindPatrolPoint();
            return;
        }

        if (hasPatrolPoint)
        {
            navAgent.SetDestination(currentPatrolPoint);
            animator.SetBool("isPatroling", true);
            
            if (navAgent.remainingDistance <= navAgent.stoppingDistance && !navAgent.pathPending)
            {
                if (!isWaitingAtPoint)
                {
                    StartWaitingAtPoint();
                }
            }
        }
    }

    private void StartWaitingAtPoint()
    {
        animator.SetBool("isPatroling", false);
        isWaitingAtPoint = true;
        waitTimer = 0f;
        navAgent.isStopped = true; 
    }

    private void PerformChase()
    {
        if (navAgent.isStopped)
        {
            navAgent.isStopped = false;
        }

        if (playerTransform != null)
        {
            navAgent.SetDestination(playerTransform.position);
        }
    }

    private IEnumerator AttackSequence()
    {
        if (isOnAttackCooldown)
            yield break;

        isOnAttackCooldown = true;

        float animationFireTime = 0.4f;
        yield return new WaitForSeconds(animationFireTime);
        
        FireProjectile();

        float remainingCooldown = attackCooldown - animationFireTime;
        if (remainingCooldown > 0)
        {
            yield return new WaitForSeconds(remainingCooldown);
        }
        
        isOnAttackCooldown = false;
    }   

    private void PerformAttack()
    {
        navAgent.SetDestination(transform.position);

        if (playerTransform != null)
        {
            Vector3 targetPosition = playerTransform.position;
            targetPosition.y = transform.position.y; 
            transform.LookAt(targetPosition);
        }

        if (!isOnAttackCooldown)
        {
            StartCoroutine(AttackSequence()); 
        }
    }

    private void UpdateBehaviourState()
    {
        DetectPlayer(); 

        if (isPlayerVisible && isPlayerInRange)
        {
            animator.SetBool("isChasing", false);
            animator.SetBool("isPatroling", false); 
            
            PerformAttack(); 
        }
        else if (isPlayerVisible && !isPlayerInRange)
        {
            animator.SetBool("isChasing", true);
            animator.SetBool("isPatroling", false);
            
            PerformChase();
        }
        else {
            animator.SetBool("isChasing", false);
            
            PerformPatrol(); 
        }
    }
}
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

        if(Physics.Raycast(firePoint.position, transform.TransformDirection(Vector3.forward), out hit, 100))
        {
            Debug.DrawRay(firePoint.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);

            GameObject bullet = Instantiate(Fire, firePoint.position, Quaternion.identity);

            Destroy(bullet, 0.1f);
        }

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
            
            if (navAgent.remainingDistance <= navAgent.stoppingDistance && !navAgent.pathPending)
            {
                if (!isWaitingAtPoint)
                {
                    StartWaitingAtPoint();
                }
                animator.SetBool("isPatroling", true);
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
        if (playerTransform != null)
        {
            navAgent.SetDestination(playerTransform.position);
        }
    }

    private IEnumerator AttackSequence()
    {
        if (isOnAttackCooldown)
            yield break;

        animator.SetTrigger("attackTrigger");
        isOnAttackCooldown = true;

        yield return new WaitForSeconds(0.1f);
        FireProjectile();

        yield return new WaitForSeconds(attackCooldown);
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
            FireProjectile();
            StartCoroutine(AttackCooldownRoutine());
            StartCoroutine(ResetAttackAnimation());
        }
    }

    private void UpdateBehaviourState()
    {
        if (!isPlayerVisible && !isPlayerInRange)
        {
            animator.SetBool("isChasing", false);
            PerformPatrol();
        }
        else if (isPlayerVisible && !isPlayerInRange)
        {
            animator.SetBool("isChasing", true);
            PerformChase();
        }
        else if (isPlayerVisible && isPlayerInRange)
        {
            if (!isOnAttackCooldown)
                StartCoroutine(AttackSequence());
        }
    }
}
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class SimpleZombieAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform[] patrolPoints;

    [Header("Detection")]
    public float darkDetectionRange = 6f;
    public float brightDetectionRange = 14f;
    public float loseInterestRangeMultiplier = 1.35f;
    public float eyeHeight = 1.6f;
    public float playerEyeHeight = 1.4f;
    public float memoryDuration = 1.5f;

    [Header("Brightness Behavior")]
    [Range(0f, 1f)]
    public float patrolBrightnessThreshold = 0.75f;

    [Header("Movement")]
    public float darkSpeed = 1.2f;
    public float brightSpeed = 2.6f;
    public float stoppingDistance = 1.5f;
    public float patrolPointReachDistance = 1.2f;

    private NavMeshAgent agent;
    private Animator animator;

    private bool isChasing;
    private int currentPatrolIndex;
    private float lastTimeSawPlayer = -999f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        agent.stoppingDistance = stoppingDistance;
        agent.isStopped = true;
        animator.SetFloat("Speed", 0f);
    }

    void Update()
    {
        float brightness = 0f;

        if (BrightnessDifficultyManager.Instance != null)
        {
            brightness = BrightnessDifficultyManager.Instance.brightnessValue;
        }

        float currentDetectionRange = Mathf.Lerp(darkDetectionRange, brightDetectionRange, brightness);
        float currentLoseInterestRange = currentDetectionRange * loseInterestRangeMultiplier;
        float currentSpeed = Mathf.Lerp(darkSpeed, brightSpeed, brightness);

        agent.speed = currentSpeed;

        if (player == null)
        {
            HandleNoPlayer(brightness, currentSpeed);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool canSeePlayer = IsPlayerVisible(currentDetectionRange);

        if (canSeePlayer)
        {
            lastTimeSawPlayer = Time.time;
        }

        if (!isChasing)
        {
            if (canSeePlayer)
            {
                isChasing = true;
            }
        }
        else
        {
            bool tooFar = distanceToPlayer > currentLoseInterestRange;
            bool forgotPlayer = Time.time > lastTimeSawPlayer + memoryDuration;

            if (tooFar || forgotPlayer)
            {
                isChasing = false;
            }
        }

        if (isChasing)
        {
            HandleChase(currentSpeed, distanceToPlayer);
            return;
        }

        HandleIdleOrPatrol(brightness, currentSpeed);
    }

    bool IsPlayerVisible(float detectionRange)
    {
        if (player == null)
        {
            return false;
        }

        Vector3 origin = transform.position + Vector3.up * eyeHeight;
        Vector3 target = player.position + Vector3.up * playerEyeHeight;
        Vector3 direction = target - origin;
        float distance = direction.magnitude;

        if (distance > detectionRange)
        {
            return false;
        }

        if (distance <= 0.01f)
        {
            return true;
        }

        direction.Normalize();

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform == player || hit.transform.IsChildOf(player))
            {
                return true;
            }

            return false;
        }

        return false;
    }

    void HandleNoPlayer(float brightness, float currentSpeed)
    {
        if (brightness >= patrolBrightnessThreshold && patrolPoints != null && patrolPoints.Length > 0)
        {
            HandlePatrol(currentSpeed);
        }
        else
        {
            agent.isStopped = true;
            agent.ResetPath();
            animator.SetFloat("Speed", 0f);
        }
    }

    void HandleChase(float currentSpeed, float distanceToPlayer)
    {
        agent.isStopped = false;
        agent.stoppingDistance = stoppingDistance;
        agent.SetDestination(player.position);

        float normalizedSpeed = 0f;

        if (brightSpeed > 0f)
        {
            normalizedSpeed = currentSpeed / brightSpeed;
        }

        if (distanceToPlayer <= stoppingDistance + 0.2f)
        {
            normalizedSpeed = 0f;
        }

        animator.SetFloat("Speed", normalizedSpeed);
    }

    void HandleIdleOrPatrol(float brightness, float currentSpeed)
    {
        if (brightness >= patrolBrightnessThreshold && patrolPoints != null && patrolPoints.Length > 0)
        {
            HandlePatrol(currentSpeed);
        }
        else
        {
            agent.isStopped = true;
            agent.ResetPath();
            animator.SetFloat("Speed", 0f);
        }
    }

    void HandlePatrol(float currentSpeed)
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            agent.isStopped = true;
            agent.ResetPath();
            animator.SetFloat("Speed", 0f);
            return;
        }

        Transform patrolTarget = patrolPoints[currentPatrolIndex];

        if (patrolTarget == null)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            animator.SetFloat("Speed", 0f);
            return;
        }

        agent.isStopped = false;
        agent.stoppingDistance = 0f;
        agent.SetDestination(patrolTarget.position);

        float distanceToPatrolPoint = Vector3.Distance(transform.position, patrolTarget.position);

        if (distanceToPatrolPoint <= patrolPointReachDistance)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }

        float normalizedSpeed = 0f;

        if (brightSpeed > 0f)
        {
            normalizedSpeed = currentSpeed / brightSpeed;
        }

        animator.SetFloat("Speed", normalizedSpeed);
    }

    void OnDrawGizmosSelected()
    {
        float previewBrightness = 0f;

        if (BrightnessDifficultyManager.Instance != null)
        {
            previewBrightness = BrightnessDifficultyManager.Instance.brightnessValue;
        }

        float previewDetectionRange = Mathf.Lerp(darkDetectionRange, brightDetectionRange, previewBrightness);
        float previewLoseInterestRange = previewDetectionRange * loseInterestRangeMultiplier;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, previewDetectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, previewLoseInterestRange);

        Vector3 origin = transform.position + Vector3.up * eyeHeight;
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(origin, 0.08f);

        if (player != null)
        {
            Vector3 target = player.position + Vector3.up * playerEyeHeight;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(origin, target);
            Gizmos.DrawSphere(target, 0.08f);
        }

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            Gizmos.color = Color.cyan;

            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] != null)
                {
                    Gizmos.DrawSphere(patrolPoints[i].position, 0.2f);

                    Transform nextPoint = patrolPoints[(i + 1) % patrolPoints.Length];
                    if (nextPoint != null)
                    {
                        Gizmos.DrawLine(patrolPoints[i].position, nextPoint.position);
                    }
                }
            }
        }
    }
}
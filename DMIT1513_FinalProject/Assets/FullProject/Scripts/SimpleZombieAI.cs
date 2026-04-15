using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class SimpleZombieAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Detection")]
    public float detectionRange = 10f;
    public float loseInterestRange = 14f;

    [Header("Movement")]
    public float darkSpeed = 1.2f;
    public float brightSpeed = 3.2f;
    public float stoppingDistance = 1.5f;

    [Header("Animation")]
    public float walkThreshold = 0.15f;
    public float runThreshold = 0.65f;

    private NavMeshAgent agent;
    private Animator animator;
    private bool isChasing;

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
        if (player == null)
        {
            StopChasing();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (!isChasing)
        {
            if (distanceToPlayer <= detectionRange)
            {
                isChasing = true;
            }
        }
        else
        {
            if (distanceToPlayer > loseInterestRange)
            {
                StopChasing();
                return;
            }
        }

        if (!isChasing)
        {
            agent.isStopped = true;
            animator.SetFloat("Speed", 0f);
            return;
        }

        float brightness = 0f;

        if (BrightnessDifficultyManager.Instance != null)
        {
            brightness = BrightnessDifficultyManager.Instance.brightnessValue;
        }

        float currentSpeed = Mathf.Lerp(darkSpeed, brightSpeed, brightness);

        agent.speed = currentSpeed;
        agent.isStopped = false;
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

    void StopChasing()
    {
        isChasing = false;
        agent.isStopped = true;
        animator.SetFloat("Speed", 0f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseInterestRange);
    }
}
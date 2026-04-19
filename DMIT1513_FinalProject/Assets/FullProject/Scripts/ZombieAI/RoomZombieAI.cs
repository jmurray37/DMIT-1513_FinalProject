using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class RoomZombieAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform roomDoor;
    public Transform roomCenter;

    [Header("Brightness Behavior")]
    [Range(0f, 1f)]
    public float doorRushBrightnessThreshold = 0.75f;

    [Header("Door Trigger")]
    public float playerNearDoorDistance = 4f;
    public float doorStoppingDistance = 1.2f;

    [Header("Room Wandering")]
    public Vector3 roomSize = new Vector3(8f, 0f, 8f);
    public float minWanderWaitTime = 1.5f;
    public float maxWanderWaitTime = 4f;
    public float wanderPointReachDistance = 1.2f;
    public float navMeshSampleDistance = 3f;
    public int maxWanderPointTries = 10;

    [Header("Movement")]
    public float wanderSpeed = 1.2f;
    public float rushSpeed = 3.4f;

    [Header("Animation")]
    public string speedParameterName = "Speed";
    public float wanderAnimationSpeed = 0.45f;
    public float rushAnimationSpeed = 1f;

    [Header("Audio")]
    public AudioClip[] wanderSounds;
    public float minSoundDelay = 4f;
    public float maxSoundDelay = 10f;
    public float wanderVolume = 0.35f;
    public float rushVolume = 0.55f;
    public float wanderPitchMin = 0.9f;
    public float wanderPitchMax = 1.05f;
    public float rushPitchMin = 1f;
    public float rushPitchMax = 1.15f;

    private NavMeshAgent agent;
    private Animator animator;
    private AudioSource audioSource;

    private Vector3 currentWanderTarget;
    private bool hasWanderTarget;
    private float nextWanderTime;
    private bool isRushingDoor;
    private float nextSoundTime;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (player == null && Camera.main != null)
        {
            player = Camera.main.transform.root;
        }

        agent.isStopped = false;
        agent.stoppingDistance = 0f;

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = 1f;
        }

        PickNextWanderTime();
        PickNextSoundTime();
        TryPickNewWanderPoint(true);
    }

    void Update()
    {
        float brightness = 0f;

        if (BrightnessDifficultyManager.Instance != null)
        {
            brightness = BrightnessDifficultyManager.Instance.brightnessValue;
        }

        bool shouldRushDoor = ShouldRushDoor(brightness);

        if (shouldRushDoor)
        {
            if (!isRushingDoor)
            {
                isRushingDoor = true;
                hasWanderTarget = false;
            }

            HandleDoorRush();
            HandleRandomSounds();
            return;
        }

        if (isRushingDoor)
        {
            isRushingDoor = false;
            agent.ResetPath();
            PickNextWanderTime();
        }

        HandleWandering();
        HandleRandomSounds();
    }

    bool ShouldRushDoor(float brightness)
    {
        if (brightness < doorRushBrightnessThreshold)
        {
            return false;
        }

        if (player == null || roomDoor == null)
        {
            return false;
        }

        float distanceFromPlayerToDoor = Vector3.Distance(player.position, roomDoor.position);

        if (distanceFromPlayerToDoor > playerNearDoorDistance)
        {
            return false;
        }

        return true;
    }

    void HandleDoorRush()
    {
        if (roomDoor == null)
        {
            agent.isStopped = true;
            SetAnimatorSpeed(0f);
            return;
        }

        agent.isStopped = false;
        agent.speed = rushSpeed;
        agent.stoppingDistance = doorStoppingDistance;
        agent.SetDestination(roomDoor.position);

        float distanceToDoor = Vector3.Distance(transform.position, roomDoor.position);

        if (distanceToDoor <= doorStoppingDistance + 0.2f)
        {
            SetAnimatorSpeed(0f);
        }
        else
        {
            SetAnimatorSpeed(rushAnimationSpeed);
        }
    }

    void HandleWandering()
    {
        agent.speed = wanderSpeed;
        agent.stoppingDistance = 0f;

        if (!hasWanderTarget)
        {
            if (Time.time >= nextWanderTime)
            {
                TryPickNewWanderPoint(false);
            }
            else
            {
                agent.isStopped = true;
                SetAnimatorSpeed(0f);
            }

            return;
        }

        agent.isStopped = false;
        agent.SetDestination(currentWanderTarget);

        float distanceToTarget = Vector3.Distance(transform.position, currentWanderTarget);

        if (distanceToTarget <= wanderPointReachDistance)
        {
            hasWanderTarget = false;
            agent.ResetPath();
            PickNextWanderTime();
            SetAnimatorSpeed(0f);
            return;
        }

        if (agent.velocity.magnitude > 0.05f)
        {
            SetAnimatorSpeed(wanderAnimationSpeed);
        }
        else
        {
            SetAnimatorSpeed(0f);
        }
    }

    void HandleRandomSounds()
    {
        if (audioSource == null)
        {
            return;
        }

        if (wanderSounds == null || wanderSounds.Length == 0)
        {
            return;
        }

        if (Time.time < nextSoundTime)
        {
            return;
        }

        if (audioSource.isPlaying)
        {
            return;
        }

        int soundIndex = Random.Range(0, wanderSounds.Length);
        AudioClip clip = wanderSounds[soundIndex];

        if (clip == null)
        {
            PickNextSoundTime();
            return;
        }

        if (isRushingDoor)
        {
            audioSource.volume = rushVolume;
            audioSource.pitch = Random.Range(rushPitchMin, rushPitchMax);
        }
        else
        {
            audioSource.volume = wanderVolume;
            audioSource.pitch = Random.Range(wanderPitchMin, wanderPitchMax);
        }

        audioSource.PlayOneShot(clip);
        PickNextSoundTime();
    }

    void TryPickNewWanderPoint(bool forceImmediateMove)
    {
        Vector3 center = transform.position;

        if (roomCenter != null)
        {
            center = roomCenter.position;
        }

        bool foundPoint = false;

        for (int i = 0; i < maxWanderPointTries; i++)
        {
            Vector3 randomPoint = GetRandomPointInRoom(center);

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, navMeshSampleDistance, NavMesh.AllAreas))
            {
                currentWanderTarget = hit.position;
                hasWanderTarget = true;
                foundPoint = true;
                break;
            }
        }

        if (!foundPoint)
        {
            hasWanderTarget = false;
            PickNextWanderTime();
            return;
        }

        if (forceImmediateMove)
        {
            nextWanderTime = Time.time;
        }
    }

    Vector3 GetRandomPointInRoom(Vector3 center)
    {
        float halfX = roomSize.x * 0.5f;
        float halfZ = roomSize.z * 0.5f;

        float randomX = Random.Range(-halfX, halfX);
        float randomZ = Random.Range(-halfZ, halfZ);

        return new Vector3(center.x + randomX, center.y, center.z + randomZ);
    }

    void PickNextWanderTime()
    {
        nextWanderTime = Time.time + Random.Range(minWanderWaitTime, maxWanderWaitTime);
    }

    void PickNextSoundTime()
    {
        nextSoundTime = Time.time + Random.Range(minSoundDelay, maxSoundDelay);
    }

    void SetAnimatorSpeed(float value)
    {
        if (animator != null && !string.IsNullOrWhiteSpace(speedParameterName))
        {
            animator.SetFloat(speedParameterName, value);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (roomCenter != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(roomCenter.position, new Vector3(roomSize.x, 0.2f, roomSize.z));
        }

        if (roomDoor != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(roomDoor.position, 0.25f);
            Gizmos.DrawWireSphere(roomDoor.position, playerNearDoorDistance);
        }

        if (hasWanderTarget)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(currentWanderTarget, 0.2f);
            Gizmos.DrawLine(transform.position, currentWanderTarget);
        }
    }
}
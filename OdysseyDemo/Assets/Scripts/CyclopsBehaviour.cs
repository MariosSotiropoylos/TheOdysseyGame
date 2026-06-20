using UnityEngine;
using UnityEngine.AI;

public class CyclopsBehaviour : MonoBehaviour
{
    [Header("PLAYER DETECTION")]
    public string playerTag = "Player";
    public Transform player;

    [Header("RANGES")]
    public float detectionRange = 12f;
    public float stopRange = 1f;

    [Header("WAIT AFTER REACHING PLAYER")]
    public float waitTimeAfterReachingPlayer = 1.5f;

    [Header("NAVMESH")]
    public NavMeshAgent agent;
    public float stoppingDistance = 1.8f;
    public float destinationUpdateRate = 0.2f;

    [Header("ROTATION")]
    public bool manuallyFacePlayer = true;
    public float rotationSpeed = 8f;

    [Header("COUNTDOWN / FALL")]
    public CyclopsTime timedFallScript;
    public bool startCountdownWhenPlayerSpotted = true;

    [Header("ANIMATOR")]
    public Animator animator;
    public string huntingBoolName = "IsHunting";

    [Header("Audio")]
    public AudioSource cyclopsAudioSource;
    public AudioClip cyclopsRoarClip;
    public AudioClip cyclopsDeathClip;

    [Header("RUNTIME STATE")]
    public bool playerInArea = false;
    public bool isWaiting = false;
    public bool isDead = false;

    private float waitTimer = 0f;
    private float nextDestinationUpdateTime = 0f;
    private bool countdownStarted = false;
    private bool mustLeaveAreaBeforeRestart = false;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (cyclopsAudioSource == null) cyclopsAudioSource = GetComponent<AudioSource>();

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObject != null) player = playerObject.transform;
        }

        if (timedFallScript == null)
            timedFallScript = GetComponent<CyclopsTime>();

        if (agent != null)
        {
            agent.stoppingDistance = stoppingDistance;
            agent.updateRotation = false;
        }

        StopAllCyclopsAudio();
        SetHunting(false);
    }

    void Update()
    {
        if (isDead)
            return;

        if (player == null || agent == null || !agent.enabled)
        {
            StopRoar();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        playerInArea = distanceToPlayer <= detectionRange;

        if (!playerInArea)
        {
            StopMoving();
            StopRoar();
            mustLeaveAreaBeforeRestart = false;
            return;
        }

        if (mustLeaveAreaBeforeRestart)
        {
            StopMoving();
            StopRoar();
            return;
        }

        if (startCountdownWhenPlayerSpotted && !countdownStarted)
        {
            countdownStarted = true;

            if (timedFallScript != null)
                timedFallScript.StartCountdown();
        }

        if (manuallyFacePlayer)
            FacePlayer();

        if (isWaiting)
        {
            HandleWaiting();
            StopRoar();
            return;
        }

        if (distanceToPlayer <= stopRange)
        {
            StartWaitingNearPlayer();
            StopRoar();
        }
        else
        {
            HuntPlayer();
            PlayRoar();
        }
    }

    void HuntPlayer()
    {
        if (agent == null || !agent.enabled)
            return;

        if (agent.isStopped)
            agent.isStopped = false;

        if (Time.time >= nextDestinationUpdateTime)
        {
            agent.SetDestination(player.position);
            nextDestinationUpdateTime = Time.time + destinationUpdateRate;
        }

        SetHunting(true);
    }

    void PlayRoar()
    {
        if (cyclopsAudioSource == null || cyclopsRoarClip == null || isDead)
            return;

        if (cyclopsAudioSource.clip != cyclopsRoarClip)
        {
            cyclopsAudioSource.Stop();
            cyclopsAudioSource.clip = cyclopsRoarClip;
        }

        cyclopsAudioSource.loop = true;

        if (!cyclopsAudioSource.isPlaying)
            cyclopsAudioSource.Play();
    }

    void StopRoar()
    {
        if (cyclopsAudioSource == null)
            return;

        if (cyclopsAudioSource.clip == cyclopsRoarClip)
        {
            cyclopsAudioSource.Stop();
            cyclopsAudioSource.loop = false;
        }
    }

    public void StopAllCyclopsAudio()
    {
        AudioSource[] sources = GetComponentsInChildren<AudioSource>(true);

        for (int i = 0; i < sources.Length; i++)
        {
            sources[i].Stop();
            sources[i].loop = false;
        }
    }

    public void PlayCyclopsDeathOnce()
    {
        StopAllCyclopsAudio();

        if (cyclopsAudioSource != null && cyclopsDeathClip != null)
        {
            cyclopsAudioSource.clip = cyclopsDeathClip;
            cyclopsAudioSource.loop = false;
            cyclopsAudioSource.Play();
        }
    }

    void StartWaitingNearPlayer()
    {
        isWaiting = true;
        waitTimer = waitTimeAfterReachingPlayer;

        StopMoving();
        StopRoar();
    }

    void HandleWaiting()
    {
        StopMoving();

        waitTimer -= Time.deltaTime;

        if (waitTimer <= 0f)
            isWaiting = false;
    }

    void StopMoving()
    {
        if (agent != null && agent.enabled)
        {
            if (!agent.isStopped)
                agent.isStopped = true;

            if (agent.hasPath)
                agent.ResetPath();
        }

        SetHunting(false);
    }

    void FacePlayer()
    {
        if (player == null)
            return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    void SetHunting(bool hunting)
    {
        if (animator != null)
            animator.SetBool(huntingBoolName, hunting);
    }

    void StopCyclops()
    {
        isWaiting = false;

        if (agent != null && agent.enabled)
        {
            StopMoving();
            agent.enabled = false;
        }

        SetHunting(false);
    }

    public void SetDead()
    {
        if (isDead)
            return;

        isDead = true;
        StopCyclops();
        PlayCyclopsDeathOnce();
    }

    public void ResetForNewAttempt()
    {
        isWaiting = false;
        isDead = false;
        playerInArea = false;
        countdownStarted = false;
        waitTimer = 0f;
        nextDestinationUpdateTime = 0f;
        mustLeaveAreaBeforeRestart = true;

        StopAllCyclopsAudio();

        if (agent != null)
        {
            if (!agent.enabled)
                agent.enabled = true;

            if (!agent.isOnNavMesh)
            {
                transform.position = originalPosition;
            }
            else
            {
                agent.isStopped = true;

                if (agent.hasPath)
                    agent.ResetPath();

                agent.Warp(originalPosition);
            }
        }
        else
        {
            transform.position = originalPosition;
        }

        transform.rotation = originalRotation;
        SetHunting(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopRange);
    }
}
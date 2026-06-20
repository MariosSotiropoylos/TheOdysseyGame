using UnityEngine;
using UnityEngine.AI;

public class SuitorBehaviour : MonoBehaviour
{
    [Header("PLAYER DETECTION")]
    public string playerTag = "Player";
    public Transform player;

    [Header("RANGES")]
    public float detectionRange = 8f;
    public float attackRange = 2.5f;

    [Header("ATTACK")]
    public float attackCooldown = 2.5f;

    [Header("NAVMESH")]
    public NavMeshAgent agent;
    public float stoppingDistance = 2.75f;
    public float destinationUpdateRate = 0.2f;

    [Header("ROTATION")]
    public bool manuallyFacePlayer = true;
    public float rotationSpeed = 6f;

    [Header("ANIMATOR")]
    public Animator animator;
    public string walkingBoolName = "IsWalking";
    public string attackTriggerName = "AttackTrigger";
    public string deathTriggerName = "DeathTrigger";

    [Header("AUDIO")]
    public SFXAudio sfxAudio;

    [Header("RUNTIME STATE")]
    public bool playerInArea = false;
    public bool isAttacking = false;
    public bool isDead = false;

    private float nextAttackTime = 0f;
    private float nextDestinationUpdateTime = 0f;

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (sfxAudio == null)
            sfxAudio = FindFirstObjectByType<SFXAudio>();

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);

            if (playerObject != null)
                player = playerObject.transform;
        }

        if (agent != null)
        {
            agent.stoppingDistance = stoppingDistance;
            agent.updateRotation = false;
        }
    }

    void Update()
    {
        if (isDead)
        {
            StopEnemy();
            return;
        }

        if (player == null || agent == null || !agent.enabled)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        playerInArea = distanceToPlayer <= detectionRange;

        if (!playerInArea)
        {
            StopWalking();
            StopAgentSafely();
            return;
        }

        if (manuallyFacePlayer)
            FacePlayer();

        if (distanceToPlayer <= attackRange)
        {
            StopWalking();
            StopAgentSafely();
            TryAttack();
        }
        else
        {
            MoveTowardPlayer();
        }
    }

    void MoveTowardPlayer()
    {
        if (isAttacking)
        {
            StopAgentSafely();
            return;
        }

        if (agent.isStopped)
            agent.isStopped = false;

        if (Time.time >= nextDestinationUpdateTime)
        {
            agent.SetDestination(player.position);
            nextDestinationUpdateTime = Time.time + destinationUpdateRate;
        }

        if (animator != null)
            animator.SetBool(walkingBoolName, true);
    }

    void TryAttack()
    {
        if (Time.time < nextAttackTime)
            return;

        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;

        if (animator != null)
        {
            animator.SetBool(walkingBoolName, false);
            animator.SetTrigger(attackTriggerName);
        }

        if (sfxAudio != null)
            sfxAudio.PlaySwordSlash();

        CancelInvoke(nameof(EndAttack));
        Invoke(nameof(EndAttack), attackCooldown * 0.5f);
    }

    void EndAttack()
    {
        isAttacking = false;
    }

    void FacePlayer()
    {
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

    void StopWalking()
    {
        if (animator != null)
            animator.SetBool(walkingBoolName, false);
    }

    void StopAgentSafely()
    {
        if (agent == null || !agent.enabled)
            return;

        if (!agent.isStopped)
            agent.isStopped = true;

        if (agent.hasPath)
            agent.ResetPath();
    }

    void StopEnemy()
    {
        CancelInvoke(nameof(EndAttack));

        isAttacking = false;

        if (agent != null && agent.enabled)
        {
            StopAgentSafely();
            agent.enabled = false;
        }

        StopWalking();
    }

    public void SetDead()
    {
        if (isDead)
            return;

        isDead = true;
        isAttacking = false;

        StopEnemy();

        if (animator != null)
            animator.SetTrigger(deathTriggerName);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(EndAttack));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
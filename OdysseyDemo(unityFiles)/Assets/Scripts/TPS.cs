using UnityEngine;
using System.Collections;

public class TPS : MonoBehaviour
{
    [Header("MOVEMENT SETTINGS")]
    public float moveSpeed = 3.5f;
    public float runSpeed = 5f;
    public float crouchSpeed = 1f;
    public float mouseSensitivity = 3f;
    public float jumpForce = 6f;

    [Header("INPUT SETTINGS")]
    public KeyCode runKey = KeyCode.R;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.C;
    public KeyCode attackKey = KeyCode.X;

    [Header("ATTACK SETTINGS")]
    public float attackDuration = 1.0f;
    public GameObject swordObject;

    [Header("AUDIO")]
    public SFXAudio sfxAudio;

    [Header("CAMERA POV")]
    public CameraPOVToggle cameraPOVToggle;

    [Header("GROUND CHECK")]
    public float groundCheckDistance = 0.05f;
    public float jumpCooldown = 0.05f;

    [Header("JUMP FORGIVENESS")]
    public float coyoteTime = 0.15f;

    [Header("CROUCH SETTINGS")]
    public float crouchingHeightMultiplier = 0.65f;
    public float crouchingRadiusMultiplier = 1.4f;

    [Header("STATES")]
    public bool IsDead = false;

    private Rigidbody rb;
    private CapsuleCollider capsule;
    private Animator animator;
    private PlayerStamina playerStamina;

    private float lastJumpTime = -999f;
    private float lastGroundedTime = -999f;

    private bool isCrouching = false;
    private bool isAttacking = false;
    private bool isKnockedBack = false;
    private bool previousIsDead = false;

    private bool isActuallyRunning = false;

    private Coroutine knockbackRoutine;

    private float standingHeight;
    private float standingRadius;
    private Vector3 standingCenter;

    private float crouchingHeight;
    private float crouchingRadius;
    private Vector3 crouchingCenter;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
        playerStamina = GetComponent<PlayerStamina>();

        if (sfxAudio == null)
        {
            sfxAudio = FindFirstObjectByType<SFXAudio>();
        }

        if (cameraPOVToggle == null && Camera.main != null)
        {
            cameraPOVToggle = Camera.main.GetComponent<CameraPOVToggle>();
        }

        if (swordObject != null)
        {
            swordObject.SetActive(false);
        }

        if (rb != null)
        {
            rb.freezeRotation = true;
        }

        if (capsule != null)
        {
            standingHeight = capsule.height;
            standingCenter = capsule.center;
            standingRadius = capsule.radius;

            crouchingHeight = standingHeight * crouchingHeightMultiplier;
            crouchingRadius = standingRadius * crouchingRadiusMultiplier;

            float heightDifference = standingHeight - crouchingHeight;
            crouchingCenter = standingCenter - new Vector3(0f, heightDifference / 2f, 0f);
        }

        previousIsDead = IsDead;
        ApplyDeathState(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (IsDead != previousIsDead)
        {
            ApplyDeathState(IsDead);
            previousIsDead = IsDead;
        }

        if (IsDead)
        {
            return;
        }

        RotateWithMouse();
        HandleCrouch();
        HandleAttack();

        bool groundedNow = IsGrounded();

        if (groundedNow)
        {
            lastGroundedTime = Time.time;
        }

        if (Input.GetKeyDown(jumpKey) && CanJump() && !isCrouching && !isAttacking && !isKnockedBack)
        {
            Jump();
            groundedNow = false;
        }

        UpdateMovementAnimations(groundedNow);
        UpdateJumpingAnimation(groundedNow);
    }

    void FixedUpdate()
    {
        if (IsDead)
        {
            if (isKnockedBack)
            {
                return;
            }

            if (rb != null)
            {
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            }

            return;
        }

        if (isKnockedBack)
        {
            return;
        }

        Move();
    }

    void RotateWithMouse()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 100f * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);
    }

    void Move()
    {
        if (rb == null)
        {
            return;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = transform.forward * v + transform.right * h;

        if (move.magnitude > 1f)
        {
            move.Normalize();
        }

        bool wantsToRun =
            Input.GetKey(runKey) &&
            move.magnitude > 0.1f &&
            !isCrouching &&
            !isAttacking &&
            !isKnockedBack;

        bool isRunning = wantsToRun;

        if (playerStamina != null)
        {
            isRunning = playerStamina.HandleRunning(wantsToRun, Time.fixedDeltaTime);
        }

        isActuallyRunning = isRunning;

        float currentSpeed;

        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }
        else if (isRunning)
        {
            currentSpeed = runSpeed;
        }
        else
        {
            currentSpeed = moveSpeed;
        }

        Vector3 targetVelocity = move * currentSpeed;

        rb.linearVelocity = new Vector3(
            targetVelocity.x,
            rb.linearVelocity.y,
            targetVelocity.z
        );
    }

    void UpdateMovementAnimations(bool groundedNow)
    {
        if (animator == null)
        {
            return;
        }

        if (isAttacking)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsSneaking", false);
            return;
        }

        if (isKnockedBack)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsSneaking", false);
            return;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 input = new Vector3(h, 0f, v);
        bool isMoving = input.magnitude > 0.1f;

        if (!groundedNow)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsSneaking", false);
            return;
        }

        bool isRunning = isMoving && isActuallyRunning && !isCrouching;
        bool isWalking = isMoving && !isRunning && !isCrouching;
        bool isSneaking = isMoving && isCrouching;

        animator.SetBool("IsWalking", isWalking);
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsCrouching", isCrouching);
        animator.SetBool("IsSneaking", isSneaking);
    }

    void UpdateJumpingAnimation(bool groundedNow)
    {
        if (animator == null)
        {
            return;
        }

        animator.SetBool("IsGrounded", groundedNow);
    }

    void HandleCrouch()
    {
        if (isAttacking || isKnockedBack)
        {
            return;
        }

        if (Input.GetKeyDown(crouchKey))
        {
            if (isCrouching)
            {
                StandUp();
            }
            else
            {
                Crouch();
            }
        }
    }

    void HandleAttack()
    {
        if (Input.GetKeyDown(attackKey) && !isAttacking && !isCrouching && !isKnockedBack)
        {
            if (cameraPOVToggle != null && cameraPOVToggle.isFirstPerson)
            {
                return;
            }

            if (animator != null)
            {
                if (animator.GetBool("IsCrouching") || animator.GetBool("IsSneaking"))
                {
                    return;
                }
            }

            Attack();
        }
    }

    void Attack()
    {
        if (animator == null)
        {
            return;
        }

        isAttacking = true;
        isActuallyRunning = false;

        if (swordObject != null)
        {
            swordObject.SetActive(true);
        }

        animator.SetBool("IsWalking", false);
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsSneaking", false);

        animator.SetTrigger("AttackTrigger");

        if (sfxAudio != null)
        {
            sfxAudio.PlaySwordSlash();
        }

        CancelInvoke(nameof(EndAttack));
        Invoke(nameof(EndAttack), attackDuration);
    }

    void EndAttack()
    {
        isAttacking = false;

        if (swordObject != null)
        {
            swordObject.SetActive(false);
        }
    }

    public void ApplyHorizontalKnockback(Vector3 direction, float force, float duration)
    {
        if (rb == null || IsDead)
        {
            return;
        }

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
        {
            direction = -transform.forward;
        }

        direction.Normalize();

        if (knockbackRoutine != null)
        {
            StopCoroutine(knockbackRoutine);
        }

        knockbackRoutine = StartCoroutine(HorizontalKnockbackRoutine(direction, force, duration));
    }

    private IEnumerator HorizontalKnockbackRoutine(Vector3 direction, float force, float duration)
    {
        isKnockedBack = true;
        isActuallyRunning = false;

        rb.linearVelocity = new Vector3(
            direction.x * force,
            rb.linearVelocity.y,
            direction.z * force
        );

        yield return new WaitForSeconds(duration);

        if (rb != null)
        {
            rb.linearVelocity = new Vector3(
                0f,
                rb.linearVelocity.y,
                0f
            );
        }

        isKnockedBack = false;
        knockbackRoutine = null;
    }

    void Crouch()
    {
        isCrouching = true;
        isActuallyRunning = false;
		
		if (sfxAudio != null)
		{
			sfxAudio.PlayCrouch();
		}

        if (capsule != null)
        {
            capsule.height = crouchingHeight;
            capsule.center = crouchingCenter;
            capsule.radius = crouchingRadius;
        }

        if (animator != null)
        {
            animator.SetBool("IsCrouching", true);
            animator.SetBool("IsSneaking", false);
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
        }
    }

    void StandUp()
    {
        if (!CanStandUp())
        {
            return;
        }

        isCrouching = false;

        if (capsule != null)
        {
            capsule.height = standingHeight;
            capsule.center = standingCenter;
            capsule.radius = standingRadius;
        }

        if (animator != null)
        {
            animator.SetBool("IsCrouching", false);
            animator.SetBool("IsSneaking", false);
        }
    }

    bool CanStandUp()
    {
        if (capsule == null)
        {
            return true;
        }

        float radius = capsule.radius * 0.95f;

        Vector3 currentWorldCenter = transform.TransformPoint(capsule.center);

        float feetY = currentWorldCenter.y - (capsule.height / 2f);
        float currentTopY = feetY + capsule.height;
        float standingTopY = feetY + standingHeight;

        if (standingTopY <= currentTopY + 0.05f)
        {
            return true;
        }

        Vector3 bottom = new Vector3(
            currentWorldCenter.x,
            currentTopY + radius,
            currentWorldCenter.z
        );

        Vector3 top = new Vector3(
            currentWorldCenter.x,
            standingTopY - radius,
            currentWorldCenter.z
        );

        Collider[] hits = Physics.OverlapCapsule(
            bottom,
            top,
            radius,
            ~0,
            QueryTriggerInteraction.Ignore
        );

        foreach (Collider hit in hits)
        {
            if (hit == null)
            {
                continue;
            }

            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                continue;
            }
            return false;
        }

        return true;
    }

    void Jump()
    {
        if (rb == null)
        {
            return;
        }

        lastJumpTime = Time.time;
        lastGroundedTime = -999f;
        isActuallyRunning = false;

        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z
        );

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        if (sfxAudio != null)
        {
            sfxAudio.PlayJump();
        }

        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsSneaking", false);

            animator.SetBool("IsGrounded", false);
            animator.SetTrigger("JumpTrigger");
        }
    }

    void ApplyDeathState(bool playDeathAnimation)
    {
        if (animator != null)
        {
            animator.SetBool("IsDead", IsDead);

            if (IsDead)
            {
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsRunning", false);
                animator.SetBool("IsCrouching", false);
                animator.SetBool("IsSneaking", false);

                if (playDeathAnimation)
                {
                    animator.SetTrigger("DeathTrigger");
                }
            }
        }

        if (IsDead)
        {
            isAttacking = false;
            isActuallyRunning = false;

            CancelInvoke(nameof(EndAttack));

            if (swordObject != null)
            {
                swordObject.SetActive(false);
            }

            if (rb != null)
            {
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            }

            if (isCrouching)
            {
                isCrouching = false;

                if (capsule != null)
                {
                    capsule.height = standingHeight;
                    capsule.center = standingCenter;
                    capsule.radius = standingRadius;
                }
            }
        }
    }

    bool CanJump()
    {
        bool recentlyGrounded = Time.time <= lastGroundedTime + coyoteTime;
        bool notJustJumped = Time.time >= lastJumpTime + jumpCooldown;

        return recentlyGrounded && notJustJumped;
    }

    bool IsGrounded()
    {
        if (capsule == null || rb == null)
        {
            return false;
        }

        if (Time.time < lastJumpTime + jumpCooldown)
        {
            return false;
        }

        if (rb.linearVelocity.y > 0.05f)
        {
            return false;
        }

        Vector3 rayOrigin = GetFeetPosition() + Vector3.up * 0.02f;

        return Physics.Raycast(
            rayOrigin,
            Vector3.down,
            groundCheckDistance + 0.02f
        );
    }

    Vector3 GetFeetPosition()
    {
        Bounds bounds = capsule.bounds;

        return new Vector3(
            bounds.center.x,
            bounds.min.y,
            bounds.center.z
        );
    }

    void OnDrawGizmosSelected()
    {
        CapsuleCollider c = GetComponent<CapsuleCollider>();

        if (c == null)
        {
            return;
        }

        Bounds bounds = c.bounds;

        Vector3 feetPosition = new Vector3(
            bounds.center.x,
            bounds.min.y,
            bounds.center.z
        );

        Vector3 rayOrigin = feetPosition + Vector3.up * 0.02f;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            rayOrigin,
            rayOrigin + Vector3.down * (groundCheckDistance + 0.02f)
        );
    }

    public void ForceStandAfterRespawn()
    {
        isCrouching = false;
        isAttacking = false;
        isKnockedBack = false;
        isActuallyRunning = false;

        CancelInvoke(nameof(EndAttack));

        if (swordObject != null)
        {
            swordObject.SetActive(false);
        }

        if (knockbackRoutine != null)
        {
            StopCoroutine(knockbackRoutine);
            knockbackRoutine = null;
        }

        if (capsule != null)
        {
            capsule.height = standingHeight;
            capsule.center = standingCenter;
            capsule.radius = standingRadius;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (animator != null)
        {
            animator.ResetTrigger("DeathTrigger");
            animator.ResetTrigger("JumpTrigger");
            animator.ResetTrigger("AttackTrigger");

            animator.SetBool("IsDead", false);
            animator.SetBool("IsCrouching", false);
            animator.SetBool("IsSneaking", false);
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsGrounded", true);

            animator.Play("Idle", 0, 0f);
        }
    }
}
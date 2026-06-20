using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    [Header("PATROL POINTS")]
    public Transform locationA;
    public Transform locationB;
    public Transform locationC;
    public Transform locationD;

    [Header("MOVEMENT SETTINGS")]
    public float moveSpeed = 2.5f;
    public float rotationSpeed = 8f;
    public float pointReachDistance = 0.2f;

    [Header("ANIMATOR")]
    public Animator animator;
    public string walkingBoolName = "IsWalking";

    [Header("RUNTIME INFO")]
    public int currentPointIndex = 0;
    public bool isPatrolling = true;

    private Transform[] patrolPoints;

    void Start()
    {
        patrolPoints = new Transform[]
        {
            locationA,
            locationB,
            locationC,
            locationD
        };

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        SetWalking(isPatrolling);
    }

    void Update()
    {
        if (!isPatrolling)
        {
            SetWalking(false);
            return;
        }

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            SetWalking(false);
            return;
        }

        Transform targetPoint = patrolPoints[currentPointIndex];

        if (targetPoint == null)
        {
            GoToNextPoint();
            return;
        }

        MoveTowardPoint(targetPoint);
    }

    void MoveTowardPoint(Transform targetPoint)
    {
        Vector3 direction = targetPoint.position - transform.position;
        direction.y = 0f;

        if (direction.magnitude <= pointReachDistance)
        {
            GoToNextPoint();
            return;
        }

        direction.Normalize();

        transform.position += direction * moveSpeed * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        SetWalking(true);
    }

    void GoToNextPoint()
    {
        currentPointIndex++;

        if (currentPointIndex >= patrolPoints.Length)
        {
            currentPointIndex = 0;
        }
    }

    void SetWalking(bool walking)
    {
        if (animator != null)
        {
            animator.SetBool(walkingBoolName, walking);
        }
    }

    public void StopPatrol()
    {
        isPatrolling = false;
        SetWalking(false);
    }

    public void StartPatrol()
    {
        isPatrolling = true;
        SetWalking(true);
    }
}
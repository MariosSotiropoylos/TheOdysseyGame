using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("POINTS")]
    public Transform pointA;
    public Transform pointB;

    [Header("MOVEMENT")]
    public float moveSpeed = 2f;
    public float reachDistance = 0.05f;

    [Header("PLAYER DETECTION")]
    public string playerTag = "Player";

    private Transform targetPoint;
    private Rigidbody rb;

    void Start()
    {
        targetPoint = pointB;

        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    void FixedUpdate()
    {
        if (pointA == null || pointB == null || targetPoint == null)
        {
            return;
        }

        MovePlatform();
    }

    void MovePlatform()
    {
        Vector3 newPosition = Vector3.MoveTowards(
            transform.position,
            targetPoint.position,
            moveSpeed * Time.fixedDeltaTime
        );

        if (rb != null)
        {
            rb.MovePosition(newPosition);
        }
        else
        {
            transform.position = newPosition;
        }

        float distance = Vector3.Distance(transform.position, targetPoint.position);

        if (distance <= reachDistance)
        {
            if (targetPoint == pointA)
            {
                targetPoint = pointB;
            }
            else
            {
                targetPoint = pointA;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(playerTag))
        {
            return;
        }

        collision.transform.SetParent(transform, true);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.CompareTag(playerTag))
        {
            return;
        }

        collision.transform.SetParent(null, true);
    }
}
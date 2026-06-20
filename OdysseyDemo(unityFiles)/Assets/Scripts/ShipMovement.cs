using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShipMovement : MonoBehaviour
{
    [Header("CAMERA REFERENCE")]
    public Transform cameraTransform;

    [Header("MOVEMENT SETTINGS")]
    public float moveSpeed = 5f;
    public float turnSpeed = 40f;

    [Header("INPUT")]
    public string verticalAxis = "Vertical";
    public string horizontalAxis = "Horizontal";

    private Rigidbody rb;
    private float moveInput;
    private float turnInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Keep ship upright, but allow Y rotation.
        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ |
            RigidbodyConstraints.FreezePositionY;
    }

    void Start()
    {
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw(verticalAxis);
        turnInput = Input.GetAxisRaw(horizontalAxis);
    }

    void FixedUpdate()
    {
        RotateShip();
        MoveShipCameraForward();
    }

    void RotateShip()
    {
        if (Mathf.Abs(turnInput) < 0.1f)
        {
            return;
        }

        float turnAmount = turnInput * turnSpeed * Time.fixedDeltaTime;

        rb.MoveRotation(
            rb.rotation * Quaternion.Euler(0f, turnAmount, 0f)
        );
    }

    void MoveShipCameraForward()
    {
        if (cameraTransform == null)
        {
            return;
        }

        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0f;

        if (cameraForward.sqrMagnitude < 0.001f)
        {
            return;
        }

        cameraForward.Normalize();

        Vector3 moveDirection = cameraForward * moveInput;

        rb.linearVelocity = new Vector3(
            moveDirection.x * moveSpeed,
            rb.linearVelocity.y,
            moveDirection.z * moveSpeed
        );
    }
}
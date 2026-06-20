using UnityEngine;

public class CameraPOVToggle : MonoBehaviour
{
    [Header("INPUT TO CHANGE CAMERA VIEW")]
    public KeyCode toggleKey = KeyCode.F;

    [Header("REFERENCES")]
    public Animator playerAnimator;

    [Header("CAMERA POSITIONS")]
    public Transform thirdPersonView;

    [Header("FIRST PERSON POSITIONS")]
    public Transform firstPersonNormalView;
    public Transform firstPersonCrouchView;
    public Transform firstPersonJumpView;

    [Header("FIRST PERSON HIDDEN MESHES")]
    public Renderer[] hideInFirstPerson;

    [Header("FIRST PERSON LOOK SETTINGS")]
    public float verticalMouseSensitivity = 3f;
    public float minVerticalAngle = -60f;
    public float maxVerticalAngle = 60f;
    public bool invertVerticalLook = false;

    [Header("TRANSITION SETTINGS")]
    public bool smoothTransition = false;
    public float transitionSpeed = 15f;

    [Header("STATE")]
    public bool isFirstPerson = false;

    private Transform targetView;
    private float verticalLookAngle = 0f;

    void Start()
    {
        if (playerAnimator == null)
        {
            playerAnimator = GetComponentInParent<Animator>();
        }

        targetView = thirdPersonView;

        SetFirstPersonHiddenMeshes(false);

        if (targetView != null)
        {
            transform.position = targetView.position;
            transform.rotation = targetView.rotation;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            TogglePOV();
        }

        ChooseCorrectView();
        HandleFirstPersonVerticalLook();
        MoveCameraToTarget();
    }

    void TogglePOV()
    {
        isFirstPerson = !isFirstPerson;
        verticalLookAngle = 0f;

        SetFirstPersonHiddenMeshes(isFirstPerson);

        ChooseCorrectView();
    }

    void ChooseCorrectView()
    {
        if (!isFirstPerson)
        {
            targetView = thirdPersonView;
            return;
        }

        targetView = firstPersonNormalView;

        if (playerAnimator == null)
        {
            return;
        }

        bool isCrouching = playerAnimator.GetBool("IsCrouching");
        bool isSneaking = playerAnimator.GetBool("IsSneaking");
        bool isGrounded = playerAnimator.GetBool("IsGrounded");


		if (!isGrounded && firstPersonJumpView != null)
        {
            targetView = firstPersonJumpView;
        }
        else if ((isCrouching || isSneaking) && firstPersonCrouchView != null)
        {
            targetView = firstPersonCrouchView;
        }
        else
        {
            targetView = firstPersonNormalView;
        }
    }

    void SetFirstPersonHiddenMeshes(bool hide)
    {
        if (hideInFirstPerson == null)
        {
            return;
        }

        for (int i = 0; i < hideInFirstPerson.Length; i++)
        {
            if (hideInFirstPerson[i] != null)
            {
                hideInFirstPerson[i].enabled = !hide;
            }
        }
    }

    void HandleFirstPersonVerticalLook()
    {
        if (!isFirstPerson)
        {
            return;
        }

        float mouseY = Input.GetAxis("Mouse Y") * verticalMouseSensitivity * 100f * Time.deltaTime;

        if (invertVerticalLook)
        {
            verticalLookAngle += mouseY;
        }
        else
        {
            verticalLookAngle -= mouseY;
        }

        verticalLookAngle = Mathf.Clamp(
            verticalLookAngle,
            minVerticalAngle,
            maxVerticalAngle
        );
    }

    void MoveCameraToTarget()
    {
        if (targetView == null)
        {
            return;
        }

        Quaternion targetRotation = targetView.rotation;

        if (isFirstPerson)
        {
            targetRotation = targetView.rotation * Quaternion.Euler(verticalLookAngle, 0f, 0f);
        }

        if (smoothTransition)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                targetView.position,
                transitionSpeed * Time.deltaTime
            );

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                transitionSpeed * Time.deltaTime
            );
        }
        else
        {
            transform.position = targetView.position;
            transform.rotation = targetRotation;
        }
    }
	
	public void ForceThirdPerson()
	{
		isFirstPerson = false;
		verticalLookAngle = 0f;

		targetView = thirdPersonView;

		SetFirstPersonHiddenMeshes(false);

		if (targetView != null)
		{
			transform.position = targetView.position;
			transform.rotation = targetView.rotation;
		}
	}
}
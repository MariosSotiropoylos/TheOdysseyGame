using UnityEngine;

public class ItemRotation : MonoBehaviour
{
    [Header("ROTATION SETTINGS")]
    public float rotationSpeed = 80f;

    [Header("ROTATION AXIS")]
    public Vector3 rotationAxis = Vector3.up;

    void Update()
    {
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }
}
using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    private Vector3 moveDirection;
    private float moveSpeed;
    private float lifeTime;
    private bool launched;

    public void Launch(Vector3 direction, float speed, float destroyAfterSeconds)
    {
        moveDirection = direction.normalized;
        moveSpeed = speed;
        lifeTime = destroyAfterSeconds;
        launched = true;

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (!launched)
        {
            return;
        }

        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(-moveDirection);
        }
    }
}
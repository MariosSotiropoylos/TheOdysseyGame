using System.Collections;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    [Header("Enemy Detection")]
    public string enemyTag = "Enemy";

    [Header("Damage Settings")]
    public float damage = 1f;

    [Header("Knockback Settings")]
    public float knockbackForce = 4f;
    public float upwardForce = 0f;
    public float knockbackDuration = 0.25f;

    [Header("Knockback Source: THE PLAYER")]
    public Transform playerRoot;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(enemyTag))
        {
            return;
        }

        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();

        if (enemyHealth == null)
        {
            enemyHealth = other.GetComponentInParent<EnemyHealth>();
        }

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }

        SuitorHealth suitorHealth = other.GetComponent<SuitorHealth>();

        if (suitorHealth == null)
        {
            suitorHealth = other.GetComponentInParent<SuitorHealth>();
        }

        if (suitorHealth != null)
        {
            suitorHealth.TakeDamage(damage);
        }

        PatrolEnemyHealth patrolEnemyHealth = other.GetComponent<PatrolEnemyHealth>();

        if (patrolEnemyHealth == null)
        {
            patrolEnemyHealth = other.GetComponentInParent<PatrolEnemyHealth>();
        }

        if (patrolEnemyHealth != null)
        {
            patrolEnemyHealth.TakeDamage(damage);
        }

        if (enemyHealth == null && suitorHealth == null && patrolEnemyHealth == null)
        {
            return;
        }

        Rigidbody enemyRb = other.GetComponent<Rigidbody>();

        if (enemyRb == null)
        {
            enemyRb = other.GetComponentInParent<Rigidbody>();
        }

        if (enemyRb != null)
        {
            ApplyKnockback(other.transform, enemyRb);
        }
        else
        {
            return;
        }
    }

    void ApplyKnockback(Transform enemyTransform, Rigidbody enemyRb)
    {
        Vector3 knockbackDirection;

        if (playerRoot != null)
        {
            knockbackDirection = enemyTransform.position - playerRoot.position;
        }
        else
        {
            knockbackDirection = enemyTransform.position - transform.root.position;
        }

        knockbackDirection.y = 0f;

        if (knockbackDirection.sqrMagnitude < 0.001f)
        {
            if (playerRoot != null)
            {
                knockbackDirection = playerRoot.forward;
            }
            else
            {
                knockbackDirection = transform.root.forward;
            }
        }

        knockbackDirection.Normalize();

        Vector3 knockbackVelocity = knockbackDirection * knockbackForce;
        knockbackVelocity.y = upwardForce;

        enemyRb.linearVelocity = knockbackVelocity;

        StartCoroutine(StopKnockback(enemyRb, knockbackDuration));
    }

    private IEnumerator StopKnockback(Rigidbody enemyRb, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (enemyRb != null)
        {
            enemyRb.linearVelocity = new Vector3(
                0f,
                enemyRb.linearVelocity.y,
                0f
            );
        }
    }
}
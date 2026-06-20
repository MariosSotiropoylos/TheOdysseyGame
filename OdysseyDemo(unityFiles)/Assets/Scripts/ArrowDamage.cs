using UnityEngine;

public class ArrowDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damage = 1f;

    [Header("Hit Settings")]
    public bool destroyArrowOnHit = true;
    public float destroyDelay = 0f;

    private bool hasHit = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit)
        {
            return;
        }

        if (other.CompareTag("Axe") || other.GetComponentInParent<Transform>().CompareTag("Axe"))
        {
            hasHit = true;

            if (destroyArrowOnHit)
            {
                Destroy(gameObject, destroyDelay);
            }

            return;
        }

        if (!other.CompareTag("Enemy"))
        {
            return;
        }

        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();

        if (enemyHealth == null)
        {
            enemyHealth = other.GetComponentInParent<EnemyHealth>();
        }

        SuitorHealth suitorHealth = other.GetComponent<SuitorHealth>();

        if (suitorHealth == null)
        {
            suitorHealth = other.GetComponentInParent<SuitorHealth>();
        }

        PatrolEnemyHealth patrolEnemyHealth = other.GetComponent<PatrolEnemyHealth>();

        if (patrolEnemyHealth == null)
        {
            patrolEnemyHealth = other.GetComponentInParent<PatrolEnemyHealth>();
        }

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
            hasHit = true;
        }

        if (suitorHealth != null)
        {
            suitorHealth.TakeDamage(damage);
            hasHit = true;
        }

        if (patrolEnemyHealth != null)
        {
            patrolEnemyHealth.TakeDamage(damage);
            hasHit = true;
        }

        if (enemyHealth == null && suitorHealth == null && patrolEnemyHealth == null)
        {
            return;
        }

        if (hasHit && destroyArrowOnHit)
        {
            Destroy(gameObject, destroyDelay);
        }
    }
}
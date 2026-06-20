using System.Collections;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 8f;
    public float lifetime = 5f;
    public int damage = 1;

    [Header("Player Damage Flash")]
    public Material damageFlashMaterial;
    public float flashDuration = 0.1f;
    public int flashCount = 2;

    private Vector3 moveDirection;
    private bool hasHit = false;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    public void SetDirection(Vector3 direction)
    {
        moveDirection = direction.normalized;
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasHit)
            return;

        HealthManager health = other.GetComponent<HealthManager>();

        if (health != null && health.CanTakeDamage())
        {
            hasHit = true;

            StartCoroutine(FlashPlayerMaterial(other.gameObject));
            health.TakeDamage(damage);

            Destroy(gameObject, flashDuration * flashCount * 2f + 0.05f);
            return;
        }

        if (!other.isTrigger)
        {
            hasHit = true;
            Destroy(gameObject);
        }
    }

    IEnumerator FlashPlayerMaterial(GameObject playerObject)
    {
        if (damageFlashMaterial == null)
            yield break;

        Renderer[] renderers = playerObject.GetComponentsInChildren<Renderer>(true);

        if (renderers.Length == 0)
            yield break;

        Material[][] originalMaterials = new Material[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].sharedMaterials;
        }

        for (int flash = 0; flash < flashCount; flash++)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] == null)
                    continue;

                Material[] flashMaterials = new Material[originalMaterials[i].Length];

                for (int j = 0; j < flashMaterials.Length; j++)
                    flashMaterials[j] = damageFlashMaterial;

                renderers[i].sharedMaterials = flashMaterials;
            }

            yield return new WaitForSeconds(flashDuration);

            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                    renderers[i].sharedMaterials = originalMaterials[i];
            }

            yield return new WaitForSeconds(flashDuration);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
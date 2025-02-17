using UnityEngine;
using System.Collections;

public class ExplosionScript : MonoBehaviour
{
    [SerializeField] float explosionRadius = 3.0f;
    private float minExplosionForce = 8.0f;
    private float maxExplosionForce = 20.0f;

    private int minDamage = 30;
    private int maxDamage = 70;
    [SerializeField] ParticleSystem explosionPrefab;
    [SerializeField] private AudioSource explosionAudioSource;
    [SerializeField] private AudioClip explosionSound;
    public void Explode()
    {
        AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        if (explosionPrefab != null)
        {
            Vector3 explosionSpawnPosition = transform.position + new Vector3 (0, 0, -2.0f);
            Instantiate(explosionPrefab, explosionSpawnPosition, Quaternion.identity);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hitCollider in colliders)
        {

            if (hitCollider.CompareTag("MapVoxel"))
            {
                StartCoroutine(DelayedDestroy(hitCollider.gameObject));
            }

            if (hitCollider.CompareTag("Worm"))
            {
                GameObject worm = hitCollider.gameObject;
                WormScript wormScript = worm.GetComponent<WormScript>();
                Rigidbody rb = wormScript.GetComponent<Rigidbody>();

                float distance = Vector3.Distance(transform.position, worm.transform.position);

                ApplyExplosionForce(rb, GetFinalExplosionForce(distance)); //TODO zeby force skalowal sie z odlegloscia
                wormScript.TakeDamage(GetFinalDamage(distance), this.gameObject); //TODO damage tez
            }
            if (hitCollider.CompareTag("Mine"))
            {
                hitCollider.gameObject.GetComponent<MineScript>().MineExplode();
            }

        }

        StartCoroutine(DelayedDestroy(gameObject));
    }

    private float ScaleExplosionForce(float distance)
    {
        return maxExplosionForce * (1.0f - Mathf.Clamp01(distance / explosionRadius));
    }

    private int ScaleDamage(float distance)
    {
        return (int)(maxDamage * (1.0f - Mathf.Clamp01(distance / explosionRadius)));
    }

    private float GetFinalExplosionForce(float distance)
    {
        return Mathf.Clamp(ScaleExplosionForce(distance), minExplosionForce, maxExplosionForce);
    }

    private int GetFinalDamage(float distance)
    {
        return Mathf.Clamp(ScaleDamage(distance), minDamage, maxDamage);
    }

    public void ApplyExplosionForce(Rigidbody rb, float explosionForce)
    {
        Vector3 forceDirection = (rb.transform.position - transform.position + new Vector3(0f, 0.2f, 0f)).normalized;
        rb.AddForce(forceDirection * explosionForce, ForceMode.Impulse);
    }

    public IEnumerator DelayedDestroy(GameObject obj)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(0.05f);
        Destroy(obj);
    }

}

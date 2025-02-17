using UnityEngine;
using System.Collections;

public class RocketScript : MonoBehaviour
{
    [SerializeField] Rigidbody rocketRb;
    [SerializeField] ExplosionScript explosion;
    CameraControlScript cameraControlScript;
    [SerializeField] AudioSource enterWaterAudioSource;
    [SerializeField] AudioClip enterWaterAudioClip;

    private bool isExploded = false;

    private void Start()
    {
        cameraControlScript = FindObjectOfType<CameraControlScript>();
        if (cameraControlScript != null)
            StartCoroutine(cameraControlScript.FollowProjectile(this.gameObject));
    }

    private void FixedUpdate()
    {
        UpdateRotation();
        WeatherScript.ApplyWindForce(rocketRb);
    }

    private void UpdateRotation()
    {
        float angle = Mathf.Atan2(rocketRb.velocity.y, rocketRb.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + 180);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isExploded)
        {
            explosion.Explode();
            EventManager.TriggerWeaponLanded();
            isExploded = true;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!isExploded)
        {
            if (collision.CompareTag("Water"))
            {
                EventManager.TriggerWeaponLanded();
                isExploded = true;
                cameraControlScript.StopFollowing();
                enterWaterAudioSource.PlayOneShot(enterWaterAudioClip);
                StartCoroutine(DelayedDestroy());
            }
        }
    }

    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }
}
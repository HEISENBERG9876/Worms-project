using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BazookaScript : UsableItemScript
{
    [SerializeField] GameObject rocketPrefab;
    [SerializeField] GameObject bazookaRotationPivot;
    [SerializeField] GameObject spherePrefab;

    [SerializeField] private AudioSource shotAudioSource;
    [SerializeField] private AudioClip shotSound;
    [SerializeField] private AudioSource chargingAudioSource;
    [SerializeField] private AudioClip chargingSound;

    [SerializeField] float minShootingForce = 5f;
    [SerializeField] float maxShootingForce = 30f;
    [SerializeField] float rotationSpeed = 0.2f;
    private float chargingDuration = 1.0f;

    private bool isChargingShot = false;
    private float chargingStartTime;

    public override void Use() //strzelanie
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (!isChargingShot)
            {
                isChargingShot = true;
                StartCoroutine(startCharging());
            }
        }
    }

    private void Shoot(float shootingForce)
    {

        Vector3 spawnOffset = new Vector3(1.5f, 0.0f, 0.0f);
        Vector3 spawnPosition = bazookaRotationPivot.transform.TransformPoint(spawnOffset);

        GameObject rocket = Instantiate(rocketPrefab, spawnPosition, bazookaRotationPivot.transform.rotation);
        Rigidbody projectileRb = rocket.GetComponent<Rigidbody>();

        projectileRb.AddForce(bazookaRotationPivot.transform.right * shootingForce, ForceMode.Impulse);
        shotAudioSource.PlayOneShot(shotSound);

        EventManager.TriggerWeaponUsed();
    }

    private IEnumerator startCharging()
    {
        chargingAudioSource.PlayOneShot(chargingSound);

        chargingStartTime = Time.time;
        float shootingForce = minShootingForce;
        StartCoroutine(chargeAnimation(chargingStartTime));

        while (Time.time - chargingStartTime < chargingDuration && Input.GetKey(KeyCode.Space))
        {
            float chargingProgress = (Time.time - chargingStartTime) / chargingDuration;
            shootingForce = Mathf.Lerp(minShootingForce, maxShootingForce, chargingProgress);
            yield return null;
        }

        chargingAudioSource.Stop();
        Shoot(shootingForce);

        yield return new WaitForSeconds(0.5f);

        isChargingShot = false;
        this.Hide();
    }

    private IEnumerator chargeAnimation(float chargingStartTime)
    {
        List<GameObject> sphereList = new List<GameObject>();

        while (Time.time - chargingStartTime < chargingDuration && Input.GetKey(KeyCode.Space))
        {
            float chargingProgress = (Time.time - chargingStartTime) / chargingDuration;
            float scale = Mathf.Lerp(0.2f, 1.2f, chargingProgress);

            Vector3 spawnOffset = new Vector3(Mathf.Lerp(0.5f, 2.0f, chargingProgress) + 0.9f, 0.05f, 0.1f);
            Vector3 spawnPosition = bazookaRotationPivot.transform.TransformPoint(spawnOffset); //chyba da sie prosciej

            GameObject sphere = Instantiate(spherePrefab, spawnPosition, bazookaRotationPivot.transform.rotation, transform);
            sphere.transform.localScale = new Vector3(scale, scale, scale);

            Renderer sphereRenderer = sphere.GetComponent<Renderer>();
            Color lerpedColor = Color.Lerp(Color.yellow, Color.red, chargingProgress);
            sphereRenderer.material.color = lerpedColor;

            sphereList.Add(sphere);

            yield return new WaitForSeconds(0.01f);
        }

        foreach(GameObject sphere in sphereList)
        {
            Destroy(sphere);
        }
    }

    public override void Rotate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            bazookaRotationPivot.transform.Rotate(0f, 0f, rotationSpeed);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            bazookaRotationPivot.transform.Rotate(0f, 0f, -rotationSpeed);
        }
    }

    private void OnDisable()
    {
        isChargingShot = false;
    }
}

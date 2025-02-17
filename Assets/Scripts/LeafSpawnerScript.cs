using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafSpawnerScript : MonoBehaviour
{
    [SerializeField] GameObject leafPrefab;
    private List<Rigidbody> activeLeaves = new List<Rigidbody>();

    private void Start()
    {
        StartCoroutine(KeepSpawningLeafs());
    }
    private IEnumerator KeepSpawningLeafs()
    {
        while (true)
        {
            StartCoroutine(SpawnLeaf());
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator SpawnLeaf()
    {
        float randomX = Random.Range(-200.0f, 250.0f);
        Vector3 spawnPosition = new Vector3(randomX, 100.0f, 15.0f);
 

        GameObject leaf = Instantiate(leafPrefab, spawnPosition, Quaternion.identity, transform);

        float randomScale = Random.Range(0.5f, 1.0f);
        leaf.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

        Rigidbody leafRb = leaf.GetComponent<Rigidbody>();
        Vector3 randomAngularForce = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));
        leafRb.AddTorque(randomAngularForce, ForceMode.Impulse);

        activeLeaves.Add(leafRb);

        yield return DespawnLeaf(leaf);
    }

    private IEnumerator DespawnLeaf(GameObject leaf)
    {
        while (leaf.transform.position.y > -100)
        {
            yield return new WaitForSeconds(1.0f);
        }

        Destroy(leaf);
        activeLeaves.Remove(leaf.GetComponent<Rigidbody>());
    }

    private void FixedUpdate()
    {
        foreach(Rigidbody rb in activeLeaves)
        {
            WeatherScript.ApplyWindForce(rb);
        }
    }

}

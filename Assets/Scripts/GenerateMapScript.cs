using UnityEngine;
using System.Collections.Generic;

public class GenerateMapScript : MonoBehaviour
{
    [SerializeField] List<GameObject> voxelPrefabs;
    [SerializeField] List<GameObject> waterPrefabs;
    private GameObject voxelPrefab;
    private GameObject waterPrefab;

    private float xOffset;
    private float yOffset;
    private int scale;
    private float perlinThreshold;
    void Awake()
    {
        //Offset, scale i perlinThreshold randomizujemy, zeby miec pseudo-losowe mapy. Offset przesuwa wartosci szumu,
        //scale okresla, jak szybko zachodza zmiany, a Threshold okresla, ponizej jakiej wartosci nie rysujemy voxli.
        xOffset = Random.Range(0f, 100f);
        yOffset = Random.Range(0f, 100f);
        scale = Random.Range(8, 15);
        perlinThreshold = (float)Random.Range(45, 60) / 100;

    }

    private void Start()
    {
        int randomVoxelIndex = Random.Range(0, voxelPrefabs.Count);
        voxelPrefab = voxelPrefabs[randomVoxelIndex];

        int randomWaterIndex = Random.Range(0, waterPrefabs.Count);
        waterPrefab = waterPrefabs[randomWaterIndex];
    }
    public void GenerateMap(int width, int height, float voxelSideLength)
    {
        for (int x = 0; x < width; x++)
        {
            float xCoord = x * voxelSideLength;
            
            for (int y = 0; y < height; y++)
            {
                float yCoord = y * voxelSideLength;

                float xNormalized = (float)(xCoord) / width;
                float yNormalized = (float)(yCoord) / height;
                float perlinValue = Mathf.PerlinNoise(xNormalized * scale + xOffset, yNormalized * scale + yOffset);

                Vector3 pos = new Vector3(xCoord, yCoord, 0);

                if (perlinValue > perlinThreshold)
                {
                    Instantiate(voxelPrefab, pos, Quaternion.identity, transform);
                }
            }
        }
        Instantiate(waterPrefab, new Vector3 (0, -500, 0), Quaternion.identity);
    }
}

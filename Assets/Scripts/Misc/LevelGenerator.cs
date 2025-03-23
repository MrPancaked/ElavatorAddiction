using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    #region Variables

    [Header("References")]
    public Terrain terrain;
    public Transform mapCenter;

    [Header("Spawn Settings")]
    public int hutsMin = 2;
    public int hutsMax = 6;
    public int treesMin = 20;
    public int treesMax = 50;
    public int rocksMin = 20;
    public int rocksMax = 30;
    public int jumpPadsMin = 3;
    public int jumpPadsMax = 5;
    public float obstacleCheckRadius = 5f;
    public float playerSpawnClearRadius = 15f;
    public float minHutDistance = 25f;
    public float graveyardDistance = 35f;

    [Header("Prefabs")]
    public GameObject churchPrefab;
    public GameObject graveyardPrefab;
    public GameObject hutPrefab;
    public GameObject jumpPadPrefab;
    public GameObject[] treePrefabs;
    public GameObject[] rockPrefabs;
    public GameObject bonfirePrefab; // Added bonfire prefab reference

    //private shit
    private Dictionary<GameObject, float> prefabRadii = new Dictionary<GameObject, float>();
    private Transform playerSpawnPoint;
    private List<Vector3> spawnedHutPositions = new List<Vector3>();
    private const int MaxTries = 50;
    public static LevelGenerator instance;
    public static LevelGenerator Instance { get { return instance; } }

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        CachePrefabRadii();
        playerSpawnPoint = GameObject.FindGameObjectWithTag("Elevator")?.transform;
        if (playerSpawnPoint == null)
        {
            Debug.LogError("Player spawn point with tag 'Elevator' not found.");
        }
        //balls
    }

    #endregion

    #region Level Generation

    public void GenerateLevel()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        spawnedHutPositions.Clear();
        Vector3 churchPosition = SpawnChurch();
        SpawnGraveyard(churchPosition, 0f);
        SpawnBonfire();
        int numJumpPads = Random.Range(jumpPadsMin, jumpPadsMax + 1);
        for (int i = 0; i < numJumpPads; i++)
        {
            SpawnObject(jumpPadPrefab, true, false);
        }
        int numHuts = Random.Range(hutsMin, hutsMax + 1);
        for (int i = 0; i < numHuts; i++)
        {
            SpawnHut();
        }
        int numTrees = Random.Range(treesMin, treesMax + 1);
        for (int i = 0; i < numTrees; i++)
        {
            SpawnObject(treePrefabs[Random.Range(0, treePrefabs.Length)], true);
        }
        int numRocks = Random.Range(rocksMin, rocksMax + 1);
        for (int i = 0; i < numRocks; i++)
        {
            SpawnObject(rockPrefabs[Random.Range(0, rockPrefabs.Length)], true);
        }
    }

    #endregion

    #region Spawning

    Vector3 SpawnChurch()
    {
        if (churchPrefab == null)
        {
            Debug.LogError("No church prefab assigned");
            return Vector3.zero;
        }
        Vector3 churchPosition = GetChurchSpawnPosition();
        Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        GameObject spawnedChurch = Instantiate(churchPrefab, churchPosition, randomRotation, transform);
        spawnedChurch.tag = "Obsctacle";
        return churchPosition;
    }

    void SpawnGraveyard(Vector3 churchPosition, float graveyardIndex)
    {
        if (graveyardPrefab == null)
        {
            Debug.LogError("No graveyard prefab assigned");
            return;
        }
        int tries = 0;
        while (tries < MaxTries)
        {
            Vector3 graveyardPosition = GetGraveyardSpawnPosition(churchPosition, graveyardIndex);
            if (!CheckForObstacles(graveyardPosition, prefabRadii[graveyardPrefab]) && !IsTooCloseToPlayerSpawn(graveyardPosition))
            {
                Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                GameObject spawnedGraveyard = Instantiate(graveyardPrefab, graveyardPosition, randomRotation, transform);
                spawnedGraveyard.tag = "Obsctacle";
                return;
            }
            tries++;
        }
        Debug.LogWarning("Could not find a suitable spot for Graveyard after " + MaxTries + " tries");
    }
    // Method to spawn a bonfire
    void SpawnBonfire()
    {
        if (bonfirePrefab == null)
        {
            Debug.LogError("No bonfire prefab assigned");
            return;
        }
        SpawnObject(bonfirePrefab, true); // Use the existing SpawnObject method to spawn the bonfire
    }
    void SpawnHut()
    {
        int tries = 0;
        while (tries < MaxTries)
        {
            Vector3 spawnPosition = GetRandomPositionOnTerrain();
            if (!CheckForObstacles(spawnPosition, prefabRadii[hutPrefab]) && !IsTooCloseToPlayerSpawn(spawnPosition) && !IsTooCloseToOtherHuts(spawnPosition))
            {
                Quaternion rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                GameObject spawnedObject = Instantiate(hutPrefab, spawnPosition, rotation, transform);
                spawnedObject.tag = "Obsctacle";
                spawnedHutPositions.Add(spawnPosition);
                return;
            }
            tries++;
        }
        Debug.LogWarning("Could not find a suitable spot for hut after " + MaxTries + " tries");
    }

    void SpawnObject(GameObject prefab, bool shouldRotate, bool addYOffset = true)
    {
        int tries = 0;
        while (tries < MaxTries)
        {
            Vector3 spawnPosition = GetRandomPositionOnTerrain(addYOffset);
            if (!CheckForObstacles(spawnPosition, prefabRadii[prefab]) && !IsTooCloseToPlayerSpawn(spawnPosition))
            {
                Quaternion rotation = Quaternion.identity;
                if (shouldRotate)
                {
                    rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                }
                GameObject spawnedObject = Instantiate(prefab, spawnPosition, rotation, transform);
                spawnedObject.tag = "Obsctacle";
                return;
            }
            tries++;
        }
        Debug.LogWarning("Could not find a suitable spot after " + MaxTries + " tries");
    }


    #endregion

    #region Other

    bool CheckForObstacles(Vector3 position, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, radius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Obsctacle"))
            {
                return true;
            }
        }
        return false;
    }

    bool IsTooCloseToOtherHuts(Vector3 position)
    {
        foreach (Vector3 hutPosition in spawnedHutPositions)
        {
            if (Vector3.Distance(position, hutPosition) < minHutDistance)
            {
                return true;
            }
        }
        return false;
    }

    bool IsTooCloseToPlayerSpawn(Vector3 position)
    {
        if (playerSpawnPoint == null)
        {
            return false;
        }
        return Vector3.Distance(position, playerSpawnPoint.position) < playerSpawnClearRadius;
    }

    Vector3 GetRandomPositionOnTerrain(bool addYOffset = true)
    {
        Vector3 terrainSize = terrain.terrainData.size;
        float x = Random.Range(-terrainSize.x / 2, terrainSize.x / 2);
        float z = Random.Range(-terrainSize.z / 2, terrainSize.z / 2);
        Vector3 worldPosition = new Vector3(x + mapCenter.position.x, 0, z + mapCenter.position.z);
        float terrainY = terrain.SampleHeight(worldPosition);
        float finalY = terrainY + terrain.transform.position.y;
        if (addYOffset)
        {
            finalY += 2.5f;
        }
        return new Vector3(worldPosition.x, finalY, worldPosition.z);
    }

    Vector3 GetChurchSpawnPosition()
    {
        Vector3 terrainSize = terrain.terrainData.size;
        Vector3 worldPosition = new Vector3(mapCenter.position.x, 0, mapCenter.position.z);
        float terrainY = terrain.SampleHeight(worldPosition);
        float finalY = terrainY + terrain.transform.position.y + 2.5f;
        return new Vector3(worldPosition.x, finalY, worldPosition.z);
    }

    Vector3 GetGraveyardSpawnPosition(Vector3 churchPosition, float graveyardIndex)
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        angle += (graveyardIndex * Mathf.PI); //Offset the angle of one of the graveyard by 180 degrees
        float xOffset = Mathf.Cos(angle) * graveyardDistance;
        float zOffset = Mathf.Sin(angle) * graveyardDistance;
        Vector3 finalPosition = new Vector3(churchPosition.x + xOffset, 0, churchPosition.z + zOffset);
        float terrainY = terrain.SampleHeight(finalPosition);
        float finalY = terrainY + terrain.transform.position.y;
        finalPosition.y = finalY;
        return finalPosition;
    }

    float GetBoundingRadius(GameObject prefab)
    {
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            bounds = renderers[0].bounds;

            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }
        }
        else
        {
            Collider[] colliders = prefab.GetComponentsInChildren<Collider>();
            if (colliders.Length > 0)
            {
                bounds = colliders[0].bounds;
                for (int i = 1; i < colliders.Length; i++)
                {
                    bounds.Encapsulate(colliders[i].bounds);
                }
            }
            else
            {
                Debug.LogError("No renderers or colliders found in " + prefab.name);
                return 0.5f;
            }
        }
        return Mathf.Max(bounds.extents.x, bounds.extents.z);
    }

    void CachePrefabRadii()
    {
        if (churchPrefab != null && !prefabRadii.ContainsKey(churchPrefab))
            prefabRadii.Add(churchPrefab, GetBoundingRadius(churchPrefab));
        if (graveyardPrefab != null && !prefabRadii.ContainsKey(graveyardPrefab))
            prefabRadii.Add(graveyardPrefab, GetBoundingRadius(graveyardPrefab));
        if (jumpPadPrefab != null && !prefabRadii.ContainsKey(jumpPadPrefab))
            prefabRadii.Add(jumpPadPrefab, GetBoundingRadius(jumpPadPrefab));
        if (hutPrefab != null && !prefabRadii.ContainsKey(hutPrefab))
            prefabRadii.Add(hutPrefab, GetBoundingRadius(hutPrefab));
        if (bonfirePrefab != null && !prefabRadii.ContainsKey(bonfirePrefab)) // Add the bonfire to the prefab radii list
            prefabRadii.Add(bonfirePrefab, GetBoundingRadius(bonfirePrefab));
        if (treePrefabs != null)
        {
            foreach (GameObject prefab in treePrefabs)
            {
                if (!prefabRadii.ContainsKey(prefab))
                    prefabRadii.Add(prefab, GetBoundingRadius(prefab));
            }
        }
        if (rockPrefabs != null)
        {
            foreach (GameObject prefab in rockPrefabs)
            {
                if (!prefabRadii.ContainsKey(prefab))
                    prefabRadii.Add(prefab, GetBoundingRadius(prefab));
            }
        }
    }

    #endregion
}
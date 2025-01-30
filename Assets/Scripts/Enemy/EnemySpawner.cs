using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;
    public int minEnemiesPerGroup = 3;
    public int maxEnemiesPerGroup = 5;

    [Header("Group Settings")]
    public int minGroups = 3;
    public int maxGroups = 5;
    public float groupSpacing = 15f; // Minimum distance between enemy groups

    [Header("Map Settings")]
    public float mapWidth = 100f; // width of the area
    public float mapHeight = 100f; //height of the area
    public float minHeight = 0f; // Minimum spawn height, relative to mapCenter.y
    public float maxHeight = 1.5f; // Maximum spawn height, relative to mapCenter.y


    [Header("References")]
    public Transform mapCenter;
    public Transform enemiesParent;

    //private shit
    private List<Vector3> spawnedGroupPositions = new List<Vector3>();
    private static EnemySpawner instance;
    public static EnemySpawner Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject); // Enforce singleton pattern
            return;
        }
        else
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);  Removed this because it shouldn't persist
        }
    }

    public void SpawnEnemies()
    {
        if (enemiesParent != null) // check if parent exists
        {
            for (int i = enemiesParent.childCount - 1; i >= 0; i--) // Loop through the children of the transform
            {
                DestroyImmediate(enemiesParent.GetChild(i).gameObject); // Destroy each child object immediately
            }
        }

        spawnedGroupPositions.Clear();

        int numGroups = Random.Range(minGroups + ElevatorController.Instance.RoomIndex, maxGroups + 1 + ElevatorController.Instance.RoomIndex);

        for (int i = 0; i < numGroups; i++)
        {
            SpawnEnemyGroup();
        }
    }

    private void SpawnEnemyGroup()
    {
        int numEnemies = Random.Range(minEnemiesPerGroup, maxEnemiesPerGroup + 1);
        int tries = 0;

        while (tries < 50)
        {
            Vector3 groupSpawnPosition = GetRandomPositionInArea();
            if (!CheckForObstacles(groupSpawnPosition, GetBoundingRadius(enemyPrefab) * 2f) && !IsTooCloseToOtherGroups(groupSpawnPosition))
            {
                for (int i = 0; i < numEnemies; i++)
                {
                    Vector3 spawnPosition = groupSpawnPosition;
                    //add a slight offset
                    float offsetRange = 2f;
                    spawnPosition.x += Random.Range(-offsetRange, offsetRange);
                    spawnPosition.z += Random.Range(-offsetRange, offsetRange);
                    GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                    spawnedEnemy.transform.SetParent(enemiesParent);
                }
                spawnedGroupPositions.Add(groupSpawnPosition);
                return;
            }

            tries++;
        }

        Debug.LogWarning("Could not find suitable position for enemy group");

    }

    private bool IsTooCloseToOtherGroups(Vector3 position)
    {
        foreach (Vector3 groupPosition in spawnedGroupPositions)
        {
            if (Vector3.Distance(position, groupPosition) < groupSpacing)
            {
                return true;
            }
        }
        return false;
    }

    //------ LEVEL GENERATION LOGIC -------//

    Vector3 GetRandomPositionInArea()
    {
        float x = Random.Range(-mapWidth / 2, mapWidth / 2);
        float z = Random.Range(-mapHeight / 2, mapHeight / 2);
        float y = Random.Range(minHeight, maxHeight);
        return new Vector3(x + mapCenter.position.x, y + mapCenter.position.y, z + mapCenter.position.z);
    }


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
}
//--------------------------------------------------------------------------------------------------
//  Description: This script generates a level by instantiating prefabs on a terrain,
//               handling object placement and avoiding collisions with each other and the player's spawn point.
//--------------------------------------------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine; 

public class LevelGenerator : MonoBehaviour 
{
    #region Variables

    [Header("References")] 
    public Terrain terrain; // Reference to the Terrain object
    public Transform mapCenter; // Reference to the map center Transform

    [Header("Spawn Settings")]
    public int hutsMin = 2; // Minimum number of huts to spawn
    public int hutsMax = 6; // Maximum number of huts to spawn
    public int treesMin = 20; // Minimum number of trees to spawn
    public int treesMax = 50; // Maximum number of trees to spawn
    public int rocksMin = 20; // Minimum number of rocks to spawn
    public int rocksMax = 30; // Maximum number of rocks to spawn
    public int jumpPadsMin = 3; // Minimum number of jump pads to spawn
    public int jumpPadsMax = 5; // Maximum number of jump pads to spawn
    public float obstacleCheckRadius = 5f; // Radius to check for obstacles
    public float playerSpawnClearRadius = 15f; // Radius around player spawn where objects shouldn't spawn
    public float minHutDistance = 25f; // Minimum distance between huts
    public float graveyardDistance = 35f; // Increased default distance for graveyards

    [Header("Prefabs")]
    public GameObject churchPrefab; // Reference to the church prefab
    public GameObject graveyardPrefab; // Reference to the graveyard prefab
    public GameObject hutPrefab; // Reference to the hut prefab
    public GameObject jumpPadPrefab; // Reference to the jump pad prefab
    public GameObject[] treePrefabs; // Array of tree prefabs
    public GameObject[] rockPrefabs; // Array of rock prefabs

    //private shit
    private Dictionary<GameObject, float> prefabRadii = new Dictionary<GameObject, float>(); // Cache for object radii
    private Transform playerSpawnPoint; // Reference to the player's spawn point Transform
    private List<Vector3> spawnedHutPositions = new List<Vector3>(); // List to store the positions of spawned huts
    private const int MaxTries = 50; // Maximum number of tries to find a suitable spawn position
    public static LevelGenerator instance;
    public static LevelGenerator Instance { get { return instance; } }

    #endregion

    #region Unity Methods

    private void Awake()  /// Makes sure this object survives the scene transition, and that there is only one.
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // IMPORTANT!
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start() // Start is called before the first frame update
    {
        CachePrefabRadii(); // Call the CachePrefabRadii method
        playerSpawnPoint = GameObject.FindGameObjectWithTag("Elevator")?.transform; // Find the player's spawn point by tag
        if (playerSpawnPoint == null) // Check if the player spawn point was found
        {
            Debug.LogError("Player spawn point with tag 'elevator' not found."); // Log an error if the spawn point is not found
        }
    }

    #endregion

    #region Level Generation

    public void GenerateLevel() // Method to generate the level
    {
        for (int i = transform.childCount - 1; i >= 0; i--) // Loop through the children of the transform
        {
            DestroyImmediate(transform.GetChild(i).gameObject); // Destroy each child object immediately
        }

        spawnedHutPositions.Clear(); // Clear hut positions list at the start of every new level
        Vector3 churchPosition = SpawnChurch(); // Spawn the church and get its position
        SpawnGraveyard(churchPosition, 0f); // Spawn the first graveyard
        SpawnGraveyard(churchPosition, 1f); // Spawn the second graveyard
        int numJumpPads = Random.Range(jumpPadsMin, jumpPadsMax + 1); // Get a random number of jump pads to spawn
        for (int i = 0; i < numJumpPads; i++) // Loop through to spawn jump pads
        {
            SpawnObject(jumpPadPrefab, true, false); // Spawn the jump pads without the Y offset
        }
        int numHuts = Random.Range(hutsMin, hutsMax + 1); // Get a random number of huts to spawn
        for (int i = 0; i < numHuts; i++) // Loop through to spawn huts
        {
            SpawnHut(); // Spawn huts
        }
        int numTrees = Random.Range(treesMin, treesMax + 1); // Get a random number of trees to spawn
        for (int i = 0; i < numTrees; i++) // Loop through to spawn trees
        {
            SpawnObject(treePrefabs[Random.Range(0, treePrefabs.Length)], true); // Spawn trees with random rotation
        }
        int numRocks = Random.Range(rocksMin, rocksMax + 1); // Get a random number of rocks to spawn
        for (int i = 0; i < numRocks; i++) // Loop through to spawn rocks
        {
            SpawnObject(rockPrefabs[Random.Range(0, rockPrefabs.Length)], true); // Spawn rocks with random rotation
        }
    }

    #endregion

    #region Spawning

    Vector3 SpawnChurch() // Method to spawn the church
    {
        if (churchPrefab == null) // Check if the church prefab exists
        {
            Debug.LogError("No church prefab assigned"); // Log an error if no church prefab is assigned
            return Vector3.zero; // Return a zero vector
        }
        Vector3 churchPosition = GetChurchSpawnPosition(); // Get the church spawn position
        Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0); // Generate a random rotation around the Y-axis
        GameObject spawnedChurch = Instantiate(churchPrefab, churchPosition, randomRotation, transform); // Instantiate the church prefab
        spawnedChurch.tag = "Obsctacle"; // Set the tag of the spawned object as "Obsctacle"
        return churchPosition; // Return the church position
    }

    void SpawnGraveyard(Vector3 churchPosition, float graveyardIndex) // Method to spawn graveyards
    {
        if (graveyardPrefab == null) // Check if the graveyard prefab exists
        {
            Debug.LogError("No graveyard prefab assigned"); // Log an error if no graveyard prefab is assigned
            return; // Return if no prefab
        }
        int tries = 0; // Initialize the tries variable to 0
        while (tries < MaxTries) // Loop while tries are less than the max tries variable
        {
            Vector3 graveyardPosition = GetGraveyardSpawnPosition(churchPosition, graveyardIndex); // Get graveyard spawn position
            if (!CheckForObstacles(graveyardPosition, prefabRadii[graveyardPrefab]) && !IsTooCloseToPlayerSpawn(graveyardPosition)) // Check for obstacles and player spawn proximity
            {
                Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0); // Generate a random rotation around the Y-axis
                GameObject spawnedGraveyard = Instantiate(graveyardPrefab, graveyardPosition, randomRotation, transform); // Instantiate graveyard prefab
                spawnedGraveyard.tag = "Obsctacle"; // Set the tag of the spawned object as "Obsctacle"
                return; // Exit function after a succesfull spawn
            }
            tries++; // Add +1 to the tries
        }
        Debug.LogWarning("Could not find a suitable spot for Graveyard after " + MaxTries + " tries"); // Output message in case of too many tries
    }

    void SpawnHut() // Method to spawn huts
    {
        int tries = 0; // Initialize the tries variable to 0
        while (tries < MaxTries) // Loop while tries are less than the max tries variable
        {
            Vector3 spawnPosition = GetRandomPositionOnTerrain(); // Get random position on the terrain
            if (!CheckForObstacles(spawnPosition, prefabRadii[hutPrefab]) && !IsTooCloseToPlayerSpawn(spawnPosition) && !IsTooCloseToOtherHuts(spawnPosition)) // Check for obstacles, player spawn, and proximity to other huts
            {
                Quaternion rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0); // Generate a random rotation around the Y-axis
                GameObject spawnedObject = Instantiate(hutPrefab, spawnPosition, rotation, transform); // Instantiate hut with rotation
                spawnedObject.tag = "Obsctacle"; // Set the tag of the spawned object as "Obsctacle"
                spawnedHutPositions.Add(spawnPosition); // Add the spawned hut position to the list
                return; // Exit method once spawned
            }
            tries++; // Add +1 to the tries variable
        }
        Debug.LogWarning("Could not find a suitable spot for hut after " + MaxTries + " tries"); // Output message in case of too many tries
    }

    void SpawnObject(GameObject prefab, bool shouldRotate, bool addYOffset = true) // Method to spawn objects
    {
        int tries = 0; // Initialize the tries variable to 0
        while (tries < MaxTries) // Loop while tries are less than the max tries variable
        {
            Vector3 spawnPosition = GetRandomPositionOnTerrain(addYOffset); // Get random position on the terrain
            if (!CheckForObstacles(spawnPosition, prefabRadii[prefab]) && !IsTooCloseToPlayerSpawn(spawnPosition)) // Check for obstacles and player spawn proximity
            {
                Quaternion rotation = Quaternion.identity; // Set default rotation
                if (shouldRotate) // Check if should rotate
                {
                    rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0); // Generate a random rotation around the Y-axis
                }
                GameObject spawnedObject = Instantiate(prefab, spawnPosition, rotation, transform); // Instantiate object with rotation
                spawnedObject.tag = "Obsctacle"; // Set the tag of the spawned object as "Obsctacle"
                return; // Exit method once spawned
            }
            tries++; // Add +1 to the tries variable
        }
        Debug.LogWarning("Could not find a suitable spot after " + MaxTries + " tries"); // Output message in case of too many tries
    }


    #endregion

    #region Other

    bool CheckForObstacles(Vector3 position, float radius) // Method to check for obstacles
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, radius); // Get colliders in the specified radius
        foreach (var hitCollider in hitColliders) // Loop through each collider
        {
            if (hitCollider.CompareTag("Obsctacle")) // Check if the collider has the "Obsctacle" tag
            {
                return true; // Return true if obstacle found
            }
        }
        return false; // Return false if no obstacle found
    }

    bool IsTooCloseToOtherHuts(Vector3 position) // Method to check if the position is too close to other huts
    {
        foreach (Vector3 hutPosition in spawnedHutPositions) // Loop through all spawned hut positions
        {
            if (Vector3.Distance(position, hutPosition) < minHutDistance) // Check if the position is too close to other huts
            {
                return true; // Return true if the position is too close to other huts
            }
        }
        return false; // Return false if the position is not too close to other huts
    }

    bool IsTooCloseToPlayerSpawn(Vector3 position) // Method to check if the position is too close to the player spawn point
    {
        if (playerSpawnPoint == null) // Check if player spawn point exists
        {
            return false; // Return false if player spawn point does not exist
        }
        return Vector3.Distance(position, playerSpawnPoint.position) < playerSpawnClearRadius; // Return true if distance is less than clear radius
    }

    Vector3 GetRandomPositionOnTerrain(bool addYOffset = true) // Method to get a random position on the terrain
    {
        Vector3 terrainSize = terrain.terrainData.size; // Get the size of the terrain
        float x = Random.Range(-terrainSize.x / 2, terrainSize.x / 2); // Get random X position relative to center of terrain
        float z = Random.Range(-terrainSize.z / 2, terrainSize.z / 2); // Get random Z position relative to center of terrain
        Vector3 worldPosition = new Vector3(x + mapCenter.position.x, 0, z + mapCenter.position.z); // Convert to world space with mapCenter
        float terrainY = terrain.SampleHeight(worldPosition); // Sample the terrain height at the world position
        float finalY = terrainY + terrain.transform.position.y; // Apply the terrain's Y position
        if (addYOffset) // Check if should add Y offset
        {
            finalY += 2.5f; // Add an offset of 2.5f to the Y position
        }
        return new Vector3(worldPosition.x, finalY, worldPosition.z); // Return the final world position
    }

    Vector3 GetChurchSpawnPosition() // Method to get the spawn position for the church
    {
        Vector3 terrainSize = terrain.terrainData.size; // Get the terrain size
        Vector3 worldPosition = new Vector3(mapCenter.position.x, 0, mapCenter.position.z); // Calculate the center world position based on the map center
        float terrainY = terrain.SampleHeight(worldPosition); // Sample the terrain height at the world position
        float finalY = terrainY + terrain.transform.position.y + 2.5f; // Add terrain's Y and an offset of 2.5f
        return new Vector3(worldPosition.x, finalY, worldPosition.z); // Return the final world position
    }

    Vector3 GetGraveyardSpawnPosition(Vector3 churchPosition, float graveyardIndex) // Method to get the spawn position for the graveyard
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad; // Get a random angle
        angle += (graveyardIndex * Mathf.PI); //Offset the angle of one of the graveyard by 180 degrees
        float xOffset = Mathf.Cos(angle) * graveyardDistance; // Calculate the X position based on the angle and the graveyard distance
        float zOffset = Mathf.Sin(angle) * graveyardDistance; // Calculate the Z position based on the angle and the graveyard distance
        Vector3 finalPosition = new Vector3(churchPosition.x + xOffset, 0, churchPosition.z + zOffset); // Get the terrain height
        float terrainY = terrain.SampleHeight(finalPosition); // Get the terrain height
        float finalY = terrainY + terrain.transform.position.y; // Apply an offset and the terrain height
        finalPosition.y = finalY; // Apply the Y fix offset
        return finalPosition; // Return the final position
    }

    float GetBoundingRadius(GameObject prefab) // Method to calculate the bounding radius of a prefab
    {
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero); // Initialize new bounds
        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();  // Get all renderers in the prefab
        if (renderers.Length > 0) // Check if there are renderers
        {
            bounds = renderers[0].bounds; // Initialize the bounds with the first renderer

            for (int i = 1; i < renderers.Length; i++) // Iterate for each renderer
            {
                bounds.Encapsulate(renderers[i].bounds); // Add each renderer's bounds to the original bounds
            }
        }
        else // If no renderers, checks for colliders instead
        {
            Collider[] colliders = prefab.GetComponentsInChildren<Collider>();  // Get all colliders in the prefab
            if (colliders.Length > 0) // Checks if there are colliders
            {
                bounds = colliders[0].bounds; // Initialize the bounds with the first collider
                for (int i = 1; i < colliders.Length; i++) // Iterate for each collider
                {
                    bounds.Encapsulate(colliders[i].bounds); // Add each collider's bounds to the original bounds
                }
            }
            else
            { // If no renderers or colliders are found
                Debug.LogError("No renderers or colliders found in " + prefab.name); // Log an error if no renderers or colliders found
                return 0.5f;  // Returns a default value if no colliders or renderers found
            }
        }
        return Mathf.Max(bounds.extents.x, bounds.extents.z); // Returns the max extents of the bounds
    }

    void CachePrefabRadii() // Method to cache the radii of the prefabs
    {
        if (churchPrefab != null && !prefabRadii.ContainsKey(churchPrefab)) // Check if churchPrefab exists and if it is already on the list
            prefabRadii.Add(churchPrefab, GetBoundingRadius(churchPrefab)); // Add churchPrefab and its radius to the dictionary
        if (graveyardPrefab != null && !prefabRadii.ContainsKey(graveyardPrefab)) // Check if graveyardPrefab exists and if it is already on the list
            prefabRadii.Add(graveyardPrefab, GetBoundingRadius(graveyardPrefab)); // Add graveyardPrefab and its radius to the dictionary
        if (jumpPadPrefab != null && !prefabRadii.ContainsKey(jumpPadPrefab)) // Check if jumpPadPrefab exists and if it is already on the list
            prefabRadii.Add(jumpPadPrefab, GetBoundingRadius(jumpPadPrefab)); // Add jumpPadPrefab and its radius to the dictionary
        if (hutPrefab != null && !prefabRadii.ContainsKey(hutPrefab)) // Check if hutPrefab exists and if it is already on the list
            prefabRadii.Add(hutPrefab, GetBoundingRadius(hutPrefab)); // Add hutPrefab and its radius to the dictionary
        if (treePrefabs != null) // Check if treePrefabs exist
        {
            foreach (GameObject prefab in treePrefabs) // Loop through each tree prefab
            {
                if (!prefabRadii.ContainsKey(prefab)) // Check if the prefab exists in prefabRadii
                    prefabRadii.Add(prefab, GetBoundingRadius(prefab)); // Add tree prefab and its radius to the dictionary
            }
        }
        if (rockPrefabs != null) // Check if rockPrefabs exist
        {
            foreach (GameObject prefab in rockPrefabs) // Loop through each rock prefab
            {
                if (!prefabRadii.ContainsKey(prefab)) // Check if the prefab exists in prefabRadii
                    prefabRadii.Add(prefab, GetBoundingRadius(prefab)); // Add rock prefab and its radius to the dictionary
            }
        }
    }

    #endregion 
}
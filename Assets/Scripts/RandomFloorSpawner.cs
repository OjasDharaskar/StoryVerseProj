using UnityEngine;

public class RandomFloorSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    [Tooltip("The prefab to spawn (e.g., Mask, Coin, Enemy)")]
    public GameObject itemPrefab; // Renamed for reusability

    [Tooltip("The exact tag used on your floor objects")]
    public string floorTag = "Spawn";

    [Tooltip("How many collectibles to spawn on EACH floor")]
    public int itemsPerFloor = 3;

    [Tooltip("How high above the floor should it spawn?")]
    public float heightOffset = 0.5f;

    [Header("Hierarchy Settings")]
    [Tooltip("If true, spawned items are parented to the floor. Keeps the hierarchy clean.")]
    public bool parentToFloor = true;

    private void Start()
    {
        // Safety Check: Prevent the game from crashing if you forgot to assign the prefab
        if (itemPrefab == null)
        {
            Debug.LogError("Spawner: Item Prefab is not assigned! Disabling script.");
            return;
        }

        SpawnOnFloors();
    }

    private void SpawnOnFloors()
    {
        GameObject[] floors = GameObject.FindGameObjectsWithTag(floorTag);

        // Safety Check: Warn the user if no floors were found
        if (floors.Length == 0)
        {
            Debug.LogWarning($"Spawner: No floors found with the tag '{floorTag}'.");
            return;
        }

        foreach (GameObject floor in floors)
        {
            Collider floorCollider = floor.GetComponent<Collider>();

            if (floorCollider == null)
            {
                Debug.LogWarning($"Spawner: Floor '{floor.name}' is missing a collider! Skipping.");
                continue;
            }

            // PERFORMANCE: Cache the bounds outside the loop so Unity doesn't 
            // recalculate them repeatedly for every single spawned item.
            Bounds bounds = floorCollider.bounds;

            // Determine the parent transform to keep the scene hierarchy clean
            Transform parentTransform = parentToFloor ? floor.transform : null;

            for (int i = 0; i < itemsPerFloor; i++)
            {
                float randomX = Random.Range(bounds.min.x, bounds.max.x);
                float randomZ = Random.Range(bounds.min.z, bounds.max.z);
                float spawnY = bounds.max.y + heightOffset;

                Vector3 spawnPosition = new Vector3(randomX, spawnY, randomZ);

                // Instantiate with the parent attached
                Instantiate(itemPrefab, spawnPosition, Quaternion.identity, parentTransform);
            }
        }
    }
}
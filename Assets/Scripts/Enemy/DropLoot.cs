//--------------------------------------------------------------------------------------------------
//  Description: Handles dropping loot items on object destruction.
//               Chooses a random loot based on its drop chance from a list of loot items.
//--------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropLoot : MonoBehaviour
{
    #region Variables

    public GameObject droppedItemPrefab;   // Prefab of the item to be dropped
    public float spawnHeightOffset = 1f;   // Offset for spawning the loot above the origin.
    public float dropForce = 300f;         // Force applied when the loot is dropped.
    public List<Loot> lootList = new List<Loot>();    // List of loot items to choose from.

    #endregion

    #region Loot Drop Logic

    Loot GetDroppedItem() /// Selects a loot item to drop based on random numbers and drop chances from loot list
    {
        int randomNumber = Random.Range(1, 101); // From 1 to 100
        List<Loot> possibleItems = new List<Loot>(); // List of possible items
        foreach (Loot item in lootList)
        {
            if (randomNumber < item.dropChance)
            {
                possibleItems.Add(item);
            }
        }
        if (possibleItems.Count > 0)
        {
            Loot droppedItem = possibleItems[Random.Range(0, possibleItems.Count)];
            return droppedItem;
        }
        return null;
    }

    public void SpawnLoot(Vector3 spawnPosition)  /// Spawns the selected loot item when called.
    {
        Loot droppedItem = GetDroppedItem();
        if (droppedItem != null)
        {
            Vector3 spawnPositionOffset = spawnPosition + Vector3.up * spawnHeightOffset; // Add offset to spawn position
            GameObject lootGameObject = Instantiate(droppedItem.lootPrefab, spawnPositionOffset, Quaternion.identity);
            // Animations and other stuff after spawning
            Vector3 dropDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            lootGameObject.GetComponent<Rigidbody>().AddForce(dropDirection * dropForce, ForceMode.Impulse); // Add force and direction to drop
        }
    }

    #endregion
}
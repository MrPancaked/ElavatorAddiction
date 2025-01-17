using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropLoot : MonoBehaviour
{
    public GameObject droppedItemPrefab;
    public float spawnHeightOffset = 1f;
    public float dropForce = 300f;
    public List<Loot> lootList = new List<Loot>();
   

    Loot GetDroppedItem()
    {
        int randomNumber = Random.Range(1, 101); // From 1 to 100
        List<Loot> possibleItems = new List<Loot>();
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

    public void SpawnLoot(Vector3 spawnPosition)
    {
        Loot droppedItem = GetDroppedItem();
        if(droppedItem != null)
        {
            Vector3 spawnPositionOffset = spawnPosition + Vector3.up * spawnHeightOffset;
            GameObject lootGameObject = Instantiate(droppedItem.lootPrefab, spawnPositionOffset, Quaternion.identity);
            // Animations and other stuff after spawning
            Vector3 dropDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            lootGameObject.GetComponent<Rigidbody>().AddForce(dropDirection * dropForce, ForceMode.Impulse);
        }
    }

}

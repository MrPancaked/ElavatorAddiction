//--------------------------------------------------------------------------------------------------
//  Description: Represents loot items. ScriptableObject to define loot properties.
//--------------------------------------------------------------------------------------------------
using UnityEngine;

[CreateAssetMenu(fileName = "Loot", menuName = "Items/Loot")]
public class Loot : ScriptableObject
{
    #region Variables
    public GameObject lootPrefab; // Prefab of the loot item.
    public string lootName;       // Name of the loot item.
    public int dropChance;        // Drop chance of the loot item, from 1 to 100.

    #endregion

    #region Constructor
    public Loot(string lootName, int dropChance)
    {
        this.lootName = lootName;
        this.dropChance = dropChance;
    }
    #endregion
}
//--------------------------------------------------------------------------------------------------
//  Description: Stores all the scene settings
//--------------------------------------------------------------------------------------------------
using UnityEngine;

[CreateAssetMenu(fileName = "Scene", menuName = "Items/Scene")]
public class SceneSettings : ScriptableObject
{
    #region Variables

    [Header("Settings")]
    public string sceneName; // IT SHOULD HAVE THE EXACT SCENE NAME PLEASE YOU FUKCING MORON
    public string backgroundColor = "#FFFFFF";
    public string fogColorHex = "#FFFFFF";
    public float fogStartDistance = 3f;
    public float fogEndDistance = 30f;
    public float lightIntensity = 0f;
    public bool hasSkybox = false;

    #endregion
}

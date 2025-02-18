//--------------------------------------------------------------------------------------------------
// Description: Keeps the game object alive through scene transitions, and makes sure there is only one.
//--------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyBetweenScenes : MonoBehaviour
{
    #region Variables

    public static DontDestroyBetweenScenes instance;
    public static DontDestroyBetweenScenes Instance { get { return instance; } }

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

    #endregion
}
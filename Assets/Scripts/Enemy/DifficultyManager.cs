using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DifficultyManager : MonoBehaviour
{
    [Header("References")]
    public TextMeshPro roomText;

    // Private variables
    [HideInInspector] public int RoomIndex = 0;
    private static DifficultyManager instance;
    public static DifficultyManager Instance { get { return instance; } }


    #region Unity Methods

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            UpdateRoomIndex();
        }
    }

    #endregion

    #region Room Index

    public void AddRoomIndex()
    {
        RoomIndex++;
        UpdateRoomIndex();
    }

    public void ResetRoomIndex()
    {
        RoomIndex = 0;
        UpdateRoomIndex();
    }

    public void UpdateRoomIndex()
    {
        roomText.text = (-RoomIndex).ToString();
    }

    #endregion
}

//--------------------------------------------------------------------------------------------------
// Description: Updates the health UI based on the health of the associated object.
//--------------------------------------------------------------------------------------------------
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class HealthUI : MonoBehaviour
{
    #region Variables

    public Health health;    /// Reference to the Health component.
    public TextMeshProUGUI healthUI; /// Text element for the health display.
    private float initialHealth; /// Initial health of the object.

    #endregion

    #region Unity Methods

    void Awake()  /// Sets the initial health of the object.
    {
        initialHealth = health.hp; // set the initial health of the object
    }

    void Update()  /// Updates the health UI display.
    {
        healthUI.text = health.hp + " / " + initialHealth; // Update the UI
        if (health.hp <= 0)
        {
            DeathManager.Instance.PlayerDied();
        }
    }

    #endregion
}
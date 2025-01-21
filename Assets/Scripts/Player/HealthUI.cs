//--------------------------------------------------------------------------------------------------
// Description: Updates the health UI based on the health of the associated object.
//--------------------------------------------------------------------------------------------------
using UnityEngine;
using TMPro;

public class HealthUI : MonoBehaviour
{
    #region Variables

    public Health health;    /// Reference to the Health component.
    public TextMeshProUGUI healthUI; /// Text element for the health display.
    public GameObject deathUI; /// Death UI that will be displayed on death
    private float initialHealth; /// Initial health of the object.

    #endregion

    #region Unity Methods

    void Awake()  /// Sets the initial health of the object.
    {
        initialHealth = health.hp; // set the initial health of the object
    }

    #endregion

    #region UI Logic

    void Update()  /// Updates the health UI display.
    {
        healthUI.text = health.hp + " / " + initialHealth; // Update the UI
        if (health.hp <= 0 && !deathUI.activeSelf)
        {
            deathUI.SetActive(true);
        }
    }

    #endregion
}
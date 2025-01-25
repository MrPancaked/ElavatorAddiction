using UnityEngine;
using UnityEngine.UI; // Required for Image
using UnityEngine.SceneManagement;


public class HealthUI : MonoBehaviour
{
    #region Variables

    public Health health;          // Reference to the Health component.
    public Image healthImage;      // Reference to the Image component.
    private float initialHealth;   // Initial health of the object.

    #endregion

    #region Unity Methods

    void Awake()  // Sets the initial health of the object.
    {
        initialHealth = health.hp; // set the initial health of the object
    }

    void Update()  // Updates the health UI display.
    {
        if (healthImage != null)
        {
            healthImage.fillAmount = health.hp / initialHealth; // Update the fill amount
        }

        if (health.hp <= 0)
        {
            DeathManager.Instance.PlayerDied();
        }
    }

    #endregion
}
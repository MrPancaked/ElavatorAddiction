//--------------------------------------------------------------------------------------------------
// Description: Handles muzzle flash effects, including light emission and its deactivation.
//--------------------------------------------------------------------------------------------------
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    #region Variables

    [System.Serializable]
    public class LightSource
    {
        public Light light;
    }
    public LightSource lightSource;

    #endregion

    #region Unity Methods

    void Awake()  /// Initializes the light intensity of muzzle flash.
    {
        Invoke(nameof(DisableLight), 0.05f);
    }

    #endregion

    #region MuzzleFlash Logic

    private void DisableLight() /// Disables the light after the flash duration.
    {
        lightSource.light.enabled = false;
    }

    #endregion
}
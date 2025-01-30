//--------------------------------------------------------------------------------------------------
// Description: 
//--------------------------------------------------------------------------------------------------
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class UISounds : MonoBehaviour
{
    #region Variables

    [Header("Sounds")] /// FMOD event references
    public EventReference hoverSound;
    public EventReference clickSound;

    #endregion

    #region Play Sounds

    public void PlayHoverSound()
    {
        AudioManager.instance.PlayOneShot2D(hoverSound);
    }

    public void PlayPressSound()
    {
        AudioManager.instance.PlayOneShot2D(clickSound);
    }

    #endregion
}
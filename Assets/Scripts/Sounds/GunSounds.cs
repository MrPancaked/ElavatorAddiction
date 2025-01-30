//--------------------------------------------------------------------------------------------------
// Description: 
//--------------------------------------------------------------------------------------------------
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class GunSounds : MonoBehaviour
{
    #region Variables

    [Header("Sounds")] /// FMOD event references
    public EventReference equipSound;
    public EventReference shotgunShotSound;
    public EventReference cockUpSound;
    public EventReference cockDownSound;

    #endregion

    #region Play Sounds

    public void PlayEquipSound()
    {
        AudioManager.instance.PlayOneShot2D(equipSound);
    }

    public void PlayGunshotSound() 
    {
        AudioManager.instance.PlayOneShot2D(shotgunShotSound);
    }

    public void PlayCockUpSound()
    {
        AudioManager.instance.PlayOneShot2D(cockUpSound);
    }

    public void PlayCockDownSound() 
    {
        AudioManager.instance.PlayOneShot2D(cockDownSound);
    }

    #endregion
}
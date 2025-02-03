using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class ButtonsAndLeverSounds : MonoBehaviour
{
    #region Variables

    [Header("Sounds")] /// FMOD event references
    public EventReference buttonSound;
    public EventReference leverDownSound;
    public EventReference leverDownFakeSound;
    public EventReference leverUpSound;

    #endregion

    #region Methods

    public void PlayButtonSound()
    {
        AudioManager.instance.PlayOneShot(buttonSound, this.transform.position);
    }

    public void PlayLeverDownSound() 
    {
        AudioManager.instance.PlayOneShot(leverDownSound, this.transform.position);
    }

    public void PlayLeverDownFakeSound() 
    {
        AudioManager.instance.PlayOneShot(leverDownFakeSound, this.transform.position);
    }

    public void PlayLeverUpSound() 
    {
        AudioManager.instance.PlayOneShot(leverUpSound, this.transform.position);
    }

    #endregion
}
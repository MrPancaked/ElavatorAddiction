using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class DoorsSounds : MonoBehaviour
{
    #region Variables

    [Header("Sounds")]
    public EventReference doorCloseSound;
    public EventReference doorOpenSound;

    #endregion

    #region Play Sounds

    public void PlayDoorCloseSound()
    {
        AudioManager.instance.PlayOneShot(doorCloseSound, this.transform.position);
    }

    public void PlayDoorOpenSound()
    {
        AudioManager.instance.PlayOneShot(doorOpenSound, this.transform.position);
    }

    #endregion
}
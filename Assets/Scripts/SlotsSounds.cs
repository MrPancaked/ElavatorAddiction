using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class SlotsSounds : MonoBehaviour
{
    #region Variables

    [Header("Sounds")]
    public EventReference winSound;
    public EventReference loseSound;
    public EventReference rollSound;
    public EventReference jackpotSound;

    public static SlotsSounds instance;
    public static SlotsSounds Instance { get { return instance; } }

    #endregion

    #region Unity Methods

    private void Awake()
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

    #region Play Sounds

    public void PlayWinSound()
    {
        AudioManager.instance.PlayOneShot(winSound, this.transform.position);
    }

    public void PlayLoseSound()
    {
        AudioManager.instance.PlayOneShot(loseSound, this.transform.position);
    }

    public void PlayRollSound()
    {
        AudioManager.instance.PlayOneShot(rollSound, this.transform.position);
    }

    public void PlayJackpotSound()
    {
        AudioManager.instance.PlayOneShot(jackpotSound, this.transform.position);
    }

    #endregion
}
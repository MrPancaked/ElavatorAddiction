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
    public EventReference hover;
    public EventReference press;
    public EventReference windowOpen;
    public EventReference windowClose;
    public EventReference dialogue;
    private static UISounds instance;
    public static UISounds Instance { get { return instance; } }

    #endregion

    #region Unity Methods

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            //DontDestroyOnLoad(gameObject); // IMPORTANT!
        }
    }

    #endregion

    #region Play Sounds

    public void HoverSound()
    {
        AudioManager.instance.PlayOneShot2D(hover);
    }

    public void PressSound()
    {
        AudioManager.instance.PlayOneShot2D(press);
    }

    public void WindowOpenSound()
    {
        AudioManager.instance.PlayOneShot2D(windowOpen);
    }

    public void WindowCloseSound()
    {
        AudioManager.instance.PlayOneShot2D(windowClose);
    }

    public void DialogueSound()
    {
        AudioManager.instance.PlayOneShot2D(dialogue);
    }

    #endregion
}
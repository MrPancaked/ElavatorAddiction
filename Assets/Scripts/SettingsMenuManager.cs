using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
public class SettingsMenuManager : MonoBehaviour
{
   
   public Slider masterVol;
    public AudioMixer mainAudioMixer;

    public void ChangeMasterVolume()
   {
    mainAudioMixer.SetFloat("MasterVol", masterVol.value);
   }
}

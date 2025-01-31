using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class EnemySounds : MonoBehaviour
{
    #region Variables

    [Header("References")]
    public Transform soundPosition;
    public EventReference spawnSound;
    public EventReference damageSound;
    public EventReference idleSound;
    public EventReference deathSound;
    public EventReference coinsSound;

    private FMOD.Studio.EventInstance idleSoundInstance; // FMOD Instance for loop

    #endregion

    public void OnDisable()
    {
        idleSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        idleSoundInstance.release();
    }

    public void IdleSoundStart()
    {
        idleSoundInstance = AudioManager.instance.CreateInstance(idleSound, soundPosition.position); //Use this function to create the event instance
        idleSoundInstance.start(); // Start the loop sound
    }

    public void UpdateIdleSound()// Update sound emitter position
    {
        FMOD.Studio.PLAYBACK_STATE playbackState;
        idleSoundInstance.getPlaybackState(out playbackState);
        if (playbackState != FMOD.Studio.PLAYBACK_STATE.STOPPED)
        {
            AudioManager.instance.Set3DAttributes(idleSoundInstance, soundPosition); // Set the 3D attributes
        } 
    }

    public void DamageSound()
    {
        AudioManager.instance.PlayOneShot(damageSound, soundPosition.position); // Play enemy damage sound
    }

    public void CoinsSound()
    {
        AudioManager.instance.PlayOneShot(deathSound, soundPosition.position); // Play enemy damage sound
        AudioManager.instance.PlayOneShot(coinsSound, soundPosition.position);
    }

    public void DeathSound()
    {
        AudioManager.instance.PlayOneShot(deathSound, soundPosition.position); // Play enemy damage sound
    }
}

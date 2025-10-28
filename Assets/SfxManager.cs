using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxManager : MonoBehaviour
{
    public static SfxManager instance;

    public AudioSource soundFXObject;

    // Some audio clips are tricky to get into some classes, so we store them / play them from here.
    public List<string> clipNames;
    public AudioClip[] clipsInOrder;

    // When we want to start/stop audio loops we use this
    private Dictionary<string, AudioSource> activeLoops;

    private static float globalSFXVolume = 1f; // 0 - 1

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        activeLoops = new Dictionary<string, AudioSource>();
    }

    private void OnEnable()
    {
        // From a settings menu, adjust all volumes?
    }

    private void OnDisable()
    {
    }

    public void adjustGlobalSFXVolume(float newPct)
    {
        globalSFXVolume = newPct;
        // TODO Play a sound effect
    }

    // Play typical audio clip
    public void playSFX(AudioClip audioClip, float volumeLevel)
    {
        AudioSource audioSource;
        audioSource = Instantiate(soundFXObject);

        audioSource.clip = audioClip;
        audioSource.volume = volumeLevel * globalSFXVolume;
        audioSource.Play();
        float clipLength = audioSource.clip.length;

        GameObject.Destroy(audioSource.gameObject, clipLength);
    }

    // Play an audio clip specified in this manager's local list
    public void playSFXbyName(string clipName, float volumeLevel)
    {
        AudioSource audioSource;
        audioSource = Instantiate(soundFXObject);

        audioSource.clip = clipsInOrder[clipNames.IndexOf(clipName)];
        audioSource.volume = volumeLevel * globalSFXVolume;
        audioSource.Play();
        float clipLength = audioSource.clip.length;

        GameObject.Destroy(audioSource.gameObject, clipLength);
    }

    // Audio loop - will play continuously until manually stopped
    public void beginSFXLoop(string nameOfLoop, AudioClip audioClip, Transform spawnHere, float volumeLevel)
    {
        AudioSource audioSource;
        audioSource = Instantiate(soundFXObject);


        if (!activeLoops.ContainsKey(nameOfLoop))
        {
            activeLoops.Add(nameOfLoop, audioSource);

            audioSource.loop = true;
            audioSource.clip = audioClip;
            audioSource.volume = volumeLevel * globalSFXVolume;
            audioSource.Play();
        }
        else
        {
            GameObject.Destroy(audioSource.gameObject);
            Debug.LogWarning($"Couldn't create looping SFX {nameOfLoop}: That loop is already running?");
        }
    }

    public void endSFXLoop(string nameOfLoop)
    {
        if (activeLoops.ContainsKey(nameOfLoop))
        {
            GameObject.Destroy(activeLoops[nameOfLoop].gameObject);
            activeLoops.Remove(nameOfLoop);
        }
    }
}
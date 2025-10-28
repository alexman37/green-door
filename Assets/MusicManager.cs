using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    // We have two instances - one for music tracks and another for ambient noise
    public static MusicManager mainInstance;
    public static MusicManager ambientInstance;
    public bool mainMusicPlayer;

    public AudioSource audioSource;

    public List<string> clipNames;
    public AudioClip[] musicTracks;

    bool fadingOut;
    bool fadingIn;
    float fadeTimer;

    private float globalMusicVolume;
    private float fadeMultiplier;

    private void Awake()
    {
        if (mainMusicPlayer && mainInstance == null)
        {
            mainInstance = this;

            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();

            fadingOut = false;
            fadingIn = false;
            fadeTimer = 0;
            globalMusicVolume = 1f;
            fadeMultiplier = 1f;
        } else if (!mainMusicPlayer && ambientInstance == null)
        {
            ambientInstance = this;

            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();

            fadingOut = false;
            fadingIn = false;
            fadeTimer = 0;
            globalMusicVolume = 1f;
            fadeMultiplier = 1f;
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnEnable()
    {
        // From a settings menu, adjust all volumes?
    }

    private void OnDisable()
    {
    }

    // In-game music handler
    public void inGameMusicFade(bool fadeOut)
    {
        // Fade out, stop playing altogether
        if (fadeOut)
        {
            fadingOut = true;
            fadingIn = false;
        }
        // Fade in, continue playing at previous levels
        else
        {
            fadingIn = true;
            fadingOut = false;
        }
    }

    public void adjustGlobalMusicVolume(float newPct)
    {
        globalMusicVolume = newPct;
        // Since there's only one we just set it here
        audioSource.volume = globalMusicVolume * fadeMultiplier;
    }

    private void Update()
    {
        // So needlessly complicated because we cant run coroutines from static methods...sigh...
        if (fadingOut)
        {
            fadeTimer = fadeTimer + Time.deltaTime;
            if (fadeTimer >= 0.3f)
            {
                fadeTimer = fadeTimer % 0.3f;
                fadeMultiplier -= 0.1f;
            }

            if (fadeMultiplier <= 0.02f)
            {
                fadingOut = false;
                fadeMultiplier = 0;
            }
            audioSource.volume = globalMusicVolume * fadeMultiplier;
        }

        else if (fadingIn)
        {
            fadeTimer = fadeTimer + Time.deltaTime;

            if (fadeTimer >= 0.3f)
            {
                fadeTimer = fadeTimer % 0.3f;
                fadeMultiplier += 0.1f;
            }

            if (fadeMultiplier >= 1)
            {
                fadingIn = false;
                fadeMultiplier = 1;
            }
            audioSource.volume = globalMusicVolume * fadeMultiplier;
        }
    }

    // Play music track
    public void playTrack(AudioClip clip, float vol)
    {
        globalMusicVolume = vol;
        audioSource.volume = globalMusicVolume;
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void playTrackByName(string n, float vol)
    {
        globalMusicVolume = vol;
        audioSource.volume = globalMusicVolume;
        audioSource.clip = musicTracks[clipNames.IndexOf(n)];
        audioSource.Play();
    }

    public void fadeOutCurrent()
    {
        fadingOut = true;
    }

    public void stopMusic()
    {
        audioSource.Stop();
    }
}
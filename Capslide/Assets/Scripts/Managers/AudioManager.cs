﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Audio Manager Singleton
    public static AudioManager Instance { get; private set; }

    [Header("Audio"), Space(8)]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource altSource;
    public Music[] backgroundMusic;
    public SFX[] soundEffects;


    [Header("Volume Settings"), Space(8)]
    [Range(0f, 1f)]
    public float masterVolume;
    [Range(0f, 1f)]
    public float musicVolume;
    [Range(0f, 1f)]
    public float sfxVolume;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
            Debug.LogWarning($"WARNING: There can only be one instance of this class.");
        }
    }

    /// <summary>
    /// Play sound effect (SFX).
    /// </summary>
    public void PlaySFX(string name, float volume = 1f)
    {
        if (!GameManager.Instance.sfxON)
            return;

        SFX sfx = Array.Find(soundEffects, sound => sound.name == name);
        if (!SoundAvailable(sfx))
            return;
        
        sfxSource.PlayOneShot(sfx.sound, volume);
    }

    /// <summary>
    /// Play single sound effect (SFX).
    /// </summary>
    public void PlaySingleSFX(string name, float volume = 1f)
    {
        if (!GameManager.Instance.sfxON)
            return;

        SFX sfx = Array.Find(soundEffects, sound => sound.name == name);
        if (!SoundAvailable(sfx))
            return;

        if (!sfxSource.isPlaying)
            sfxSource.PlayOneShot(sfx.sound, volume);
    }
    /// <summary>
    /// Play alternative sound effect (SFX).
    /// </summary>
    public void PlayAltSFX(string name, float volume = 1f, float pitch = 1f)
    {
        if (!GameManager.Instance.sfxON)
            return;

        SFX sfx = Array.Find(soundEffects, sound => sound.name == name);
        if (!SoundAvailable(sfx))
            return;

        altSource.pitch = pitch;
        if (!altSource.isPlaying)
            altSource.PlayOneShot(sfx.sound, volume);
    }
    /// <summary>
    /// Play a random sound effect (SFX) from a specified list of sound effects.
    /// </summary>
    public void PlayRandomSFX(params string[] sfxNames)
    {
        int argumentCount = sfxNames.Length;
        string selectedSFX = sfxNames[UnityEngine.Random.Range(0, argumentCount)];
        PlaySFX(selectedSFX);
    }
    /// <summary>
    /// Play background music.
    /// </summary>
    public void PlayMusic(string name, float volume = 1f, float pitch = 1f)
    {
        if (!GameManager.Instance.bgmON)
            return;

        Music msc = Array.Find(backgroundMusic, m => m.name == name);
        if (!MusicAvailable(msc))
            return;

        musicSource.volume = volume;
        musicSource.pitch = pitch;
        musicSource.clip = msc.music;
        musicSource.Play();
    }

    public IEnumerator FadeOut(float rate)
    {
        while (musicSource.volume > 0f)
        {
            musicSource.volume -= rate;
            yield return new WaitForEndOfFrame();
        }
    }
    public IEnumerator FadeIn(float rate, float volume = 1f)
    {
        while (musicSource.volume < volume)
        {
            musicSource.volume += rate;
            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// Toggle background music.
    /// </summary>
    public void ToggleMusic()
    {
        musicSource.enabled = GameManager.Instance.bgmON;

        if (!musicSource.isPlaying)
            PlayMusic("Main Theme");
    }

    /// <summary>
    /// Mute all audio.
    /// </summary>
    public void MuteAudio()
    {
        musicSource.volume = 0f;
        sfxSource.volume = 0f;
        altSource.volume = 0f;
    }

    /// <summary>
    /// Unmute all audio.
    /// </summary>
    public void UnmuteAudio()
    {
        musicSource.volume = 0.5f;
        sfxSource.volume = 1f;
        altSource.volume = 1f;
    }

    /// <summary>
    /// Stop playing music.
    /// </summary>
    public void Stop(string name)
    {
        Music msc = Array.Find(backgroundMusic, m => m.name == name);
        if (!MusicAvailable(msc))
            return;

        musicSource.Stop();
    }
    /// <summary>
    /// Check if sound effect (SFX) is available in the Audio Manager.
    /// </summary>
    private bool SoundAvailable(SFX sfx)
    {
        if (sfx != null)
            return true;

        Debug.LogError($"ERROR: Sound effect {sfx.name} not found!");
        return false;
    }
    /// <summary>
    /// Check if music is available in the Audio Manager.
    /// </summary>
    private bool MusicAvailable(Music msc)
    {
        if (msc != null)
            return true;

        Debug.LogError($"ERROR: Music {msc.name} not found!");
        return false;
    }

    /// <summary>
    /// Set the music volume.
    /// </summary>
    /// <param name="volume">The volume to change</param>
    public void SetMusicVolume(float volume) => musicSource.volume = volume;
}

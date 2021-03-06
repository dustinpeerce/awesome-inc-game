﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {


    // Static Instance
    public static AudioManager Instance;


    // Public Audio Sources
    public AudioSource musicAudioSource;
    public AudioSource audioClipAudioSource;


    // Public Audio Clips
    [Header("UI Audio Clips")]
    public AudioClip sfxMenuSelect;
    public AudioClip sfxHighlight;

    [Header("Game Manager Audio Clips")]
    public AudioClip sfxSelectItem;
    public AudioClip sfxPoint;
    public AudioClip sfxLose; 
    public AudioClip sfxEscape;

    [Header("Game Hub Audio Clips")]
    public AudioClip sfxDoor;

    [Header("Shop 1 Audio Clips")]
    public AudioClip sfxDealCard;
    public AudioClip sfxUncoverCard;
    public AudioClip sfxNoPair;

    [Header("Shop 2 Audio Clips")]
    
   

    [Header("Shop 3 Audio Clips")]
    public AudioClip sfxCollision;

    [Header("Shop 4 Audio Clips")]
    public AudioClip sfxFlagTile;
    public AudioClip sfxSelectMine;


    // Private
    private float musicVolume = 1;
    private float audioClipVolume = 1;



    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            musicVolume = PlayerPrefs.GetFloat(GameVals.Settings.musicVolumeKey, 1);
            audioClipVolume = PlayerPrefs.GetFloat(GameVals.Settings.audioClipVolumeKey, 1);
        }
        else {
            Destroy(gameObject);
        }
    }


    public void PlayAudioClip(AudioClip clip) {
        audioClipAudioSource.PlayOneShot(clip, audioClipVolume);
    }


}

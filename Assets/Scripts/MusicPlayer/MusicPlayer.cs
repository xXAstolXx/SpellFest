using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioMixer audioMixer;
    [SerializeField]
    private String audioMixerExposedParameter;
    [SerializeField]
    private float durationForFadeOut;
    [SerializeField]
    private float targetVolume;
    [SerializeField]
    private List<AudioClip> audioClips;
    [SerializeField]
    private int elementToPlay;
    [SerializeField]
    private bool playRandom;
    [SerializeField]
    private bool playFadeOut;
    [SerializeField, Range(0, 1)]
    private float volume;
    private AudioSource audioSource;


    private CircleCollider2D circleCollider2D;

    private void Awake()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = volume;
        if(playRandom == true)
        {
            PlayRandom();
        }
        else
        {
            Play(elementToPlay);
        }
    }

    private void Play(int index)
    {
        audioSource.clip = audioClips[index - 1];
        audioSource.Play();
    }

    private void PlayRandom()
    {
        int r = Random.Range(0, audioClips.Count);
        AudioClip clip = audioClips[r];
        audioSource.clip = clip;
        audioSource.Play();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(playFadeOut == true)
        {
            FadeOut(audioMixer, audioMixerExposedParameter, durationForFadeOut, targetVolume);
        }
    }

    private void FadeOut(AudioMixer audioMixer, String exposedParameter, float duration, float targetVolume)
    {
        StartCoroutine(FadeMixerGroup.StartFade(audioMixer, exposedParameter, duration, targetVolume));
    }
}

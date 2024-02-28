using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SFXSlider : MonoBehaviour
{
    Slider slider;

    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();
    }

    public void SetVolume(AudioMixer sfxMixer,float volume)
    {
        sfxMixer.SetFloat("sfxVolume", volume);
    }
}

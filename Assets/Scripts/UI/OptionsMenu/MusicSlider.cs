using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MusicSlider : MonoBehaviour
{
    Slider slider;

    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();
    }

    public void SetVolume(AudioMixer musicMixer, float volume)
    {
        musicMixer.SetFloat("musicVolume", volume);
    }
}

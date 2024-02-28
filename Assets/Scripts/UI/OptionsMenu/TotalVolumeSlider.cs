using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class TotalVolumeSlider : MonoBehaviour
{
    Slider slider;

    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();
    }

    public void SetVolume(AudioMixer masterMixer, float volume)
    {
        masterMixer.SetFloat("masterVolume", volume);
    }
}

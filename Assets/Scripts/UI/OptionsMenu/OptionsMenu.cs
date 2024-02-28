using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    FullscreenToogle fullscreenToogle;
    bool isFullscreen;

    ResolutionDropdown resolutionDropdown;

    SFXSlider sfxSlider;
    MusicSlider musicSlider;
    TotalVolumeSlider totalVolumeSlider;
    [SerializeField]
    AudioMixer masterMixer;
    [SerializeField]
    AudioMixer sfxMixer;
    [SerializeField]
    AudioMixer musicMixer;

    [SerializeField]
    private PauseMenu pauseMenu;

    private void Awake()
    {
        fullscreenToogle = GetComponentInChildren<FullscreenToogle>();
        isFullscreen = Screen.fullScreen;

        resolutionDropdown = GetComponentInChildren<ResolutionDropdown>();

        sfxSlider = GetComponentInChildren<SFXSlider>();
        musicSlider = GetComponentInChildren<MusicSlider>();
        totalVolumeSlider = GetComponentInChildren<TotalVolumeSlider>();
    }

    private void Start()
    {
        StartToogleText(isFullscreen);
        gameObject.SetActive(false);
    }

    private void StartToogleText(bool isFullscreen)
    {
       fullscreenToogle.UpdateText(isFullscreen);
    }

    public void OnToogleClicked(bool fullScreen)
    {
        fullscreenToogle.SetFullscreen(fullScreen);
    }

    public void SetResolution(int resolutionIndex)
    {
        resolutionDropdown.SetResolution(resolutionIndex);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSlider.SetVolume(sfxMixer, volume);
    }

    public void SetMusicVolume(float volume)
    {
        musicSlider.SetVolume(musicMixer, volume);
    }

    public void SetTotalVolume(float volume)
    {
        totalVolumeSlider.SetVolume(masterMixer, volume);
    }

    public void OnBackBtnClicked()
    {
        gameObject.SetActive(false);
    }

    public void OnBackBtnClickedInGame()
    {
        gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(true);
    }

    public void Initialize()
    {
        gameObject.SetActive(true);
    }
}

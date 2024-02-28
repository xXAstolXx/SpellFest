using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PauseMenu : MonoBehaviour
{
    ResumeBtn resumeBtn;
    OptionsBtn optionsBtn;
    [SerializeField]
    OptionsMenu optionsMenu;
    MainMenuBtn mainMenuBtn;

    //public UnityEvent OnPause { get; private set; } = new UnityEvent();
    public UnityEvent OnResume { get; private set; } = new UnityEvent();



    private void Awake()
    {
        resumeBtn = GetComponentInChildren<ResumeBtn>();
        optionsBtn = GetComponentInChildren<OptionsBtn>();
        mainMenuBtn = GetComponentInChildren<MainMenuBtn>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void ShowPopUp()
    {
        gameObject.SetActive(true);
        PauseGame();
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
    }

    private void UnPauseGame()
    {
        Time.timeScale = 1f;
    }

    public void OnMainMenuBtnClicked(int index)
    {
        mainMenuBtn.LoadMainMenuScene(index);
    }

    public void OnOptionsBtnClicked()
    {
        optionsBtn.OpenOptionsMenu(optionsMenu.gameObject);
        gameObject.SetActive(false);
    }

    public void OnResumeBtnClicked()
    {
        resumeBtn.ResumeGame();
        OnResume.Invoke();
        ClosePopUp();
    }

    public void ClosePopUp()
    {
        UnPauseGame();
        gameObject.SetActive(false);
    }
}

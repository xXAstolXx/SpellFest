using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class LoadSceneByIndex : MonoBehaviour
{
    [SerializeField]
    PlayableDirector director;

    int chosenIndex = -1;
    bool loadingStarted = false;
    [SerializeField]
    MainMenu mainMenu;

    public void PlayFadeOut()
    {
        if (mainMenu.PlayTutorial()) { chosenIndex = 1; }
        if (!mainMenu.PlayTutorial()) { chosenIndex = 2; }

        Time.timeScale = 1f;
        if (!loadingStarted)
        {
            loadingStarted = true;
            director.Play();
        }
    }
    public void PlayFadeOutInt(int index)
    {
        Time.timeScale = 1f;
        if (!loadingStarted)
        {
            loadingStarted = true;
            chosenIndex = index;
            director.Play();
        }
    }

    public void LoadScene()
    {
        if(chosenIndex > -1) SceneManager.LoadScene(chosenIndex);

    }
}

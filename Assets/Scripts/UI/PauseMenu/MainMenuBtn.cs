using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuBtn : MonoBehaviour
{
    public void LoadMainMenuScene(int index)
    {
        SceneManager.LoadScene(index);
    }
}

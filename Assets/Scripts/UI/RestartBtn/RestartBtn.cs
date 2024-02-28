using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestartBtn : MonoBehaviour
{
    [SerializeField]
    private int levelToRestart;

    public void OnRestartClicked()
    {
        ResumeGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
    }


}

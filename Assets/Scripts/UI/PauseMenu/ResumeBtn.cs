using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResumeBtn : MonoBehaviour
{
    public void ResumeGame()
    {
        UnPauseGame();
    }

    private void UnPauseGame()
    {
        Time.timeScale = 1f;
    }

}

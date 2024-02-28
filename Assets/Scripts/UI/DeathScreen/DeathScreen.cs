using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    public bool isActive { get; private set; } = false;
    private RandomDeathMessages deathMessages;

    private void Start()
    {
        deathMessages = GetComponent<RandomDeathMessages>();
        gameObject.SetActive(false);
    }
    public void OnPlayerDeath()
    {
        isActive = true;
        if(isActive != false)
        {
            Debug.Log("ui onplayerdeath");
            gameObject.SetActive(true);
            deathMessages.DisplayRandomDeathMessage();
            PauseGame();

        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
    }
}

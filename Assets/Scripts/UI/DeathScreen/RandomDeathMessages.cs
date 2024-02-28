using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomDeathMessages : MonoBehaviour
{
    [SerializeField]
    private TMP_Text deathTxt;
    [SerializeField]
    private string[] deathMessages;

    private bool showOnceDeathMessages = false;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            DisplayRandomDeathMessage();
        }
    }

    public void DisplayRandomDeathMessage()
    {
        if (showOnceDeathMessages == false)
        {
            if (deathMessages.Length > 0)
            {
                int randomIndex = Random.Range(0, deathMessages.Length);
                deathTxt.text = deathMessages[randomIndex];
                showOnceDeathMessages = true;
            }
            else
            {
                deathTxt.text = "Dafault";
                showOnceDeathMessages = true;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextBox : MonoBehaviour
{
    private TMP_Text textBox;

    private void Awake()
    {
        textBox = GetComponentInChildren<TMP_Text>();
    }

    public void SetText(string text)
    {
        if(text == string.Empty)
        {
            textBox.text = "Ups something went wrong ";
        }
        textBox.text = text;
    }

    public void SetText(char charToSet)
    {
        textBox.text += charToSet;
    }
}

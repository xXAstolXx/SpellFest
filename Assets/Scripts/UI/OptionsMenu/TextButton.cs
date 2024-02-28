using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextButton : MonoBehaviour
{
    TMP_Text txt;
    [SerializeField]
    Color onColor = Color.green;
    [SerializeField]
    Color offColor = Color.red;
    private void Awake()
    {
        txt = GetComponentInChildren<TMP_Text>();
    }


    public void SetText(string textToSet)
    {
        txt.text = textToSet;
    }

    public void SetTextColor(bool isFullscreen)
    {
        if (isFullscreen) { txt.color = onColor; }
        else { txt.color = offColor; }
    }
}

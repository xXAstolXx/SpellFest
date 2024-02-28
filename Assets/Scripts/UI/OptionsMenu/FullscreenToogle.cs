using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FullscreenToogle : MonoBehaviour
{
    TextButton textButton;

    private void Awake()
    {
        textButton = GetComponentInChildren<TextButton>();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        UpdateTextButton(isFullscreen);
    }

    public void UpdateText(bool isFullscreen)
    {
        UpdateTextButton(isFullscreen);
    }

    private void UpdateTextButton(bool isFullscreen)
    {
        if (isFullscreen) { textButton.SetText("On"); }
        else { textButton.SetText("Off"); }
        textButton.SetTextColor(isFullscreen);
    }
}

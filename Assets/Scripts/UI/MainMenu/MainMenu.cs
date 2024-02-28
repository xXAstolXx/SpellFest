using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{

    OptionsMenu optionsMenu;
    [SerializeField]
    bool playTutorial = true;
    [SerializeField]
    bool unlockedArenaMode = true;

    public bool PlayTutorial()
    {
        if(playTutorial)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool UnlockedArenaMode()
    {
        if (unlockedArenaMode) { return true; }
        else { return false; }
    }

    private void Awake()
    {
        optionsMenu = GetComponentInChildren<OptionsMenu>();
    }

    public void OnOptionsButtonClicked()
    {
        optionsMenu.Initialize();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuStartBtn : MonoBehaviour
{
    [SerializeField]
    private PopUpMenu popUp;
    

    public void OpenGameModeSelection()
    {
        popUp.PopUp();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosePopUpBtn : MonoBehaviour
{
    [SerializeField]
    private GameObject popUpToClose;

    public void ClosePopUp()
    {
        popUpToClose.SetActive(false);
    }
}

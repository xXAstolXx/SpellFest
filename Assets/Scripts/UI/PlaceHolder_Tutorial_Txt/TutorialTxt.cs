using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialTxt : MonoBehaviour
{
    private TMP_Text tutText;

    private void Awake()
    {
        tutText = GetComponent<TMP_Text>();
        tutText.enabled = false;
    }

    public void ShowTutorialTxt(bool shouldDisplay)
    {
        if(shouldDisplay == true )
        {
            tutText.enabled = true;
        }
        else if(shouldDisplay == false)
        {
            tutText.enabled = false;
        }
    }
}

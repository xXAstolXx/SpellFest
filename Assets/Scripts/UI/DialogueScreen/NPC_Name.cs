using TMPro;
using UnityEngine;

public class NPC_Name : MonoBehaviour
{
    private TMP_Text nameTxt;

    private void Awake()
    {
        nameTxt = GetComponentInChildren<TMP_Text>();
    }

    public void SetName(string name)
    {
        if(name == string.Empty)
        {
            nameTxt.text = "Something went Wrong";
        }
        nameTxt.text = name;
    }
}

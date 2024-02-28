using TMPro;
using UnityEngine;

public class SpellTxt : MonoBehaviour
{
    private TMP_Text spellTxt;

    private void Awake()
    {
        spellTxt = GetComponent<TMP_Text>();
    }

    public void UpdateSpellTxt(string textInput)
    {
        spellTxt.text = "Active Spell: " + textInput; 
    }
}

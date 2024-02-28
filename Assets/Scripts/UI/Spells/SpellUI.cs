using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellUI : MonoBehaviour
{
    [SerializeField]
    private Spell fireSpell;
    [SerializeField]
    private Spell iceSpell;

    public void UpdateSpellUI(SpellInput spell)
    {
        if(spell == SpellInput.FIRE)  //FIRE
        {
            fireSpell.HightlightSpell(true);
            iceSpell.HightlightSpell(false);
        }
        else if(spell == SpellInput.ICE) //ICE
        {
            iceSpell.HightlightSpell(true);
            fireSpell.HightlightSpell(false);
        }
    }
}

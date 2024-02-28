using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerGraphic : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> graphicObjects;

    public int activeGraphic = 1;
    [SerializeField]
    float blinkTime;
    bool isInvincible = false;
    bool isInvis = false;

    private SpellInput activeSpell;


    private void Awake()
    {
        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
            {
                graphicObjects[i].SetActive(true);
            }
            else
            {
                graphicObjects[i].SetActive(false);
            }
        }
        UpdateSpellGraphic(SpellInput.FIRE);
    }

    public void SetGraphic(int setter)
    {
        for (int i = 0; i < 4; i++)
        {
            if (i == setter)
            {
                graphicObjects[i].SetActive(true);
            }
            else
            {
                graphicObjects[i].SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (isInvincible)
        {
            if (!isInvis)
            {
                StartCoroutine(BlinkTimer(blinkTime));
            }
        }
    }

    public void UpdateGraphicMoveStates(Vector2 moveVector)
    {
        if (moveVector.magnitude != 0f)
        {
            if (moveVector.y < 0)
            {
                activeGraphic = 1;
            }
            else if (moveVector.x < 0 && moveVector.y == 0)
            {
                activeGraphic = 2;
            }
            else if (moveVector.x > 0 && moveVector.y == 0)
            {
                activeGraphic = 0;
            }
            else
            {
                activeGraphic = 3;
            }

            for (int i = 0; i < 4; i++)
            {
                if (activeGraphic == i)
                {
                    graphicObjects[i].SetActive(true);
                }
                else
                {
                    graphicObjects[i].SetActive(false);
                }
            }

            var graphicAnimator = graphicObjects[activeGraphic].GetComponent<Animator>();
            graphicAnimator.SetBool("isWalking", true);

            if (activeSpell == SpellInput.FIRE)
            {
                //Fire Spell
                graphicAnimator.SetBool("Fire", true);
                graphicAnimator.SetBool("Ice", false);
            }
            else if (activeSpell == SpellInput.ICE)
            {
                //Ice Spell
                graphicAnimator.SetBool("Fire", false);
                graphicAnimator.SetBool("Ice", true);
            }
        }
        else
        {
            var graphicAnimator = graphicObjects[activeGraphic].GetComponent<Animator>();
            graphicAnimator.SetBool("isWalking", false);
        }

    }

    public void SetInvincible()
    {
        isInvincible = true;
        isInvis = false;
    }

    public void SetVincible()
    {
        StopCoroutine("BlinkTimer");
        isInvincible = false;
        isInvis = false;
        SetVisibilty(true);
    }

    private void SetVisibilty(bool value)
    {
        foreach(GameObject obj in graphicObjects)
        {
            obj.GetComponent<SpriteRenderer>().enabled = value;
        }
    }

    private IEnumerator BlinkTimer(float duration)
    {
        isInvis = true;
        SetVisibilty(false);

        yield return new WaitForSeconds(duration);
        SetVisibilty(true);
        yield return new WaitForSeconds(duration);
        isInvis = false;
    }

    public void UpdateSpellGraphic(SpellInput spell)
    {
        activeSpell = spell;
        var graphicAnimator = graphicObjects[activeGraphic].GetComponent<Animator>();


        if (activeSpell == SpellInput.FIRE)
        {
            //Fire Spell
            graphicAnimator.SetBool("Fire", true);
            graphicAnimator.SetBool("Ice", false);
        }
        else if (activeSpell == SpellInput.ICE)
        {
            //Ice Spell
            graphicAnimator.SetBool("Fire", false);
            graphicAnimator.SetBool("Ice", true);
        }
    }
}

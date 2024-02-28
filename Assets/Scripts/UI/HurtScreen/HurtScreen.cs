using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HurtScreen : MonoBehaviour
{
    [SerializeField]
    float showTime;

    Image graphic;

    Animator animator;

    private void Awake()
    {
        graphic = GetComponentInChildren<Image>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        graphic.enabled = false;
    }


    public void ShowHurtScreen()
    {
        if(graphic.enabled == false)
        {
            graphic.enabled = true;
        }
        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        
        animator.SetBool("fadeIn", true);
        yield return new WaitForSeconds(showTime);
        animator.SetBool("fadeIn", false);
    }
}

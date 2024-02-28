using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC_Icon : MonoBehaviour
{
    [SerializeField]
    private Sprite gorbatIcon;

    [SerializeField]
    private Sprite fireWispIcon;
    [SerializeField]
    private Sprite iceWispIcon;

    private Image icon;
    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        icon = GetComponentInChildren<Image>();
    }

    public void SetIcon(NPCType type)
    {
        switch (type)
        {
            case NPCType.GORBAT:
                icon.sprite = gorbatIcon;
                break;
            case NPCType.FIREWISP:
                icon.sprite = fireWispIcon;
                animator.SetBool("FireWispTalking", true);
                break;
            case NPCType.ICEWISP:
                icon.sprite = iceWispIcon;
                animator.SetBool("IceWispTalking", true);
                break;
        }
    }
}

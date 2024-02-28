using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueScreen : MonoBehaviour
{
    private TextBox textBox;

    private NPC_Name npcName;

    private NPC_Icon npcIcon;

    private bool isActive = false;

    private Animator animator;

    private void Awake()
    {
        textBox = GetComponentInChildren<TextBox>();
        npcName = GetComponentInChildren<NPC_Name>();
        npcIcon = GetComponentInChildren<NPC_Icon>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        gameObject.SetActive(isActive);
    }

    public void ShowWindow()
    {
        isActive = true;
        gameObject.SetActive(isActive);
        animator.SetBool("isOpen", isActive);
    }

    public void CloseWindow()
    {
        isActive = false;
        animator.SetBool("isOpen", isActive);
        gameObject.SetActive(isActive);
       
    }

    public void SetNPCIcon(NPCType type)
    {
        npcIcon.SetIcon(type);
    }

    public void SetNPCName(string nameToSet)
    {
        npcName.SetName(nameToSet);
    }

    public void SetTextInTextBox(string textToSet)
    {
        textBox.SetText(textToSet);
    }

    public void SetTextInTextBox(char charToSet)
    {
        textBox.SetText(charToSet);
    }
}

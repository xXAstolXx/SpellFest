using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class DialogueSystem : MonoBehaviour
{
    private Queue<string> sentences;
    private bool isDialogueActive;
    bool isTyping = false;
    string currentSentence;

    [SerializeField,Range(0.01f, 0.09f),Tooltip(" going to 0.01 makes the Text go faster otherdirection it makes it slower!")]
    private float displayLetterTime = 0.05f;

    public UnityEvent OnStartTalking { get; private set; } = new UnityEvent();
    public UnityEvent OnStopTalking { get; private set; } = new UnityEvent();
    public UnityEvent OnEndDialogue { get; private set; } = new UnityEvent();



    void Start()
    {
        sentences = new Queue<string>();
    }


    public void StartDialogue(Dialogue dialogue)
    {
        isDialogueActive = true;
        UI.Instance.dialogueScreen.ShowWindow();
        UI.Instance.dialogueScreen.SetNPCName(dialogue.name);
        UI.Instance.dialogueScreen.SetNPCIcon(dialogue.NPCType);
        
        sentences.Clear();

        foreach(string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (isDialogueActive)
        {
            if (sentences.Count == 0)
            {
                EndDialogue();
                return;
            }
            if (isTyping)
            {
                UI.Instance.dialogueScreen.SetTextInTextBox(currentSentence);
                isTyping = false;
                OnStopTalking.Invoke();
                StopAllCoroutines();
            }
            else
            {
                currentSentence = sentences.Dequeue();
                StopAllCoroutines();
                StartCoroutine(TypeSentence(currentSentence));
            }
        }
    }

    IEnumerator TypeSentence(string sentence)
    {
        OnStartTalking.Invoke();
        isTyping = true;
        UI.Instance.dialogueScreen.SetTextInTextBox("");
        foreach(char letter in sentence.ToCharArray())
        {
            UI.Instance.dialogueScreen.SetTextInTextBox(letter);
            yield return new WaitForSeconds(displayLetterTime);
        }
        isTyping = false;
        OnStopTalking.Invoke();
    }

    public void EndDialogue()
    {
        UI.Instance.dialogueScreen.CloseWindow();
        isDialogueActive = false;
        OnStopTalking.Invoke();
        OnEndDialogue.Invoke();
    }
}

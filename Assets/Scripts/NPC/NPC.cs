using System;
using System.Collections;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField]
    protected Dialogue dialogue;

    protected bool dialogueShowed = false;
    protected bool isPlayerInDialogue = false;
    protected bool isPlayerInRange;
    protected bool isTalking = false;
    protected bool isPausing = false;

    protected InteractScreen interactSreen;

    protected AudioSource audioSource;

    [Header("SoundOptions")]
    [SerializeField]
    protected bool usePitch = false;
    [SerializeField, Range(-3f, 0f)]
    protected float minRandomPitchRange;
    [SerializeField, Range(0, 3f)]
    protected float maxRandomPitchRange;
    [SerializeField]
    protected float chanceforSubSound;
    Player player;


    protected void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        interactSreen = GetComponentInChildren<InteractScreen>();
        if(usePitch)
        {
            RandomPitch();
        }
    }

    protected virtual void Start()
    {
        Game.Instance.dialogueSystem.OnStartTalking.AddListener(OnStartTalk);
        Game.Instance.dialogueSystem.OnStopTalking.AddListener(OnStopTalk);

    }

    protected void Update()
    {
        if (isTalking)
        {
            if (!audioSource.isPlaying)
            {
                //if (isPausing == true)
                //{
                //    StartCoroutine(RandomPause());
                //}
                
               PlaySound();
                
            }
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        player = collision.transform.parent.gameObject.GetComponent<Player>();
        isPlayerInRange = true;
        player.isInDialogue = true;

        if (dialogueShowed == false)
        {
            Debug.Log("enter 1");

            player.OnAdvanceDialogue.AddListener(RestartDialogue);
            TriggerDialogue();
            dialogueShowed = true;
            isPlayerInDialogue = true;
            interactSreen.ClosePopUp();
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        isPlayerInRange = false;
        isPlayerInDialogue = false;
        Game.Instance.dialogueSystem.EndDialogue();
        interactSreen.PopUp();
        player = collision.transform.parent.gameObject.GetComponent<Player>();
        player.isInDialogue = false;
    }

    private void TriggerDialogue()
    {
        Game.Instance.dialogueSystem.StartDialogue(dialogue);
    }

    private void RestartDialogue()
    {
        if(isPlayerInDialogue == false && isPlayerInRange)
        {
            isPlayerInDialogue = true;
            TriggerDialogue();
            interactSreen.ClosePopUp();
        }
    }

    void OnStartTalk()
    {
        isTalking = true;
        PlaySound();
    }

    void OnStopTalk()
    {
        isTalking = false;
    }

    private void PlaySound()
    {
        float value = UnityEngine.Random.Range(0f,1f);
        if(value <= chanceforSubSound)
        {
            audioSource.clip = GetSubTalkingSound();
        }
        else
        {
            audioSource.clip = GetMainTalkingSound();
        }
        audioSource.Play();
        //isPausing = true;
    }

    private AudioClip GetMainTalkingSound()
    {
        switch (dialogue.NPCType)
        {
            case NPCType.GORBAT:
                return GlobalSound.Instance.GorbatSound.TalkingSoundMain;
            case NPCType.FIREWISP:
                return GlobalSound.Instance.WispsSound.TalkingSoundMain;
            case NPCType.ICEWISP:
                return GlobalSound.Instance.WispsSound.TalkingSoundMain;
        }
        return GlobalSound.Instance.GorbatSound.TalkingSoundMain;
    }
    private AudioClip GetSubTalkingSound()
    {
        switch (dialogue.NPCType)
        {
            case NPCType.GORBAT:
                return GlobalSound.Instance.GorbatSound.TalkingSoundSub;
            case NPCType.FIREWISP:
                return GlobalSound.Instance.WispsSound.TalkingSoundSub;
            case NPCType.ICEWISP:
                return GlobalSound.Instance.WispsSound.TalkingSoundSub;
        }
        return GlobalSound.Instance.GorbatSound.TalkingSoundSub;
    }

    private void RandomPitch()
    {
        float randomPitch = UnityEngine.Random.Range(minRandomPitchRange, maxRandomPitchRange);
        audioSource.pitch = randomPitch;
    }

    //IEnumerator RandomPause()
    //{
    //    float duration = UnityEngine.Random.Range(0f, 0.5f);
    //    yield return new WaitForSeconds(0);
    //    isPausing = false;
    //}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPC_Sound", menuName = "NPC/Sound")]
public class NPCSound : ScriptableObject
{
    [SerializeField]
    AudioClip talkingSoundMain;
    [SerializeField]
    AudioClip talkingSoundSub;

    public AudioClip TalkingSoundMain { get => talkingSoundMain;  }
    public AudioClip TalkingSoundSub { get => talkingSoundSub;  }

}

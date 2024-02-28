using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game_Sound", menuName = "Game/Sound")]
public class GameSound : ScriptableObject
{
    [SerializeField]
    AudioClip checkPointActivated;
    [SerializeField]
    AudioClip arenaStartet;
    [SerializeField]
    AudioClip arenaCleared;

    public AudioClip CheckPointActivated { get => checkPointActivated; }
    public AudioClip ArenaCleared { get => arenaCleared; }
    public AudioClip ArenaStartet { get => arenaStartet; }
}

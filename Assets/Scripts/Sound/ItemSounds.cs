using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_Sound", menuName = "Item/Sound")]
public class ItemSounds : ScriptableObject
{
    [SerializeField]
    AudioClip healSound;
    [SerializeField]
    AudioClip levelUpSound;

    public AudioClip HealSound { get => healSound; }
    public AudioClip LevelUpSound { get => levelUpSound; }

}

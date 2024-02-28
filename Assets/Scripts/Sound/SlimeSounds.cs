using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Slime_Sound", menuName = "Enemies/Sound/Slime")]
public class SlimeSounds : EnemySound
{
    [Header("DeathSounds")]
    [SerializeField]
    private AudioClip basicSlimeDeathSound;
    [SerializeField]
    private AudioClip fireSlimeDeathSound;
    [SerializeField]
    private AudioClip iceSlimeDeathSound;
    [Header("WalkingSound")]
    [SerializeField]
    private AudioClip basicSlimeWalkingSound;    
    [SerializeField]
    private AudioClip fireSlimeWalkingSound;    
    [SerializeField]
    private AudioClip iceSlimeWalkingSound;


    public AudioClip BasicSlimeWalkingSound { get => basicSlimeWalkingSound; }
    public AudioClip FireSlimeWalkingSound { get => fireSlimeWalkingSound; }
    public AudioClip IceSlimeWalkingSound { get => iceSlimeWalkingSound; }

    public AudioClip BasicSlimeDeathSound { get => basicSlimeDeathSound; }
    public AudioClip FireSlimeDeathSound { get => fireSlimeDeathSound; }
    public AudioClip IceSlimeDeathSound { get => iceSlimeDeathSound; }
}

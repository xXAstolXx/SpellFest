using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player_Sound", menuName = "Player/Sound")]
public class PlayerSound : ScriptableObject
{
    [SerializeField]
    private AudioClip walkingSound;
    [SerializeField]
    private AudioClip startChargeSound;
    [SerializeField]
    private AudioClip mediumChargeSound;
    [SerializeField]
    private AudioClip decreaseChargeSound;
    [SerializeField]
    private AudioClip deathSound;
    [SerializeField]
    private AudioClip windBlastSound;
    [SerializeField]
    private AudioClip fireShootSound;
    [SerializeField]
    private AudioClip iceShootSound;
    [SerializeField]
    private AudioClip hurtSound;

    public AudioClip WalkingSound { get => walkingSound;  }
    public AudioClip StartChargeSound { get => startChargeSound; }
    public AudioClip MediumChargeSound { get => mediumChargeSound; }
    public AudioClip DecreaseChargeSound { get => decreaseChargeSound; }   
    public AudioClip DeathSound { get => deathSound; }
    public AudioClip WindBlastSound { get => windBlastSound; }
    public AudioClip FireShootSound { get => fireShootSound; }
    public AudioClip IceShootSound { get => iceShootSound; }
    public AudioClip HurtSound { get => hurtSound; }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_Sound", menuName = "Enemies/Sound")]
public class EnemySound : ScriptableObject
{
    [SerializeField]
    protected AudioClip idleSound;
    [SerializeField]
    protected AudioClip attackSound;

    [SerializeField]
    protected AudioClip hurtSound;

    [SerializeField]
    protected AudioClip deathSound;

    public AudioClip IdleSound { get { return idleSound; } }
    public AudioClip AttackSound { get { return attackSound; } }
    public AudioClip HurtSound { get { return hurtSound; } }
    public AudioClip DeathSound { get { return deathSound; } }

}

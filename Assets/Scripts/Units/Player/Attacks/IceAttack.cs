using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IceAttack : ElementAttack
{
    [SerializeField]
    float stunDuration;
    public float StunDuration { get { return stunDuration; } }
    [SerializeField]
    float unstunableDuration;
    public float UnstunableDuration { get { return unstunableDuration; } }
    [SerializeField]
    float damageAmplifier;
    public float DamageAmplifier { get { return damageAmplifier; } }
}

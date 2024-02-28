using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyVFX : MonoBehaviour
{
    [SerializeField]
    ParticleSystem freezeEnd;
    public ParticleSystem FreezeEnd { get => freezeEnd; }
    [SerializeField]
    ParticleSystem freezeCancel;
    public ParticleSystem FreezeCancel { get => freezeCancel; }
    [SerializeField]
    ParticleSystem explodeOnDeath;
    public ParticleSystem ExplodeOnDeath => explodeOnDeath;
    [SerializeField]
    ParticleSystem burn;
    public ParticleSystem Burn => burn;

    public UnityEvent OnDeathfinished { get; private set; } = new UnityEvent();


    private void OnParticleSystemStopped()
    {
        OnDeathfinished.Invoke();
    }
}

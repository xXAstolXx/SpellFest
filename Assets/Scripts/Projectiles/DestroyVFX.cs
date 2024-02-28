using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DestroyVFX : MonoBehaviour
{
    public UnityEvent OnparticleSystemFinished { get; private set; } = new UnityEvent();


    private void OnParticleSystemStopped()
    {
        OnparticleSystemFinished.Invoke();
    }
}

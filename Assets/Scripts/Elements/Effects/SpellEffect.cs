using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellEffect : MonoBehaviour
{
    [SerializeField]
    ElementType type;
    public ElementType Type => type;
    [SerializeField]
    EffectType effectType;
    public EffectType EffectType => effectType;

    [SerializeField]
    protected float duration;


    public abstract void Apply(Unit unit);

    protected abstract IEnumerator Timer(Unit unit);
}

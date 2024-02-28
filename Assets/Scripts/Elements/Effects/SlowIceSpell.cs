using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowIceSpell : SpellEffect
{
    [SerializeField]
    float speedModifier;


    public override void Apply(Unit unit)
    {
        bool isSlowed = false;
        foreach (var effect in unit.spellEffects)
        {
            if (effect.EffectType == EffectType.SLOW_ICEATTACK)
            {
                isSlowed = true;
            }
        }

        if (isSlowed == false)
        {
            AddEffect(unit);
        }

        unit.spellEffects.Add(this);
        StartCoroutine(Timer(unit));
    }

    protected override IEnumerator Timer(Unit unit)
    {
        yield return new WaitForSeconds(duration);
        unit.spellEffects.Remove(this);

        if (unit)
        {
            bool isSlowed = false;
            foreach (var effect in unit.spellEffects)
            {
                if (effect.EffectType == EffectType.SLOW_ICEATTACK)
                {
                    isSlowed = true;
                }
            }

            if (isSlowed == false)
            {
                RemoveEffect(unit);
            }
        }
    }

    public void AddEffect(Unit unit)
    {
        unit.unitMovement.ModifySpeed(speedModifier);
    }

    public void RemoveEffect(Unit unit)
    {
        unit.unitMovement.ResetSpeed(speedModifier);
    }
}

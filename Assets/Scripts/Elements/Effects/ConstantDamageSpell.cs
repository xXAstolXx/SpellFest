using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantDamageSpell : SpellEffect
{
    [SerializeField]
    float dpsEnemy;
    [SerializeField]
    float intervalDamagePlayer;


    public override void Apply(Unit unit)
    {
        bool isBurning = false;
        foreach (var effect in unit.tileEffects)
        {
            if (effect.EffectType == EffectType.BURNING)
            {
                isBurning = true;
            }
        }
        foreach (var effect in unit.spellEffects)
        {
            if (effect.EffectType == EffectType.BURNING)
            {
                isBurning = true;
            }
        }

        if (isBurning == false)
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
            bool isBurning = false;
            foreach (var effect in unit.tileEffects)
            {
                if (effect.EffectType == EffectType.BURNING)
                {
                    isBurning = true;
                }
            }
            foreach (var effect in unit.spellEffects)
            {
                if (effect.EffectType == EffectType.BURNING)
                {
                    isBurning = true;
                }
            }

            if (isBurning == false)
            {
                RemoveEffect(unit);
            }
        }
    }

    protected void AddEffect(Unit unit)
    {
        unit.GetComponentInChildren<SpriteRenderer>().material.SetInt("_IsBurning", 1);

        if (unit.gameObject.GetComponent<Enemy>())
        {
            unit.health.ApplyDps(dpsEnemy);
        }
        else
        {
            unit.health.ApplyIntervalDamage(intervalDamagePlayer);
        }
    }

    protected void RemoveEffect(Unit unit)
    {
        unit.GetComponentInChildren<SpriteRenderer>().material.SetInt("_IsBurning", 0);
        if (unit.gameObject.GetComponent<Enemy>())
        {
            unit.health.RemoveDps(dpsEnemy);
        }
        else
        {
            unit.health.RemoveIntervalDamage(intervalDamagePlayer);
        }
    }
}

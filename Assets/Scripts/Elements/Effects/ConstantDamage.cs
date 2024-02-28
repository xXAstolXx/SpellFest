using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ConstantDamage : Effect
{
    [SerializeField]
    float dpsEnemy;
    [SerializeField]
    float intervalDamagePlayer;


    public override void Apply(Unit unit)
    {
        bool isBurning = false;
        foreach (var effect in unit.spellEffects)
        {
            if (effect.EffectType == EffectType.BURNING)
            {
                isBurning = true;
            }
        }
        if (isBurning == false)
        {
            AddValues(unit);
        }
    }

    public override void Remove(Unit unit)
    {
        bool isBurning = false;
        foreach (var effect in unit.spellEffects)
        {
            if (effect.EffectType == EffectType.BURNING)
            {
                isBurning = true;
            }
        }
        if (isBurning == false)
        {
            RemoveValues(unit);
        }
    }

    private void AddValues(Unit unit)
    {
        //unit.GetComponentInChildren<SpriteRenderer>().material.SetInt("_IsBurning", 1);
        if (unit.GetComponentInChildren<PlayerVFX>())
        {
            unit.GetComponentInChildren<PlayerVFX>().StartBurn();
        }
        else
        {
            unit.GetComponentInChildren<EnemyVFX>().Burn.Play();
        }

        if (unit.gameObject.GetComponent<Enemy>())
        {
            unit.health.ApplyDps(dpsEnemy);
        }
        else
        {
            unit.health.ApplyIntervalDamage(intervalDamagePlayer);
        }
    }

    private void RemoveValues(Unit unit)
    {
        //unit.GetComponentInChildren<SpriteRenderer>().material.SetInt("_IsBurning", 0);
        if (unit.GetComponentInChildren<PlayerVFX>())
        {
            unit.GetComponentInChildren<PlayerVFX>().StopBurn();
        }
        else
        {
            unit.GetComponentInChildren<EnemyVFX>().Burn.Stop();
        }

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

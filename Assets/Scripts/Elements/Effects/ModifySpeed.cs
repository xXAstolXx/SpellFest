using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ModifySpeed : Effect
{
    [SerializeField]
    float speedModifier;


    public override void Apply(Unit unit)
    {
        unit.unitMovement.ModifySpeed(speedModifier);
    }

    public override void Remove(Unit unit)
    {
        unit.unitMovement.ResetSpeed(speedModifier);
    }
}

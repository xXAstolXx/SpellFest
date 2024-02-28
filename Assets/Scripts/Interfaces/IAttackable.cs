using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    public abstract bool ReceiveAttack(float damage, AttackType damageType, float angle, Vector2 force, GameObject source);

    public abstract bool ReceiveAttack(float damage, AttackType damageType, GameObject source);
}

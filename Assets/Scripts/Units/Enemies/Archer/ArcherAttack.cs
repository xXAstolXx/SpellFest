using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ArcherAttack", menuName = "EnemyAttacks/ArcherAttack")]
public class ArcherAttack : ScriptableObject
{
    [SerializeField]
    float speed;
    public float Speed { get { return speed; } }

    [SerializeField]
    float chargeTime;
    public float ChargeTime { get { return chargeTime; } }

    [SerializeField]
    float impactDamage;
    public float ImpactDamage { get { return impactDamage; } }

    [SerializeField]
    float knockBackStrength;
    public float KnockBackStrength { get { return knockBackStrength; } }
}
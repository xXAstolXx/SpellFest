using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    List<float> speed;
    public List<float> Speed {  get { return speed; } }

    [SerializeField]
    List<float> impactDamage;
    public List<float> ImpactDamage { get {  return impactDamage; } }

    [SerializeField]
    AttackType type;
    public AttackType Type { get { return type; } }

    [SerializeField]
    List<float> enemyKnockBackStrength;
    public List<float> EnemyKnockBackStrength { get {  return enemyKnockBackStrength; } }

    [SerializeField, Range(0, 1f)]
    float shakeStrength;
    public float ShakeStrength { get { return shakeStrength; } }
}

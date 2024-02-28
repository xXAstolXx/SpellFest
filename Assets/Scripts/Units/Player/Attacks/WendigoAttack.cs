using System.Collections.Generic;
using UnityEngine;


public class WendigoAttack : MonoBehaviour
{
    [SerializeField]
    ElementType elementType;
    public ElementType ElementType { get { return elementType; } }
    [SerializeField]
    List<float> speed;
    public List<float> Speed { get { return speed; } }

    [SerializeField]
    List<float> impactDamage;
    public List<float> ImpactDamage { get { return impactDamage; } }

    [SerializeField]
    AttackType type;
    public AttackType Type { get { return type; } }

    [SerializeField]
    List<float> enemyKnockBackStrength;
    public List<float> EnemyKnockBackStrength { get { return enemyKnockBackStrength; } }

    [SerializeField, Range(0, 1f)]
    float shakeStrength;
    public float ShakeStrength { get { return shakeStrength; } }

    [SerializeField, Tooltip("Each added element increases the tileradius by 1.")]
    List<float> tileSpawnChance;
    public List<float> TileSpawnChance { get {  return tileSpawnChance; } }

    [SerializeField]
    List<float> aoeRadius;
    public List<float> AoeRadius { get {  return aoeRadius; } }

    [SerializeField]
    float tileSpawnDelay;
    public float TileSpawnDelay { get {  return tileSpawnDelay; } }
}

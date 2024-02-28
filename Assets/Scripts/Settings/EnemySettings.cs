using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EnemySettings", menuName = "Global Settings/Enemies")]
public class EnemySettings : ScriptableObject
{
    [SerializeField]
    Color enemyReceiveDamageColor;
    public Color EnemyReceiveDamageColor => enemyReceiveDamageColor;

    [SerializeField]
    float enemyReceiveDamageColorTime;
    public float EnemyReceiveDamageColorTime => enemyReceiveDamageColorTime;

    [SerializeField]
    float damageAmpSameElement;
    public float DamageAmpSameElement => damageAmpSameElement;
    [SerializeField]
    float damageAmpOppositeElement;
    public float DamageAmpOppositeElement => damageAmpOppositeElement;

    [Header("Enemy material")]
    [SerializeField]
    float dissolveTime;
    public float DissolveTime => dissolveTime;
    [SerializeField]
    float dissolveStrength;
    public float DissolveStrength => dissolveStrength;
    [SerializeField]
    Color dissolveColor;
    public Color DissolveColor => dissolveColor;

    [SerializeField]
    Color spawnColor;
    public Color SapwnColor => spawnColor;
}

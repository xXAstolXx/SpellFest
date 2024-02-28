using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ElementAttack : PlayerAttack
{
    [SerializeField]
    ElementType elementType;
    public ElementType ElementType { get { return elementType; } }

    [SerializeField]
    List<float> maxDistance;
    public List<float> MaxDistance { get { return maxDistance; } }

    [SerializeField, Tooltip("Each added element increases the tileradius by 1.")]
    List<float> tileSpawnChance;
    public List<float> TileSpawnChance { get {  return tileSpawnChance; } }

    [SerializeField]
    List<float> aoeRadius;
    public List<float> AoeRadius { get {  return aoeRadius; } }

    [SerializeField]
    float tileSpawnDelay;
    public float TileSpawnDelay { get {  return tileSpawnDelay; } }

    [SerializeField]
    List<GameObject> effects;
    public List<GameObject> Effects => effects;
}

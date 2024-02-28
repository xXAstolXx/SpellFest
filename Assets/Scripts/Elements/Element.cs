using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Element : ScriptableObject
{
    public ElementType type;

    [SerializeField]
    List<Effect> effects;
}

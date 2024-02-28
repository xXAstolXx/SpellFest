using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementSettings
{
    public List<Element> elements;

    public Dictionary<ElementTransform, ElementType> transformElements = new Dictionary<ElementTransform, ElementType>()
    { { new ElementTransform(ElementType.FIRE, ElementType.ICE), ElementType.WATER
},
        { new ElementTransform(ElementType.FIRE, ElementType.WATER), ElementType.STEAM},
        { new ElementTransform(ElementType.WATER, ElementType.FIRE), ElementType.STEAM},
        { new ElementTransform(ElementType.ICE, ElementType.FIRE), ElementType.WATER},
        { new ElementTransform(ElementType.FIRE, ElementType.STEAM), ElementType.FIRE},
        { new ElementTransform(ElementType.STEAM, ElementType.FIRE), ElementType.FIRE},
        { new ElementTransform(ElementType.ICE, ElementType.WATER), ElementType.ICE},
        { new ElementTransform(ElementType.WATER, ElementType.ICE), ElementType.ICE},
        { new ElementTransform(ElementType.ICE, ElementType.STEAM), ElementType.ICE},
        { new ElementTransform(ElementType.STEAM, ElementType.ICE), ElementType.STEAM},
        { new ElementTransform(ElementType.WATER, ElementType.STEAM), ElementType.STEAM},
        { new ElementTransform(ElementType.STEAM, ElementType.WATER), ElementType.STEAM},
    };
}

public struct ElementTransform
{
    ElementType attackType;
    ElementType targetType;

    public ElementTransform(ElementType attackType, ElementType targetType)
    {
        this.attackType = attackType;
        this.targetType = targetType;
    }
}

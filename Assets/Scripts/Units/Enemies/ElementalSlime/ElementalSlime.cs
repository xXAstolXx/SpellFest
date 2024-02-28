using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ElementalSlime : Slime
{
    protected override void Initialize()
    {
        base.Initialize();

        foreach(var type in typeToIgnore)
        {
            ignoredElementTypes.Add(type);
        }
    }
}

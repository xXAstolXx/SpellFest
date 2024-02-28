using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Slide : Effect
{
    public override void Apply(Unit unit)
    {
        unit.StartSliding();
    }

    public override void Remove(Unit unit)
    {
        unit.StopSliding();
    }
}

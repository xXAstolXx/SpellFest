using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointGraphic : MonoBehaviour
{
    public void FinishPeting()
    {
        transform.parent.GetComponent<CheckPoint>().FinishPeting();
    }
}

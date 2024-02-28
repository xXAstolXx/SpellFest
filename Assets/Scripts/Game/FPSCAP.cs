using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCAP : MonoBehaviour
{
    [SerializeField]
    private int fPS = 60;
    private void Start()
    {
        Application.targetFrameRate = fPS;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GlobalData
{
    public int currentCheckPoint;
    public float spawnPositionX;
    public float spawnPositionY;
    public bool iceSpellUpgraded;
    public bool fireSpellUpgraded;


    public void Initialize()
    {
        currentCheckPoint = 0;
        iceSpellUpgraded = false;
        fireSpellUpgraded = false;
    }
}

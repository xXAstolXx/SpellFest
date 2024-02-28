using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JsonReadWriteSystem
{
    public void SaveGlobalData(GlobalData globalData)
    {
        string json = JsonUtility.ToJson(globalData);
        File.WriteAllText(Application.dataPath + "/GlobalDataFile.json", json);
    }

    public void LoadGlobalData()
    {
        string json = File.ReadAllText(Application.dataPath + "/GlobalDataFile.json");
        JsonUtility.FromJsonOverwrite(json, Game.Instance.globalData);
    }
}

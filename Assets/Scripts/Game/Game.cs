using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    static Game instance;

    public static Game Instance
    {
        get { return instance; }
    }

    [SerializeField]
    bool isTestRun = false;

    public GlobalData globalData;

    const string globalDataFileName = "GlobalData.dat";
    string globalDataPath;

    public CameraShake cameraShake { get; private set; } 
    public ScreenFreeze screenFreeze { get; private set; }

    public DialogueSystem dialogueSystem { get; private set;}


    private void Awake()
    {
        instance = this;
        cameraShake = GetComponentInChildren<CameraShake>();
        screenFreeze = GetComponentInChildren<ScreenFreeze>();
        dialogueSystem = GetComponentInChildren<DialogueSystem>();

        globalDataPath = Application.persistentDataPath + globalDataFileName;

        if (SceneManager.GetActiveScene().buildIndex < 2 || isTestRun)
        {
            globalData = new GlobalData();
            globalData.Initialize();
            SaveGlobalData();
        }
        else
        {
            LoadGlobalData();
        }
    }

    void LoadGlobalData()
    {
        Debug.Log("file exists: " + Directory.Exists(globalDataPath));
        Debug.Log(globalDataPath);
        globalData = (GlobalData)SerializeData.Load(globalDataPath);
        Debug.Log("checkpoint: "+globalData.currentCheckPoint);
        Debug.Log("firespellupgraded: " + globalData.fireSpellUpgraded);
    }

    public void SaveGlobalData()
    {
        Debug.Log("Save global data: file exists: " + Directory.Exists(globalDataPath));
        SerializeData.Save(globalData, globalDataPath);
        Debug.Log("checkpoint: " + globalData.currentCheckPoint);
        Debug.Log("firespellupgraded: " + globalData.fireSpellUpgraded);
    }
}

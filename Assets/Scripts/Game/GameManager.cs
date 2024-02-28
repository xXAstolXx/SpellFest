using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region singleton
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("There is no object with a LevelManager script.");
                return null;
            }
            else
            {
                return instance;
            }
        }
    }
    #endregion

    private const string startScenePath = "Scenes/Menus/MainMenu";

    private SceneLoader sceneLoader;


    LevelData levelData;

    private Coroutine pauseTimer;


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found another object with a GameManager script.");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        Initialize();

        StartGame();
    }

    private void Initialize()
    {
        sceneLoader = new SceneLoader();
    }

    private void StartGame()
    {
        sceneLoader.LoadFirstScene(startScenePath);
    }

    public void ChangeScene(int scene)
    {
        sceneLoader.ChangeScene(scene);
    }

    public void PauseGame(float duration)
    {
        StopCoroutine(pauseTimer);
        pauseTimer = StartCoroutine(PauseTimer(duration));
    }

    public IEnumerator PauseTimer(float duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSeconds(duration);
        Time.timeScale = 1;
    }
}

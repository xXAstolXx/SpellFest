using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    #region singleton
    private static LevelManager instance;
    public static LevelManager Instance
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

    LevelData levelData;


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found another object with a LevelManager script.");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void InitializePlayer(Player player)
    {

    }
}

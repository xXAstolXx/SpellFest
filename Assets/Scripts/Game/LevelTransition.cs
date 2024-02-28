using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    [SerializeField]
    private int levelToLoad;
    private CapsuleCollider2D capsuleCollider2D;

    private void Awake()
    {
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SceneManager.LoadScene(levelToLoad);
        JsonReadWriteSystem jsonReadWriteSystem = new JsonReadWriteSystem();
        GlobalData globalData = Game.Instance.globalData;
        globalData.currentCheckPoint = 0;
        jsonReadWriteSystem.SaveGlobalData(globalData);
    }
}

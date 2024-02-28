using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private int sceneToLoad;


    public void LoadFirstScene(string startScenePath)
    {
        SceneManager.LoadScene(startScenePath, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByPath(startScenePath));
    }

    public void ChangeScene(int sceneToLoad)
    {
        this.sceneToLoad = sceneToLoad;
        AsyncOperation asyncUnloadOperation = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        asyncUnloadOperation.completed += LoadNextScene;
    }

    private void LoadNextScene(AsyncOperation asyncOperation)
    {
        asyncOperation.completed -= LoadNextScene;
        asyncOperation = null;

        AsyncOperation asyncLoadOperation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        asyncOperation.completed += SetLoadedSceneActive;
    }

    private void SetLoadedSceneActive(AsyncOperation asyncOperation)
    {
        asyncOperation.completed -= SetLoadedSceneActive;
        asyncOperation = null;

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneToLoad));
    }
}

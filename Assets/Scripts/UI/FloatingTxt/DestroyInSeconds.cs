using UnityEngine;

public class DestroyInSeconds : MonoBehaviour
{
    [SerializeField]
    private float secondsToDestroy = 1f;
    

    public void DestroyGameObject()
    {
        Destroy(gameObject, secondsToDestroy);
    }
}

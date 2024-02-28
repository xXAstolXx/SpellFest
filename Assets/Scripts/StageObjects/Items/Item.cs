using UnityEngine;

public class Item : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"collision: {collision} ");
        Activate(collision);
    }
    protected virtual void Activate(Collider2D collision)
    {
        Destroy(gameObject);
    }
}

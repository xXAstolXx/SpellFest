using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionChecker : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent OnCollisionEnter;
    [HideInInspector]
    public UnityEvent OnCollisionExit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnCollisionEnter.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnCollisionExit.Invoke();
    }
}

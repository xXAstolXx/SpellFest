using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InflictDamageCollider : MonoBehaviour
{
    new Collider2D collider;

    [HideInInspector]
    public UnityEvent OnPlayerHit;


    private void Awake()
    {
        collider = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnPlayerHit.Invoke();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitBox : MonoBehaviour
{
    new Collider2D collider;

    public UnityEvent OnHitBoxEntered;


    private void Awake()
    {
        collider = GetComponent<Collider2D>();
    }


}

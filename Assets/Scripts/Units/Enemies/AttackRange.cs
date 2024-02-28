using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackRange : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent OnPlayerInRange = new UnityEvent();
    public bool PlayerInRange { get; private set; }
    public bool isActive = true;


    private void Awake()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.parent.GetComponent<Player>())
        {
            if(isActive)
            {
                PlayerInRange = true;
            }
            OnPlayerInRange.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.parent.GetComponent<Player>())
        {
            PlayerInRange = false;
        }
    }
}

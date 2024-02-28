using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SightRange : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent<GameObject> OnPlayerInSight = new UnityEvent<GameObject>();
    [HideInInspector]
    public UnityEvent OnPlayerOutOfSight = new UnityEvent();

    public bool playerInRange {  get; private set; }
    public bool isActive = true;

    public GameObject target {  get; private set; }


    private void Awake()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.parent.GetComponent<Player>())
        {
            target = collision.transform.parent.gameObject;
            if (isActive)
            {
                OnPlayerInSight.Invoke(collision.transform.parent.gameObject);
            }
            playerInRange = true;
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.parent.GetComponent<Player>())
        {
            if(isActive)
            {
                OnPlayerOutOfSight.Invoke();
            }
            playerInRange = false;
        }
    }
}

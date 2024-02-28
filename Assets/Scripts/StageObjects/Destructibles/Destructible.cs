using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Destructible : MonoBehaviour
{
    [SerializeField]
    private GameObject itemToDrop;
    [SerializeField]
    private float dropChance;
    //[SerializeField]
    //float hP;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if(Random.Range(0f,1f) < dropChance)
        {
            Instantiate(itemToDrop, transform.position, Quaternion.identity);
        }
    }
}

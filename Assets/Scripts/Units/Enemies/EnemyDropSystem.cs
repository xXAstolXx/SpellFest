using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDropSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject[] itemsToDrop;
    [SerializeField]
    private float dropRate;

    private void Start()
    {
        dropRate = Mathf.Clamp01(dropRate);
    }

    public void DropItem()
    {
        if(Random.value <= dropRate)
        {
            GameObject itemToDrop = itemsToDrop[Random.Range(0, itemsToDrop.Length)];
            Instantiate(itemToDrop, transform.position, Quaternion.identity);
        }
    }
}

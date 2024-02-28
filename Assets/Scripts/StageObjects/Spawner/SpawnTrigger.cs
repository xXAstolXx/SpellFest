using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class SpawnTrigger : MonoBehaviour
{
    [SerializeField]
    List<Spawner> spawnsToTrigger;

    bool hasTriggered = false;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(hasTriggered == false)
        {
            Debug.LogWarning("spawn units");
            hasTriggered = true;
            foreach (Spawner spawner in spawnsToTrigger)
            {
                spawner.SpawnEnemy();
            }
        }
    }
}

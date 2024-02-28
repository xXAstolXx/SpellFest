using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SpawnSet : MonoBehaviour
{
    [SerializeField]
    List<Spawner> spawners;
    SpriteRenderer spawnGraphic;


    private void Awake()
    {
        spawnGraphic = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        foreach (Spawner spawner in spawners)
        {
            spawner.OnSpawnActivated.AddListener(StartSpawnAnimation);
            spawner.OnSpawnFinished.AddListener(DestroySpawn);
        }
    }

    void StartSpawnAnimation()
    {
        Debug.Log("start light");
        GetComponent<Animator>().SetBool("isActive", true);
    }

    void StopSpawnAnimation()
    {
        Debug.Log("stop light");
        GetComponent<Animator>().SetBool("isActive", false);
    }

    private void DestroySpawn(Spawner spawner)
    {
        StopSpawnAnimation();
        spawners.Remove(spawner);
        if (spawners.Count == 0)
        {
            GetComponent<Animator>().SetBool("isDissolving", true);
        }
    }

    private void DestroySpawnSet()
    {
        Destroy(gameObject);
    }
}

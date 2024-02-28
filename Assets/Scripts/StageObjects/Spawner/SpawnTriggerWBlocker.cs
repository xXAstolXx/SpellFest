using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class SpawnTriggerWBlocker : MonoBehaviour
{
    [SerializeField]
    List<Spawner> spawnsToTrigger;
    int triggeredSpawnsCount;

    [SerializeField]
    List<SpawnBlocker> blockerToTrigger;

    List<GameObject> enemies = new List<GameObject>();

    bool hasTriggered = false;
    bool blockerDessolved = false;

    AudioSource audioSource;
    bool hasPlayed = false;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        if(spawnsToTrigger != null)
        {
            foreach (var spawn in spawnsToTrigger)
            {
                spawn.OnEnemySpawned.AddListener(AddEnemy);
            }
        }
    }

    private void Update()
    {
        if (hasTriggered)
        {
            if (spawnsToTrigger.Count <= triggeredSpawnsCount)
            {
                if (CheckEnemiesDead())
                {
                    PlayArenaClearedSound();
                    DestroyBlocker();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered == false)
        {
            Debug.LogWarning("spawn units");
            hasTriggered = true;
            foreach (Spawner spawner in spawnsToTrigger)
            {
                spawner.SpawnEnemy();
            }
            foreach (SpawnBlocker blocker in blockerToTrigger)
            {
                blocker.Activate();
            }
            PlayArenaStartetSound();
        }
    }

    bool CheckEnemiesDead()
    {
        foreach (var enemy in enemies)
        {
            if (enemy)
            {
                return false;
            }
        }
        return true;
    }

    private void DestroyBlocker()
    {
        if(blockerDessolved == false)
        {
            blockerDessolved = true;
            foreach (var blocker in blockerToTrigger)
            {
                blocker.Deactivate();
            }
        }
    }

    void AddEnemy(Enemy enemy)
    {
        enemies.Add(enemy.gameObject);
        triggeredSpawnsCount += 1;
    }

    private void PlayArenaClearedSound()
    {
        if(!hasPlayed)
        {
            hasPlayed = true;
            audioSource.PlayOneShot(GlobalSound.Instance.GameSound.ArenaCleared);
        } 
    }

    private void PlayArenaStartetSound()
    {
        audioSource.PlayOneShot(GlobalSound.Instance.GameSound.ArenaStartet);
    }
}

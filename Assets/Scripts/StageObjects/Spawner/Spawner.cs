using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    float spawnDelay = 0f;
    [SerializeField]
    int spawnUntilCheckpoint = 0;
    [SerializeField]
    GameObject enemy;
    [SerializeField]
    GameObject arenaCenter;
    public GameObject Enemy => enemy;

    public UnityEvent<Enemy> OnEnemySpawned { get; private set; } = new UnityEvent<Enemy>();

    public UnityEvent OnSpawnActivated { get; private set; } = new UnityEvent();
    public UnityEvent<Spawner> OnSpawnFinished { get; private set; } = new UnityEvent<Spawner>();


    private void Start()
    {
        if (spawnUntilCheckpoint < Game.Instance.globalData.currentCheckPoint)
        {
            Destroy(gameObject);
        }
    }

    public void SpawnEnemy()
    {
        StartCoroutine(SpawnDelayTimer());
    }

    private IEnumerator SpawnDelayTimer()
    {
        yield return new WaitForSeconds(spawnDelay);
        OnSpawnActivated.Invoke();
        yield return new WaitForSeconds(2f);

        GameObject obj = Instantiate(enemy, new Vector3(transform.position.x, transform.position.y - 0.5f), Quaternion.identity);
        if(arenaCenter != null)
        {
            obj.GetComponent<Wendigo>().arenaCenter = arenaCenter;
        }
        OnEnemySpawned.Invoke(obj.GetComponent<Enemy>());
        OnSpawnFinished.Invoke(this);
    }

    void OnDrawGizmos()
    {
        // Draw a semitransparent red cube at the transforms position
        Gizmos.color = new Color(1,0,0,0.6f);
        Gizmos.DrawSphere(transform.position, 1);
    }
}

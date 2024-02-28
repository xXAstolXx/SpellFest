using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DebugBuildError : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(DelayedNavMeshAgent());
    }

    private IEnumerator DelayedNavMeshAgent()
    {
        yield return new WaitForSeconds(5);
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}

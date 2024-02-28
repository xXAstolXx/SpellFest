using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFreeze : MonoBehaviour
{
    [SerializeField, Range(0, 1)]
    private float duration = 1f;

    private float pendingFreezeDuration = 0f;
    private bool isFrozen = false;


    private void Update()
    {
        if(pendingFreezeDuration > 0 && !isFrozen)
        {
            StartCoroutine(DoFreeze());
        }
    }

    public void Freeze()
    {
        pendingFreezeDuration = duration;
    }
    
    private IEnumerator DoFreeze()
    {
        isFrozen = true;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);
       
        Time.timeScale = 1f;
        pendingFreezeDuration = 0;
        isFrozen = false;
    }
}

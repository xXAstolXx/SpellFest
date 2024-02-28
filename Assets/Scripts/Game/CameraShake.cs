using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    private float shakeDuration = 2/5f;
    private float ShakeStrength;
    private bool isShaking = false;

    private Vector3 initialPosition;

    Coroutine shakeTimer;


    private void Update()
    {
        if(isShaking)
        {
            transform.localPosition = initialPosition + Random.insideUnitSphere * ShakeStrength;
            shakeDuration -= Time.deltaTime;
        }
        else
        {
            shakeDuration = 0f;
            transform.localPosition = initialPosition;
        }
    }

    public void TriggerShake(float shakeStrength)
    {
        this.ShakeStrength = shakeStrength;

        StopCoroutine(shakeTimer);
        shakeTimer = StartCoroutine(ShakeTimer());
    }

    private IEnumerator ShakeTimer()
    {
        isShaking = true;
        initialPosition = transform.localPosition;

        yield return new WaitForSeconds(shakeDuration);

        isShaking = false;
        transform.localPosition = initialPosition;
    }
}

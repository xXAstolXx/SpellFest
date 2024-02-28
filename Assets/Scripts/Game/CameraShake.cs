using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Transform gameObjectTransform;
    private float shakeDuration = 0f;
    [SerializeField,Range(0,2.0f)]
    private float startShakeDuration = 2.0f;
    [SerializeField,Range(0,1.0f)]
    private float shakeMagnitude = 0.7f;
    [SerializeField,Range(0,5f)]
    private float dampingSpeed = 1.0f;

    private Vector3 initialPosition;

    public float ShakeMagnitude { get{ return shakeMagnitude; } set { shakeMagnitude = value; } }

    private void Awake()
    {
        if(transform == null)
        {
            gameObjectTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    private void OnEnable()
    {
        initialPosition = transform.localPosition;
    }

    private void Update()
    {
        if(shakeDuration > 0)
        {
            transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0f;
            transform.localPosition = initialPosition;
        }
    }

    public void TriggerShake(float shakeStrength)
    {
        shakeMagnitude = shakeStrength;
        shakeDuration = 2.0f;
    }
}

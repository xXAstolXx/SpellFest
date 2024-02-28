using UnityEngine;

public class WendigoShake : MonoBehaviour
{
    [SerializeField,Range(0,1.0f)]
    private float shakeMagnitude = 0.7f;
    private Vector3 initialPosition;
    bool isShaking = false;


    public float ShakeMagnitude { get{ return shakeMagnitude; } set { shakeMagnitude = value; } }


    private void OnEnable()
    {
    }

    private void Update()
    {
        if(isShaking)
        {
            transform.position = (Vector2)(initialPosition + Random.insideUnitSphere * shakeMagnitude);
        }
    }

    public void StartShake(Vector3 position)
    {
        initialPosition = position;
        isShaking = true;
    }

    public void StopShake()
    {
        isShaking = false;
        transform.position = initialPosition;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyGraphic : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    bool isDissolving = false;
    float dissolvedAmount;

    bool isSpawning = false;
    float spawnTime;

    public UnityEvent OnDissolveEnd {  get; private set; } = new UnityEvent();
    public UnityEvent OnSpawnEnd { get; private set; } = new UnityEvent();


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        spriteRenderer.material.SetFloat("_PixelsX", spriteRenderer.size.x * 16);
        spriteRenderer.material.SetFloat("_PixelsY", spriteRenderer.size.y * 16);

        spriteRenderer.material.SetFloat("_DissolveStrength", GlobalSettings.Instance.EnemySettings.DissolveStrength);
        spriteRenderer.material.SetInt("_IsStunned", 0);
        spriteRenderer.material.SetInt("_IsUnstunable", 0);
        spriteRenderer.material.SetColor("_TakeDamageColor", GlobalSettings.Instance.EnemySettings.EnemyReceiveDamageColor);
        spriteRenderer.material.SetColor("_DissolveColor", GlobalSettings.Instance.EnemySettings.DissolveColor);
    }

    public void EndDashAni()
    {
        transform.parent.GetComponent<Wendigo>().EndDashAnimation();
    }

    private void Update()
    {
        if (isDissolving)
        {
            dissolvedAmount += Time.deltaTime / GlobalSettings.Instance.EnemySettings.DissolveTime;

            spriteRenderer.material.SetFloat("_DissolveAmount", dissolvedAmount);
            if (dissolvedAmount >= 1) 
            {

                OnDissolveEnd.Invoke();
            }
        }

        if (isSpawning)
        {
            dissolvedAmount -= Time.deltaTime / spawnTime;

            spriteRenderer.material.SetFloat("_DissolveAmount", dissolvedAmount);
            if (dissolvedAmount <= 0)
            {

                OnSpawnEnd.Invoke();
                isSpawning = false;
                transform.GetChild(0).gameObject.SetActive(true);

            }
        }
    }

    public void StartDissolving()
    {
        dissolvedAmount = 0;
        isDissolving = true;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void StartSpawning(float duration)
    {
        spawnTime = duration;
        dissolvedAmount = 1;
        spriteRenderer.material.SetFloat("_DissolveAmount", dissolvedAmount);
        isSpawning = true;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void OnDeathAnimationEnd()
    {
        transform.parent.gameObject.GetComponent<Wendigo>().VictoryScreen();
    }
}

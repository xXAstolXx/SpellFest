using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBlocker : MonoBehaviour
{
    [SerializeField]
    float spawnTime;
    [SerializeField]
    float dissolveStrength;
    [SerializeField]
    Color dissolveColor;
    bool isDissolving = false;
    bool isSpawning = false;
    float dissolvedAmount;

    SpriteRenderer spriteRenderer;


    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.material.SetFloat("_DissolveStrength", dissolveStrength);
        spriteRenderer.material.SetColor("_DissolveColor", dissolveColor);

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isDissolving)
        {
            dissolvedAmount += Time.deltaTime / spawnTime;

            spriteRenderer.material.SetFloat("_DissolveAmount", dissolvedAmount);
            if (dissolvedAmount >= 1)
            {
                gameObject.SetActive(false);
            }
        }

        if (isSpawning)
        {
            dissolvedAmount -= Time.deltaTime / spawnTime;

            spriteRenderer.material.SetFloat("_DissolveAmount", dissolvedAmount);
            if (dissolvedAmount <= 0)
            {
                isSpawning = false;
            }
        }
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        dissolvedAmount = 1;
        isSpawning = true;
        spriteRenderer.material.SetFloat("_DissolveAmount", dissolvedAmount);
    }

    public void Deactivate()
    {
        dissolvedAmount = 0;
        isDissolving = true;
        isSpawning = false;
        spriteRenderer.material.SetFloat("_DissolveAmount", dissolvedAmount);
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FireVFX : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        spriteRenderer.sharedMaterial.SetTexture("_FireTex", GlobalFireVFX.Instance.renderTexture);

    }

    private void Update()
    {
    }
}


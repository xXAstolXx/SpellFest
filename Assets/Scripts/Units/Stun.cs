using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Stun : MonoBehaviour
{
    Enemy target;
    SpriteRenderer spriteRenderer;
    float freezeTime = 0;


    private void Update()
    {
        UpdateStunAnimation();
    }

    public void StartStun(float duration, float stunImmunity, Color freezeColorGreenChannel, Enemy target)
    {
        this.target = target;
        this.target.OnDie.AddListener(TargetDied);
        spriteRenderer = target.gameObject.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.material.SetColor("_GreenChannelColor", freezeColorGreenChannel);

        if(target.isUnstunable == false || target.isStunned == false)
        {
            StartCoroutine(StunTimer(duration, stunImmunity));
        }
    }

    private IEnumerator StunTimer(float duration, float stunImmunity)
    {
        Debug.Log("#######Start stun");
        target.StartStun(stunImmunity);

        yield return new WaitForSeconds(duration);

        target.EndStun();
        Debug.Log("#######end stun");
        TargetDied();
    }

    private void TargetDied()
    {
        Destroy(gameObject);
    }

    private void UpdateStunAnimation()
    {
        if (spriteRenderer)
        {
            spriteRenderer.material.SetFloat("_FreezeTime", freezeTime);
            freezeTime += Time.deltaTime;
        }
    }
}

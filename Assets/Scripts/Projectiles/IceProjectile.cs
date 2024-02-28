using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IceProjectile : ElementProjectile
{
    [SerializeField]
    GameObject stunObject;
    [SerializeField]
    Color freezeColorGreenChannel;


    protected override IEnumerator AoeEffect()
    {
        var enemies = FindObjectsOfType<Enemy>();
        foreach (var enemy in enemies)
        {
            float yPosition = enemy.transform.position.y + enemy.GetComponent<CapsuleCollider2D>().offset.y;
            Vector2 vector = new Vector3(enemy.transform.position.x, yPosition) - impactPosition;
            Vector2 direction = vector.normalized;

            if (vector.magnitude - new Vector2((enemy.GetComponent<CapsuleCollider2D>().size.x / 2f) * Mathf.Abs(direction.x), (enemy.GetComponent<CapsuleCollider2D>().size.y / 2f) * Mathf.Abs(direction.y)).magnitude <= attack.AoeRadius[step])
            {
                Game.Instance.cameraShake.TriggerShake(attack.ShakeStrength);

                enemy.ReceiveAttack(attack.ImpactDamage[step], AttackType.ICEATTACK, source);

                if (attack.Effects.Count > 0)
                {
                    foreach (GameObject effect in attack.Effects)
                    {

                        if (enemy.ignoredElementTypes.Contains(effect.GetComponent<SpellEffect>().Type))
                        {
                            foreach (var type in enemy.ignoredElementTypes)
                            {
                                Debug.Log(type);
                            }
                            continue;
                        }
                        GameObject instance = Instantiate(effect, enemy.transform);
                        instance.GetComponent<SpellEffect>().Apply(enemy);
                    }
                }             
                if (enemy.isUnstunable == false && enemy.TypeToIgnore.Contains(ElementType.ICE) == false)
                {
                    StartCoroutine(DelayStun(enemy));
                }
                enemy.GetComponentInChildren<SpriteRenderer>().material.SetColor("_DissolveColor", ColorSteps[0]);
            }
        }
        yield return null;
    }

    private IEnumerator DelayStun(Enemy enemy)
    {
        yield return new WaitForEndOfFrame();

        if (enemy)
        {
            if(enemy.isUnstunable == false)
            {
                GameObject stunObj = Instantiate(stunObject);
                stunObj.GetComponent<Stun>().StartStun(((IceAttack)attack).StunDuration, ((IceAttack)attack).UnstunableDuration, freezeColorGreenChannel, enemy);
                enemy.health.AmplifyDamage(((IceAttack)attack).DamageAmplifier, ((IceAttack)attack).StunDuration);
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Slime : Enemy, IEnemy
{
    [SerializeField]
    SlimeType slimeType;
    [SerializeField]
    float damage;

    [SerializeField]
    float dashSelfKnockBackStrength;
    [SerializeField]
    float dashPlayerKnockBackStrength;

    SlimeMovement slimeMovement;

    ActionStateSlime state = ActionStateSlime.IDLE;


    protected override void Awake()
    {
        base.Awake();

        slimeMovement = GetComponent<SlimeMovement>();
    }

    protected override void Initialize()
    {
        base.Initialize();

        sightRange.OnPlayerInSight.AddListener(StartChase);
        attackRange.OnPlayerInRange.AddListener(StartAttack);
        slimeMovement.OnStopSliding.AddListener(UpdateActionState);

        animator.SetBool("isDying", false);
        animator.SetBool("isAttacking", false);

        if (usePitch)
        {
            RandomPitch();
        }
    }

    protected override void Update()
    {
        base.Update();

        switch (state)
        {
            case ActionStateSlime.CHASE:
            case ActionStateSlime.ATTACK:
                if (target)
                {
                    slimeMovement.SetTargetPosition(target.transform.position);
                }
                break;
            case ActionStateSlime.STUNNED:
            case ActionStateSlime.IDLE:
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            PlayerHit();
        }
    }

    public override void StartStun(float stunImmuneDuration)
    {
        base.StartStun(stunImmuneDuration);

        state = ActionStateSlime.STUNNED;
        slimeMovement.StopMove();

        StopCoroutine("RecoverFromAttack");
    }

    public override void EndStun()
    {
        base.EndStun();

        UpdateActionState();
    }

    private void StartChase(GameObject target)
    {
        this.target = target;
        if (isRecovering == false && state == ActionStateSlime.IDLE && slimeMovement.type != MovementType.SLIDE)
        {
            Debug.Log("start chase");

            state = ActionStateSlime.CHASE;
            slimeMovement.StartMove();
            animator.SetBool("isWalking", true);
            PlayWalkingSound();
        }
    }

    private void StartAttack()
    {
        if(state != ActionStateSlime.STUNNED)
        {
            if (isRecovering == false && slimeMovement.type != MovementType.SLIDE)
            {
                slimeMovement.StartDash();
                animator.SetBool("isAttacking", true);
                state = ActionStateSlime.ATTACK;
            }
        }
    }

    private void StopAttack()
    {
        state = ActionStateSlime.IDLE;
        slimeMovement.StopMove();
        animator.SetBool("isAttacking", false);
        StopAudioSource();
        StartCoroutine(RecoverFromAttack());
    }

    private IEnumerator RecoverFromAttack()
    {
        isRecovering = true;
        yield return new WaitForSeconds(attackRecoverTime);

        isRecovering = false;

        UpdateActionState();
    }

    protected override void UpdateActionState()
    {
        if(!isRecovering)
        {
            if (attackRange.PlayerInRange)
            {
                if (GetComponent<Collider2D>().IsTouching(target.GetComponent<Player>().hitbox))
                {
                    Vector3 direction = (target.transform.position - transform.position).normalized;
                    target.GetComponent<Player>().ReceiveAttack(damage,
                                                                AttackType.NONE,
                                                                TransformHelper.DirectionToAngle(direction),
                                                                direction * dashPlayerKnockBackStrength,
                                                                gameObject);
                    StopAttack();
                }
                else
                {
                    StartAttack();
                }
            }
            else
            {
                slimeMovement.StartMove();
                state = ActionStateSlime.CHASE;
                PlayWalkingSound();
            }
        } 
    }

    protected override void OnKnockBackEnd()
    {
        if(isDieing == false)
        {
            UpdateActionState();
        }
    }

    private void PlayerHit()
    {
        if (isRecovering == false)
        {
            target.GetComponent<Player>().ReceiveAttack(damage,
                                                        AttackType.NONE,               
                                                        TransformHelper.DirectionToAngle(slimeMovement.faceDirection),
                                                        slimeMovement.faceDirection * dashPlayerKnockBackStrength,
                                                        gameObject);
            KnockBack(slimeMovement.faceDirection * -1 * dashSelfKnockBackStrength);
            StopAttack();
        }
    }

    public override void StartSliding()
    {
        StopAllCoroutines();
        state = ActionStateSlime.STUNNED;
        slimeMovement.StartSliding();
        animator.enabled = false;
    }

    public override void StopSliding()
    {
        state = ActionStateSlime.CHASE;
        UpdateActionState();
        slimeMovement.StopSliding();
        animator.enabled = true;
    }

    public override bool ReceiveAttack(float damage, AttackType damageType, GameObject source)
    {
        damage = GetEffectiveDamage(damage, damageType);

        base.ReceiveAttack(damage, damageType, source);

        StartChase(source);
        PlayHurtSound();
        return true;
    }

    public override bool ReceiveAttack(float damage, AttackType damageType, float angle, Vector2 force, GameObject source)
    {
        damage = GetEffectiveDamage(damage, damageType);

        base.ReceiveAttack(damage, damageType, angle, force, source);

        StartChase(source);
        PlayHurtSound();
        return true;
    }

    float GetEffectiveDamage(float damage, AttackType damageType)
    {
        float result = damage;

        switch (slimeType)
        {
            case SlimeType.FIRE:
                switch (damageType)
                {
                    case AttackType.BASEATTACK:
                        break;
                    case AttackType.FIREATTACK:
                        damage *= GlobalSettings.Instance.EnemySettings.DamageAmpSameElement;
                        break;
                    case AttackType.ICEATTACK:
                        damage *= GlobalSettings.Instance.EnemySettings.DamageAmpOppositeElement;
                        break;
                }
                break;
            case SlimeType.ICE:
                switch (damageType)
                {
                    case AttackType.BASEATTACK:
                        break;
                    case AttackType.FIREATTACK:
                        damage *= GlobalSettings.Instance.EnemySettings.DamageAmpOppositeElement;
                        break;
                    case AttackType.ICEATTACK:
                        damage *= GlobalSettings.Instance.EnemySettings.DamageAmpSameElement;
                        break;
                }
                break;
        }

        return damage;
    }


    public override void Die()
    {
        base.Die();
        StopAudioSource();
        PlayDeathSound();
    }

    private void PlayHurtSound()
    {
        return;
    }

    private void PlayDeathSound()
    {
        audioSource.loop = false;
        switch (slimeType)
        {
            default:
            case SlimeType.BASIC:
                audioSource.clip = GlobalSound.Instance.SlimeSound.BasicSlimeDeathSound;
                break;
            case SlimeType.FIRE:
                audioSource.clip = GlobalSound.Instance.SlimeSound.FireSlimeDeathSound;
                break;
            case SlimeType.ICE:
                audioSource.clip = GlobalSound.Instance.SlimeSound.IceSlimeDeathSound;
                break;
            case SlimeType.SILLY_MAN:
                audioSource.clip = GlobalSound.Instance.SillyManSound.DeathSound;
                break;
        }
        audioSource.Play();
    }

    private void PlayWalkingSound()
    {
        audioSource.loop = true;
        switch (slimeType)
        {
            case SlimeType.BASIC:
                audioSource.clip = GlobalSound.Instance.SlimeSound.BasicSlimeWalkingSound;
                break;
            case SlimeType.FIRE:
                audioSource.clip = GlobalSound.Instance.SlimeSound.FireSlimeWalkingSound;
                break;
            case SlimeType.ICE:
                audioSource.clip = GlobalSound.Instance.SlimeSound.IceSlimeWalkingSound;
                break;
        }
        audioSource.Play();
    }

    private void StopAudioSource()
    {
        audioSource.Stop();
        audioSource.loop = false;
    }
}

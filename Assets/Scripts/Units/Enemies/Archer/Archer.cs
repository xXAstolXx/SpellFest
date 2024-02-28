using System.Collections;
using UnityEngine;


public class Archer : Enemy
{
    [SerializeField]
    GameObject projectile;
    [SerializeField]
    Transform shootPosition;

    ArcherMovement archerMovement;
    BoxCollider2D aimCollider;
    Vector2 size;
    Vector2 offset;

    ActionStateArcher state = ActionStateArcher.IDLE;

    Coroutine chargeCoroutine;
    Coroutine recoverCoroutine;


    protected override void Awake()
    {
        base.Awake();

        archerMovement = GetComponent<ArcherMovement>();
        animator = GetComponentInChildren<Animator>();
        aimCollider = GetComponentInChildren<BoxCollider2D>();
    }

    protected override void Initialize()
    {
        base.Initialize();

        sightRange.OnPlayerInSight.AddListener(TrySetActive);
        attackRange.OnPlayerInRange.AddListener(UpdateActionState);
        archerMovement.OnStopSliding.AddListener(UpdateActionState);

        animator.SetBool("isDying", false);
        animator.SetBool("isAttacking", false);

        InitAimCollider();

        if (usePitch)
        {
            RandomPitch();
        }
    }

    protected override void Update()
    {
        base.Update();

        if (state != ActionStateArcher.IDLE)
        {
            UpdateAimCollider();
        }

        switch (state)
        {
            case ActionStateArcher.CHASE:
                archerMovement.SetTargetPosition(target.transform.position);
                break;
            case ActionStateArcher.AIMING:
                CheckValidAttackSight();
                archerMovement.SetTargetPosition(target.transform.position);
                break;
            case ActionStateArcher.CHARGE:
                break;
            case ActionStateArcher.STUNNED:
            case ActionStateArcher.IDLE:
                break;
        }
    }

    private void UpdateAimCollider()
    {
        Vector3 shootVector = target.transform.position + new Vector3(0, 1) - aimCollider.transform.position;
        size.y = shootVector.magnitude;
        offset.y = shootVector.magnitude / 2f;
        aimCollider.size = size;
        aimCollider.offset = offset;
        aimCollider.transform.rotation = Quaternion.Euler(0, 0, TransformHelper.DirectionToAngle(shootVector.normalized) - 90f);
    }

    private void InitAimCollider()
    {
        size = aimCollider.size;
        offset = aimCollider.offset;
        size.x = projectile.GetComponent<CapsuleCollider2D>().size.y;
        aimCollider.size = size;
    }

    public override void StartStun(float stunImmuneDuration)
    {
        base.StartStun(stunImmuneDuration);

        state = ActionStateArcher.STUNNED;
        if(state == ActionStateArcher.CHARGE)
        StopAllCoroutines();
    }

    public override void EndStun()
    {
        base.EndStun();

        CheckForTarget();
    }

    private void StartAiming()
    {
        archerMovement.StartMove();
        animator.SetBool("isWalking", true);
        animator.SetBool("isAttacking", false);
        state = ActionStateArcher.AIMING;
    }

    private void TrySetActive(GameObject target)
    {
        this.target = target;
        UpdateActionState();  
    }

    private void StartChase()
    {
        state = ActionStateArcher.CHASE;
        archerMovement.StartMove();
        animator.SetBool("isAttacking", false);
    }

    private void StartCharge()
    {
        Debug.Log("start attack");
        archerMovement.StopMove();
        animator.SetBool("isAttacking", true);
        chargeCoroutine = StartCoroutine(Charge());
    }

    private IEnumerator Charge()
    {
        state = ActionStateArcher.CHARGE;
        yield return new WaitForSeconds(projectile.GetComponent<ArcherProjectile>().Attack.ChargeTime);
        FinishCharge();
    }

    private void FinishCharge()
    {
        Shoot();
        recoverCoroutine = StartCoroutine(RecoverFromAttack());
        animator.SetBool("isAttacking", false);
    }

    private void TryCancelCharge()
    {
        if(state == ActionStateArcher.CHARGE)
        {
            StopCoroutine(chargeCoroutine);
        }
    }

    private void CheckValidAttackSight()
    {
        if(archerMovement.type != MovementType.SLIDE)
        {
            if (aimCollider.IsTouching(CustomGrid.Instance.wallMap.GetComponent<CompositeCollider2D>()))
            {
                Debug.Log("collide with walls");
            }
            else
            {
                StartCharge();
            }
        }
    }

    private void Shoot()
    {
        GameObject obj = Instantiate(projectile);
        obj.GetComponent<ArcherProjectile>().Initialize((Vector2)(target.transform.position - transform.position).normalized, shootPosition.position, gameObject);
        obj.GetComponent<ArcherProjectile>().Shoot();
        audioSource.clip = GlobalSound.Instance.ArcherSound.CrossbowShoot;
        audioSource.Play();
    }

    private IEnumerator RecoverFromAttack()
    {
        state = ActionStateArcher.RECOVERING;
        yield return new WaitForSeconds(attackRecoverTime);
        CheckForTarget();
    }

    private void CheckForTarget()
    {
        if(attackRange.PlayerInRange)
        {
            StartAiming();
        }
        else
        {
            StartChase();
        }
    }

    protected override void UpdateActionState()
    {
        switch (state)
        {
            case ActionStateArcher.IDLE:
                if (target == null)
                {
                    return;
                }
                else
                {
                    StartChase();
                }
                break;
            case ActionStateArcher.CHASE:
                if (attackRange.PlayerInRange)
                {
                    StartAiming();
                }
                break;
            case ActionStateArcher.AIMING:
            case ActionStateArcher.CHARGE:
            case ActionStateArcher.RECOVERING:
            case ActionStateArcher.STUNNED:
                break;
        }
    }

    public override void StartSliding()
    {
        StopAllCoroutines();
        state = ActionStateArcher.STUNNED;
        archerMovement.StartSliding();
        animator.enabled = false;
    }

    public override void StopSliding()
    {
        state = ActionStateArcher.CHASE;
        UpdateActionState();
        archerMovement.StopSliding();
        animator.enabled = true;
    }

    public override bool ReceiveAttack(float damage, AttackType damageType, GameObject source)
    {
        base.ReceiveAttack(damage, damageType, source);
        TryCancelCharge();
        UpdateActionState();
        return true;
    }

    public override bool ReceiveAttack(float damage, AttackType damageType, float angle, Vector2 force, GameObject source)
    {
        base.ReceiveAttack( damage, damageType, angle, force, source);
        TryCancelCharge();
        UpdateActionState();
        return true;
    }

    public override void Die()
    {
        StopAllCoroutines();
        base.Die();
        audioSource.clip = GlobalSound.Instance.ArcherSound.DeathSound;
        audioSource.Play();
    }
}

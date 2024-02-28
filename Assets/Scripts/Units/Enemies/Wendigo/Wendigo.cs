using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Wendigo : Enemy
{
    [SerializeField]
    int winScreenBuildIndex;

    WendigoState state = WendigoState.IDLE;

    WendigoPhase phase = WendigoPhase.ONE;

    [SerializeField]
    public GameObject arenaCenter;

    [SerializeField]
    WendigoPhaseStats phaseOneStats;
    [SerializeField]
    WendigoPhaseStats phaseTwoStats;
    [SerializeField]
    WendigoPhaseStats phaseThreeStats;
    [SerializeField]
    WendigoBossPlayer wendigoBossPlayer;

    int step;

    WendigoPhaseStats stats;

    [SerializeField]
    float playerDamageWhenTacklingWendigo;
    [SerializeField]
    float chanceForDashAttack = 0.5f;

    [SerializeField, Header("Dash Attack")]
    float dashDamage;
    [SerializeField]
    float dashChannelTime;
    [SerializeField]
    float dashTargetExtension;

    [SerializeField]
    float dashSelfKnockBackStrength;
    [SerializeField]
    float dashPlayerKnockBackStrength;
    [SerializeField]
    float hitPlayerKnockBackStrength;

    Vector3 dashPosition;

    [SerializeField, Header("Range Attack")]
    WendigoProjectile fireProjectile;
    [SerializeField]
    WendigoProjectile iceProjectile;
    [SerializeField]
    int projectileAmount;
    float timePerProjectile;
    [SerializeField]
    float circularSpeed;
    [SerializeField]
    float radius;
    [SerializeField]
    float blastRotatingTime;

    List<WendigoProjectile> rotatingProjectiles = new List<WendigoProjectile>();

    [SerializeField, Header("Transition phase")]
    float expansionRateWaveAttack;
    [SerializeField]
    float transitionPhaseTime;
    [SerializeField]
    float timeBetweenWaves;
    [SerializeField]
    GameObject enemyHealthBar;


    [SerializeField, Range(0, 1f)]
    float shakeStrength;

    WendigoMovement wendigoMovement;
    [SerializeField]
    BoxCollider2D aimCollider;
    Vector2 size;
    Vector2 offset;

    Coroutine channelDash;
    Coroutine channelBlast;
    Coroutine blast;
    Coroutine recoverAttack;
    Coroutine follow;


    protected override void Awake()
    {
        base.Awake();

        aimCollider = GetComponentInChildren<BoxCollider2D>();
        wendigoMovement = GetComponent<WendigoMovement>();
    }

    protected override void Initialize()
    {
        base.Initialize();

        GetComponent<WendigoHealth>().OnHealthPointLimit.AddListener(StartTransitionPhaseOne);
        sightRange.OnPlayerInSight.AddListener(TryStartChase);
        attackRange.OnPlayerInRange.AddListener(TryStartAim);

        animator.SetBool("isCasting", false);
        animator.SetBool("isSlashing", false);
        animator.SetBool("isRaising", false);

        enemyHealthBar.SetActive(false);

        isUnstunable = true;

        stats = phaseOneStats;

        step = 0;

        InitAimCollider();

        timePerProjectile = (2 * Mathf.PI) / (projectileAmount * circularSpeed);
    }

    protected override void Update()
    {
        base.Update();

        if(state != WendigoState.IDLE)
        {
            UpdateAimCollider();
        }

        switch (state)
        {
            case WendigoState.CHASE:
                wendigoMovement.SetTargetPosition(target.transform.position);
                break;
            case WendigoState.AIMING:
                CheckValidAttackSight();
                wendigoMovement.SetTargetPosition(target.transform.position);
                break;
            case WendigoState.DASHATTACK:
                if (Mathf.Approximately(transform.position.x, dashPosition.x) 
                    && Mathf.Approximately(transform.position.y, dashPosition.y))
                {
                    Debug.Log("target position reached!");

                    FinishAttack();
                }
                else if (Mathf.Approximately(transform.position.x, wendigoMovement.lastFramePosition.x) 
                         && Mathf.Approximately(transform.position.y, wendigoMovement.lastFramePosition.y))
                {
                    FinishAttack();
                }
                break;
            case WendigoState.TRANSITIONPHASEONE:
                if (Mathf.Approximately(transform.position.x, arenaCenter.transform.position.x) 
                    && Mathf.Approximately(transform.position.y, arenaCenter.transform.position.y))
                {
                    StartTransitionPhaseTwo();
                }
                break;
            case WendigoState.FOLLOW:
                wendigoMovement.SetTargetPosition(target.transform.position);
                break;
            case WendigoState.STUNNED:
            case WendigoState.CHANNELINGDASH:
            case WendigoState.BLAST:
            case WendigoState.CHANNELINGBLAST:
            case WendigoState.IDLE:
                break;
        }
    }

    private void UpdateAimCollider()
    {
        Vector3 shootVector = target.transform.position - transform.position;
        size.y = shootVector.magnitude;
        offset.y = shootVector.magnitude / 2f;
        aimCollider.size = size;
        aimCollider.offset = offset;
        aimCollider.transform.rotation = Quaternion.Euler(0, 0, TransformHelper.DirectionToAngle(shootVector.normalized) - 90f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collision");
        if (collision.gameObject.GetComponent<Player>())
        {
            Debug.Log("collision with player, position: " + transform.position);
                
            if (state == WendigoState.DASHATTACK)
            {
                Debug.Log("collision with player -> dash");
                PlayerHitOnDash();
            }
            else
            {
                PlayerHit(playerDamageWhenTacklingWendigo, hitPlayerKnockBackStrength);
                Debug.Log("collision with player -> hit");
            }
        }
    }

    private void InitAimCollider()
    {
        size = aimCollider.size;
        offset = aimCollider.offset;
        size.x = 1f;
        aimCollider.size = size;
    }

    private void TryStartChase(GameObject target)
    {
        if (this.target == false)
        {
            this.target = target;
            enemyHealthBar.SetActive(true);

            StartChase();
            wendigoBossPlayer.PlayArenaSound(GlobalSound.Instance.WendigoSound.ArenaSound);
        }
    }

    private void StartChase()
    {
        Debug.Log("start chase");

        state = WendigoState.CHASE;
        wendigoMovement.StartMove();
    }

    void StartChannelDash()
    {
        Debug.Log("start channel dash, position" + transform.position);
        wendigoMovement.StopMove();

        channelDash = StartCoroutine(ChannelDash());
    }

    IEnumerator ChannelDash()
    {
        animator.SetBool("isRaising", true);

        state = WendigoState.CHANNELINGDASH;
        wendigoMovement.isShaking = true;
        GetComponent<WendigoShake>().StartShake(transform.position);

        yield return new WaitForSeconds(dashChannelTime);

        wendigoMovement.isShaking = false;
        GetComponent<WendigoShake>().StopShake();
        StartDash();
        animator.SetBool("isRaising", false);

    }

    void StartDash()
    {
        Debug.Log("Dash! position: " + transform.position);
        Vector3 targetPosition = target.transform.position - new Vector3(0, 1, 0);
        Vector3 vecToTarget = targetPosition - transform.position;
        Vector3 direction = vecToTarget.normalized;
        dashPosition = targetPosition + direction * dashTargetExtension;

        wendigoMovement.StartDash(dashPosition);
        animator.SetBool("isSlashing", true);
        state = WendigoState.DASHATTACK;
        audioSource.PlayOneShot(GlobalSound.Instance.WendigoSound.DashAttack);
    }

    private void PlayerHitOnDash()
    {
        Debug.Log("player hit on dash! position: "+ transform.position);
        animator.SetBool("isSlashing", true);

        PlayerHit(dashDamage, dashPlayerKnockBackStrength);

    }

    public void EndDashAnimation()
    {
        animator.SetBool("isSlashing", false);
        FinishAttack();
    }

    private IEnumerator RecoverAttack()
    {
        Debug.Log("recover from attack, podition: " + transform.position);

        isRecovering = true;
        yield return new WaitForSeconds(attackRecoverTime);
        isRecovering = false;
        follow = StartCoroutine(Follow());
    }

    private IEnumerator Follow()
    {
        state = WendigoState.FOLLOW;
        wendigoMovement.StartFollow(target.transform.position);
        yield return new WaitForSeconds(stats.AttackPauseDuration);
        wendigoMovement.StopMove();
        UpdateActionState();
    }

    void StartChannelBlast()
    {
        wendigoMovement.StopMove();
        state = WendigoState.CHANNELINGBLAST;
        StopAllCoroutines();
        channelBlast = StartCoroutine(ChannelBlast());
    }

    IEnumerator ChannelBlast()
    {
        Debug.Log("Channel blast");
        animator.SetBool("isRaising", true);

        if (phase == WendigoPhase.ONE || phase == WendigoPhase.TWO)
        {
            if(Random.Range(0f,1f) < 0.5f)
            {
                for (int i = 0; i < projectileAmount; i++)
                {
                    yield return new WaitForSeconds(timePerProjectile);
                    ActivateProjectile(fireProjectile.gameObject);
                }
            }
            else
            {
                for (int i = 0; i < projectileAmount; i++)
                {
                    yield return new WaitForSeconds(timePerProjectile);
                    ActivateProjectile(iceProjectile.gameObject);
                }
            }
        }
        else
        {
            for (int i = 0; i < projectileAmount; i++)
            {
                yield return new WaitForSeconds(timePerProjectile);
                if (i % 2 == 0)
                {
                    ActivateProjectile(fireProjectile.gameObject);
                }
                else
                {
                    ActivateProjectile(iceProjectile.gameObject);
                }
            }
        }

        
        Debug.Log("amount of activated projectiles: "+rotatingProjectiles.Count);


        yield return new WaitForSeconds(blastRotatingTime);
        animator.SetBool("isRaising", false);

        StartBlast();
    }

    private void ActivateProjectile(GameObject projectileTemplate)
    {
        GameObject projectile = Instantiate(projectileTemplate);
        projectile.GetComponent<WendigoProjectile>().Initialize(this.gameObject);
        projectile.GetComponent<WendigoProjectile>().StartRotate(circularSpeed, radius);
        rotatingProjectiles.Add(projectile.GetComponent<WendigoProjectile>());
    }

    void StartBlast()
    {
        blast = StartCoroutine(Blast());
    }

    IEnumerator Blast()
    {
        animator.SetBool("isCasting", true);

        Debug.Log("Blast!");
        state = WendigoState.BLAST;

        for (int i = 0; i < rotatingProjectiles.Count; i++)
        {
            yield return new WaitForSeconds(timePerProjectile);
            rotatingProjectiles[i].StartTravel(target.transform.position);
        }
        rotatingProjectiles.Clear();
        FinishAttack();
        animator.SetBool("isCasting", false);

    }

    private void CheckValidAttackSight()
    {
        if (aimCollider.IsTouching(CustomGrid.Instance.wallMap.GetComponent<CompositeCollider2D>()))
        {
        }
        else
        {
            Debug.Log("no walls");

            ChooseNextAttack();
        }
    }

    private void TryStartAim()
    {
        if(state == WendigoState.CHASE)
        {
            StartAim();
        }
    }

    private void StartAim()
    {
        Debug.Log("start aiming");
        state = WendigoState.AIMING;
        wendigoMovement.StartMove();
    }

    void ChooseNextAttack()
    {
        Debug.Log("choose next attack");
        if(Random.Range(0f, 1f) < chanceForDashAttack)
        {
            StartChannelDash();
        }
        else
        {
            StartChannelBlast();
        }
    }

    void StartTransitionPhaseOne(WendigoPhase phase)
    {
        Debug.Log("cancel actions from state: " + state);
        CancelActions();

        switch (phase)
        {
            case WendigoPhase.TWO:
                stats = phaseTwoStats;
                step = 1;
                break;
            case WendigoPhase.THREE:
                stats = phaseThreeStats;
                step = 2;
                break;
        }
        wendigoMovement.StartMove();
        wendigoMovement.SetTargetPosition(arenaCenter.transform.position);
        this.phase = phase;
        state = WendigoState.TRANSITIONPHASEONE;
        Debug.Log("change phase to :" + phase);
    }

    void CancelActions()
    {
        if (state == WendigoState.CHANNELINGDASH)
        {
            StopCoroutine(channelDash);
        }
        else if (state == WendigoState.IDLE)
        {
            StopCoroutine(recoverAttack);
        }
        else if (state == WendigoState.CHANNELINGBLAST)
        {
            StopCoroutine(channelBlast);

            foreach (var projectile in rotatingProjectiles)
            {
                Destroy(projectile.gameObject);
            }
            rotatingProjectiles.Clear();
        }
        else if (state == WendigoState.BLAST)
        {
            StopCoroutine(blast);
            foreach (var projectile in rotatingProjectiles)
            {
                if (projectile)
                {
                    Destroy(projectile.gameObject);
                }
            }
            rotatingProjectiles.Clear();
        }
        else if (state == WendigoState.FOLLOW)
        {
            StopCoroutine(follow);
        }
        animator.SetBool("isCasting", false);
        animator.SetBool("isSlashing", false);
        animator.SetBool("isRaising", false);

        wendigoMovement.StopMove();
    }

    void StartTransitionPhaseTwo()
    {
        state = WendigoState.TRANSITIONPHASETWO;
        Vector3Int currentTile = new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        Debug.Log("Start transition phase two");
        CustomGrid.Instance.TileWave(currentTile, expansionRateWaveAttack);
        StartCoroutine(WaveAttackTimer(currentTile));

    }

    IEnumerator WaveAttackTimer(Vector3Int startTile)
    {
        animator.SetBool("isRaising", true);

        float time = 0;

        while(time < transitionPhaseTime)
        {
            CustomGrid.Instance.TileWave(startTile, expansionRateWaveAttack);

            yield return new WaitForSeconds(timeBetweenWaves);

            time += timeBetweenWaves;
        }
        FinishTransition();
        animator.SetBool("isRaising", false);
    }

    void FinishTransition()
    {
        UpdateActionState();
    }

    private void FinishAttack()
    {
        Debug.Log("finish attack");
        state = WendigoState.IDLE;
        wendigoMovement.StopMove();
        recoverAttack = StartCoroutine(RecoverAttack());
        animator.SetBool("isSlashing", false);
    }

    protected override void UpdateActionState()
    {
        if (!isRecovering)
        {
            if (attackRange.PlayerInRange)
            {
                if (GetComponent<Collider2D>().IsTouching(target.GetComponent<Player>().hitbox))
                {
                    Vector3 direction = (target.transform.position - transform.position).normalized;
                    target.GetComponent<Player>().ReceiveAttack(playerDamageWhenTacklingWendigo,
                                                                AttackType.NONE,
                                                                TransformHelper.DirectionToAngle(direction),
                                                                direction * dashPlayerKnockBackStrength,
                                                                gameObject);
                    FinishAttack();
                    Debug.Log("FinishAttack");
                }
                else
                {
                    StartAim();
                }
            }
            else
            {
                StartChase();
                Debug.Log("startchase");
            }
        }
    }

    private void PlayerHit(float damage, float force)
    {
        if (isRecovering == false)
        {
            target.GetComponent<Player>().ReceiveAttack(playerDamageWhenTacklingWendigo,
                                                        AttackType.NONE,
                                                        TransformHelper.DirectionToAngle(wendigoMovement.faceDirection),
                                                        wendigoMovement.faceDirection * force,
                                                        gameObject);
        }
    }

    public override bool ReceiveAttack(float damage, AttackType damageType, GameObject source)
    {
        base.ReceiveAttack(damage, damageType, source);
        if(enemyHealthBar.activeSelf == false)
        {
            StartChase();
            enemyHealthBar.SetActive(true);
        }
        audioSource.PlayOneShot(GlobalSound.Instance.WendigoSound.HurtSound);
        return true;
    }

    public override bool ReceiveAttack(float damage, AttackType damageType, float angle, Vector2 force, GameObject source)
    {
        base.ReceiveAttack(damage, damageType, angle, Vector2.zero, source);
        if (enemyHealthBar.activeSelf == false)
        {
            StartChase();
            enemyHealthBar.SetActive(true);
        }
        audioSource.PlayOneShot(GlobalSound.Instance.WendigoSound.HurtSound);
        return true;
    }

    public override void Die()
    {
        OnDie.Invoke();

        sightRange.isActive = false;
        attackRange.isActive = false;
        CancelActions();
        Debug.Log("----------die call of enemy");
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<EnemyMovement>().enabled = false;
        GetComponent<EnemyHealth>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        GetComponentInChildren<EnemyHealthbar>().gameObject.SetActive(false);
        animator.SetBool("isWalking", false);
        animator.SetBool("isDying", true);
        animator.SetBool("isCasting", false);
        animator.SetBool("isSlashing", false);
        animator.SetBool("isRaising", false);
        audioSource.PlayOneShot(GlobalSound.Instance.WendigoSound.DeathSound);
    }

    public void VictoryScreen()
    {
        SceneManager.LoadScene(winScreenBuildIndex);
    }
}



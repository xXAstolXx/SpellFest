using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public abstract class Enemy : Unit
{
    [SerializeField]
    float spawnTime = 0;
    [SerializeField]
    int spawnUntilCheckpoint = 0;

    [SerializeField]
    protected List<ElementType> typeToIgnore = new List<ElementType>();
    public List<ElementType> TypeToIgnore => typeToIgnore; 

    [SerializeField]
    GameObject floatingTxtObject;

    protected EnemyDropSystem enemyDropSystem;
    EnemyMovement enemyMovement;
    SpriteRenderer spriteRenderer;
    EnemyGraphic graphic;
    EnemyVFX enemyVFX;
    EnemyHealthbar enemyHealthbar;
    protected Animator animator;

    [SerializeField]
    protected float attackRecoverTime;
    protected bool isRecovering;

    [SerializeField]
    protected float knockBackRecoverTime;

    [Header("SoundOptions")]
    [SerializeField]
    protected bool usePitch = false;
    [SerializeField, Range(-3f, 0f)]
    protected float minRandomPitchRange;
    [SerializeField, Range(0, 3f)]
    protected float maxRandomPitchRange;

    public bool isStunned { get; private set; } = false;
    public bool isUnstunable { get; protected set; } = false;

    protected bool isDieing { get; private set; } = false;

    protected SightRange sightRange;
    protected AttackRange attackRange;

    protected GameObject target;

    public UnityEvent OnSpawnFinished { get; private set; } = new UnityEvent();
    public UnityEvent OnDie { get; private set; } = new UnityEvent();

    protected AudioSource audioSource;


    protected override void Awake()
    {
        base.Awake();

        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        enemyDropSystem = GetComponent<EnemyDropSystem>();
        enemyMovement = GetComponent<EnemyMovement>();
        enemyMovement.OnStopSliding.AddListener(UpdateActionState);
        sightRange = GetComponentInChildren<SightRange>();
        attackRange = GetComponentInChildren<AttackRange>();
        audioSource = GetComponent<AudioSource>();
        enemyVFX = GetComponentInChildren<EnemyVFX>();
        enemyHealthbar = GetComponentInChildren<EnemyHealthbar>();
        graphic = GetComponentInChildren<EnemyGraphic>();
    }

    protected override void Initialize()
    {
        base.Initialize();

        enemyMovement.OnAgentRestart.AddListener(OnKnockBackEnd);
        graphic.OnDissolveEnd.AddListener(enemyDropSystem.DropItem);
        graphic.OnDissolveEnd.AddListener(DestroyEnemy);

        animator.SetBool("isDying", false);

        if (spawnUntilCheckpoint < Game.Instance.globalData.currentCheckPoint && spawnTime == 0)
        {
            Destroy(gameObject);
        }

        if (spawnTime != 0)
        {
            StartSpawning();
            graphic.OnSpawnEnd.AddListener(FinishSpawning);
        }
    }

    protected void ShowDamage(string text, AttackType attackType)
    {
        if (floatingTxtObject)
        {
            GameObject prefab = Instantiate(floatingTxtObject, transform.position, Quaternion.identity);
            prefab.GetComponent<DamageNumber>().PopUp(text, attackType);
        }
    }

    void StartSpawning()
    {
        graphic.StartSpawning(spawnTime);

        sightRange.isActive = false;
        attackRange.isActive = false;

        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<EnemyMovement>().enabled = false;
        GetComponent<EnemyHealth>().enabled = false;
        GetComponentInChildren<Animator>().enabled = false;
        enemyHealthbar.gameObject.SetActive(false);

        isDieing = true;
    }

    void FinishSpawning()
    {
        OnSpawnFinished.Invoke();
        sightRange.isActive = true;
        attackRange.isActive = true;

        GetComponent<NavMeshAgent>().enabled = true;
        GetComponent<EnemyMovement>().enabled = true;
        GetComponent<EnemyHealth>().enabled = true;
        GetComponentInChildren<Animator>().enabled = true;
        enemyHealthbar.gameObject.SetActive(true);

        isDieing = false;

        if (sightRange.playerInRange && target == null)
        {
            target = sightRange.target;
        }
        UpdateActionState();
    }

    public virtual void StartDieAnimation()
    {
        GetComponentInChildren<Animator>().SetBool("isDead", true);
    }

    public override void Die()
    {
        OnDie.Invoke();

        sightRange.isActive = false;
        attackRange.isActive = false;
        isDieing = true;

        Debug.Log("----------die call of enemy");
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<EnemyMovement>().enabled = false;
        GetComponent<EnemyHealth>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        GetComponentInChildren<Animator>().enabled = false;
        GetComponentInChildren<EnemyHealthbar>().gameObject.SetActive(false);

        isUnstunable = true;
        graphic.StartDissolving();
        enemyVFX.ExplodeOnDeath.Play();
        enemyVFX.Burn.Stop();
    }

    private void DestroyEnemy()
    {
        Debug.Log("destroy enemy");
        Destroy(gameObject);
    }

    public override bool ReceiveAttack(float damage, AttackType damageType, GameObject source)
    {
        if(isDieing == false)
        {
            if (isStunned)
            {
                CancelStun();
            }

            target = source;
            var damageToUI = Mathf.RoundToInt(health.TakeDamage(damage) * GlobalSettings.Instance.UiDamageIncreaseMultiplier);
            ShowDamage(damageToUI.ToString(), damageType);
            StartCoroutine(ChangeColorOnReceiveDamage());

            GameManager.Instance.PauseGame(GlobalSettings.Instance.ScreenFreezeDurationEnemyAttack);

            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool ReceiveAttack(float damage, AttackType damageType, float angle, Vector2 force, GameObject source)
    {
        if (isDieing == false)
        {
            if (isStunned)
            {
                CancelStun();
            }

            target = source;
            var damageToUI = Mathf.RoundToInt(health.TakeDamage(damage) * GlobalSettings.Instance.UiDamageIncreaseMultiplier);
            ShowDamage(damageToUI.ToString(), damageType);
            StartCoroutine(ChangeColorOnReceiveDamage());

            GameManager.Instance.PauseGame(GlobalSettings.Instance.ScreenFreezeDurationEnemyAttack);

            KnockBack(force);

            return true;
        }
        else
        {
            return false;
        }
    }

    protected virtual void UpdateActionState() { }

    public virtual void StartStun(float stunImmunityDuration)
    {
        StartCoroutine(StunImmunTimer(stunImmunityDuration));
        spriteRenderer.material.SetInt("_IsStunned", 1);

        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<EnemyMovement>().enabled = false;
        GetComponentInChildren<Animator>().enabled = false;
        isStunned = true;
    }

    public virtual void EndStun()
    {
        if (isStunned)
        {
            spriteRenderer.material.SetInt("_IsStunned", 0);
            GetComponent<EnemyMovement>().enabled = true;
            GetComponentInChildren<EnemyVFX>().FreezeEnd.Play();
            isStunned = false;

            if (unitMovement.type == MovementType.SLIDE)
            {
                GetComponent<EnemyMovement>().RestartSliding();
            }
            else
            {
                GetComponentInChildren<Animator>().enabled = true;
                GetComponent<NavMeshAgent>().enabled = true;
            }
        }
    }

    public virtual void CancelStun()
    {
        spriteRenderer.material.SetInt("_IsStunned", 0);

        GetComponent<NavMeshAgent>().enabled = true;
        GetComponent<EnemyMovement>().enabled = true;
        GetComponentInChildren<Animator>().enabled = true;
        GetComponentInChildren<EnemyVFX>().FreezeCancel.Play();
        isStunned = false;
    }

    private IEnumerator StunImmunTimer(float duration)
    {
        isUnstunable = true;
        spriteRenderer.material.SetInt("_IsUnstunable", 1);

        yield return new WaitForSeconds(duration);

        spriteRenderer.material.SetInt("_IsUnstunable", 0);
        isUnstunable = false;
    }

    private IEnumerator ChangeColorOnReceiveDamage()
    {
        spriteRenderer.material.SetInt("_IsTakeDamageColor", 1);
        yield return new WaitForSeconds(GlobalSettings.Instance.EnemySettings.EnemyReceiveDamageColorTime);
        spriteRenderer.material.SetInt("_IsTakeDamageColor", 0);
    }

    protected void KnockBack(Vector2 force)
    {
        if(!gameObject.GetComponent<Wendigo>())
        {
            StartCoroutine(enemyMovement.Pause(knockBackRecoverTime));
            StartCoroutine(enemyMovement.ApplyForce(force));
        }
    }

    protected virtual void OnKnockBackEnd() { }

    protected void RandomPitch()
    {
        float randomPitch = UnityEngine.Random.Range(minRandomPitchRange, maxRandomPitchRange);
        audioSource.pitch = randomPitch;
    }
}

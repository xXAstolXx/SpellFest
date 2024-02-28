using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class Player : Unit
{
    private AudioSource audioSource;
    private PlayerMovement movement;
    PullPath pullPath;
    PlayerVFX playerVFX;
    public PlayerGraphic graphic { get; private set; }

    public Collider2D hitbox;

    [SerializeField]
    GameObject baseProjectile;
    [SerializeField]
    GameObject fireProjectile;
    [SerializeField]
    GameObject iceProjectile;

    bool iceSpellUpgraded = false;
    bool fireSpellUpgraded = false;

    [SerializeField]
    Effect slowOnCastSpell;

    [SerializeField]
    float baseAttackCooldown;

    public SpellInput activeSpell { get; private set; } = SpellInput.FIRE;
    bool isPulling = false;

    [SerializeField]
    float invincibleTime;
    [HideInInspector]
    public bool isInvincible;

    public bool isInDialogue = false;
    public bool isInCheckpointRange = false;

    public UnityEvent OnAdvanceDialogue { get; private set; } = new UnityEvent();
    public UnityEvent OnPetCat { get; private set; } = new UnityEvent();

    Transform projectileSpawnPosition;


    protected override void Awake()
    {
        base.Awake();

        movement = GetComponent<PlayerMovement>();
        pullPath = GetComponentInChildren<PullPath>();
        pullPath.OnSpellChargeChanged.AddListener(OnChangeSpellCharge);

        graphic = GetComponentInChildren<PlayerGraphic>();

        playerVFX = GetComponentInChildren<PlayerVFX>();

        audioSource = GetComponent<AudioSource>();

        projectileSpawnPosition = transform.Find("ProjectileSpawnPosition");

        Time.timeScale = 1f;
    }

    protected override void Start()
    {
        base.Start();

        if(Game.Instance.globalData.currentCheckPoint != 0)
        {
            Vector3 position = new Vector3(Game.Instance.globalData.spawnPositionX, Game.Instance.globalData.spawnPositionY, 0);
            transform.position = position;
        }
    }

    protected override void Initialize()
    {
        base.Initialize();

        pullPath.Initialize();
        health.OnTakeDamage.AddListener(SetInvincible);
        UpdateSpellUI();
    }

    protected override void Update()
    {
        base.Update();

    }

    public override void Die()
    {
        if(UI.Instance.deathScreen.isActive == false)
        {
            Debug.Log("die");
            Game.Instance.SaveGlobalData();
            UI.Instance.deathScreen.OnPlayerDeath();
        }
    }

    public void LeftClick(Vector3 mousePosition)
    {
        if (isPulling == false)
        {
            Debug.Log("start spell attack");
            isPulling = true;
            slowOnCastSpell.Apply(this);
            //playerVFX.SetPullIndicator();

            switch (activeSpell)
            {
                case SpellInput.FIRE:
                    pullPath.StartPull(fireProjectile.GetComponent<ElementProjectile>().attack.MaxDistance, fireProjectile);
                    break;
                case SpellInput.ICE:
                    pullPath.StartPull(iceProjectile.GetComponent<ElementProjectile>().attack.MaxDistance, iceProjectile);
                    break;
            }
        }
    }

    public void RightClick(Vector3 mousePosition)
    {
        if (isPulling)
        {
            isPulling = false;
            pullPath.FinishPull();
            slowOnCastSpell.Remove(this);
            //playerVFX.DeactivatePullIndicator();
        }
    }

    public void LeftClickReleased(Vector3 mousePosition)
    {
        if (isPulling)
        {
            if (pullPath.isPathValid)
            {
                Debug.Log("Shoot spell");

                SpellAttack();
            }
            else if (pullPath.pullDirection.magnitude != 0)
            {
                BaseAttack();
            }
            isPulling = false;
            pullPath.FinishPull();
            slowOnCastSpell.Remove(this);
            //playerVFX.DeactivatePullIndicator();
        }
    }

    public void ChangeSpell()
    {
        if (!isPulling)
        {
            switch(activeSpell)
            {
                case SpellInput.FIRE:
                    activeSpell = SpellInput.ICE;
                    playerVFX.ChangeSpell(activeSpell);
                    break;
                case SpellInput.ICE:
                    activeSpell = SpellInput.FIRE;
                    playerVFX.ChangeSpell(activeSpell);
                    break;
            }
            UpdateSpellUI();
        }
    }

    private void OnChangeSpellCharge(int step)
    {
        if(activeSpell == SpellInput.FIRE)
        {
            playerVFX.ChangeCharge(AttackType.FIREATTACK, step);
        }
        else
        {
            playerVFX.ChangeCharge(AttackType.ICEATTACK, step);
        }
    }

    private void SpellAttack()
    {
        Vector2 direction = pullPath.pullDirection * -1;

        GameObject projectile;
        if(activeSpell == SpellInput.FIRE)
        {
            projectile = Instantiate(fireProjectile);
            projectile.GetComponent<ElementProjectile>().isUpgraded = fireSpellUpgraded;
            audioSource.PlayOneShot(GlobalSound.Instance.PlayerSound.FireShootSound);
        }
        else
        {
            projectile = Instantiate(iceProjectile);
            projectile.GetComponent<ElementProjectile>().isUpgraded = iceSpellUpgraded;
            audioSource.PlayOneShot(GlobalSound.Instance.PlayerSound.IceShootSound);
        }
        
        projectile.GetComponent<ElementProjectile>().Initialize(direction, pullPath.travelDistance, pullPath.GetCurrentPullStep(), 
                                                                projectileSpawnPosition.position, gameObject);
        projectile.GetComponent<ElementProjectile>().Shoot();
    }

    private void BaseAttack()
    {
        Vector2 direction = pullPath.pullDirection * -1;

        audioSource.PlayOneShot(GlobalSound.Instance.PlayerSound.WindBlastSound);
        GameObject projectile = Instantiate(baseProjectile);
        projectile.GetComponent<BaseProjectile>().Initialize(direction, projectileSpawnPosition.position, gameObject);
        projectile.GetComponent<BaseProjectile>().Shoot();
    }

    private void UpdateSpellUI()
    {
        UI.Instance.spellUI.UpdateSpellUI(activeSpell);
        graphic.UpdateSpellGraphic(activeSpell);
    }

    private void SetInvincible()
    {
        StartCoroutine(InvincibleTimer());
    }

    private IEnumerator InvincibleTimer()
    {
        isInvincible = true;
        graphic.SetInvincible();
        health.SetInvincible();
        yield return new WaitForSeconds(invincibleTime);
        graphic.SetVincible();
        health.SetVincible();
        isInvincible = false;
    }

    public override void StartSliding()
    {
        StopAllCoroutines();
        movement.StartSliding();
        GetComponentInChildren<Animator>().enabled = false;
    }

    public override void StopSliding()
    {
        movement.StopSliding();
        GetComponentInChildren<Animator>().enabled = true;
    }

    public override bool ReceiveAttack(float damage, AttackType damageType, float angle, Vector2 force, GameObject source)
    {
        if(isInvincible)
        {
            return false;
        }
        else
        {
            health.TakeDamage(damage);
            playerVFX.TakeDamage(damage, angle);
            StartCoroutine(movement.ApplyForce(force));
            UI.Instance.hurtScreen.ShowHurtScreen();
            audioSource.PlayOneShot(GlobalSound.Instance.PlayerSound.HurtSound);
            return true;
        }       
    }

    public override bool ReceiveAttack(float damage, AttackType damageType, GameObject source)
    {
        if(isInvincible) 
        {
            return false;
        }
        else 
        { 
            health.TakeDamage(damage);
            UI.Instance.hurtScreen.ShowHurtScreen();
            audioSource.PlayOneShot(GlobalSound.Instance.PlayerSound.HurtSound);
            return true;
        }
    }

    public override void Heal(float value)
    {
        base.Heal(value);
        audioSource.clip = GlobalSound.Instance.ItemSound.HealSound;
        audioSource.Play();
        playerVFX.PlayHealVFX();
    }

    public void UpgradeFireSpell()
    {
        fireSpellUpgraded = true;
    }

    public void UpgradeIceSpell()
    {
        iceSpellUpgraded = true;
    }

    public void UpdateMoveInput(Vector2 input)
    {
        graphic.UpdateGraphicMoveStates(input);
        movement.UpdateInput(input);

        if (isPulling)
        {
            pullPath.UpdateDirection(input);
        }
    }

    public void Interact()
    {
        if (isInDialogue)
        {
            Game.Instance.dialogueSystem.DisplayNextSentence();
            Game.Instance.dialogueSystem.OnEndDialogue.AddListener(EndDialogue);
            OnAdvanceDialogue.Invoke();
        }
        else if (isInCheckpointRange)
        {
            OnPetCat.Invoke();
        }
    }

    private void EndDialogue()
    {
        isInDialogue = false;
    }
}

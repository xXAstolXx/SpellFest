using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public abstract class UnitHealth : MonoBehaviour
{
    [SerializeField]
    protected float maxHP;
    protected float currentHP;
    protected float dps = 0;
    protected float hps = 0;
    protected float intervalDamage = 0;

    protected float damageAmplifier = 1;

    protected float hpChange = 0;

    bool isInvincible = false;

    [HideInInspector]
    public UnityEvent OnDie { get; private set; } = new UnityEvent();
    [HideInInspector]
    public UnityEvent OnTakeDamage { get; private set; } = new UnityEvent();


    protected virtual void Start()
    {
        currentHP = maxHP;
        InitializeUI();
    }

    protected virtual void Update()
    {
        UpdateConstantDamage();
    }

    protected virtual void LateUpdate()
    {
        if (hpChange != 0 && !isInvincible)
        {
            currentHP = Mathf.Clamp(currentHP + hpChange, 0, maxHP);

            if (currentHP <= 0)
            {
                OnDie.Invoke();
            } else if (hpChange < 0)
            {
                OnTakeDamage.Invoke();
            }

            UpdateUI();
        }

        hpChange = 0;
    }

    protected abstract void InitializeUI();

    protected abstract void UpdateUI();

    public virtual void SetInvincible()
    {
        isInvincible = true;
    }

    public virtual void SetVincible()
    {
        isInvincible = false;
    }

    protected virtual void UpdateConstantDamage()
    {
        hpChange -= (dps * Time.deltaTime) * damageAmplifier;
        if (intervalDamage != 0)
        {
            hpChange -= intervalDamage * damageAmplifier;
            UI.Instance.hurtScreen.ShowHurtScreen();
        }
        hpChange += (hps * Time.deltaTime);
    }

    public virtual float TakeDamage(float damage)
    {
        float finalDamage = damage * damageAmplifier;
        hpChange -= finalDamage;
        return finalDamage;
    }

    public void Heal(float heal)
    {
        hpChange += heal;
    }

    public void ApplyDps(float dps)
    {
        this.dps += dps;
    }

    public void RemoveDps(float dps)
    {
        this.dps -= dps;
    }

    public void ModifyDamageTaken(float damageModifier)
    {
        this.damageAmplifier += damageModifier - 1;
    }

    public void ResetDamageTaken(float damageModifier)
    {
        this.damageAmplifier -= damageModifier - 1;
    }

    public void ApplyIntervalDamage(float intervalDamage)
    {
        this.intervalDamage += intervalDamage;
    }

    public void RemoveIntervalDamage(float intervalDamage)
    {
        this.intervalDamage -= intervalDamage;
    }

    public void ResetHealth()
    {
        currentHP = maxHP;
    }

    public void AmplifyDamage(float amount, float duration)
    {
        StartCoroutine(AmplifyDamageTimer(amount, duration));
    }

    private IEnumerator AmplifyDamageTimer(float amount, float duration)
    {
        damageAmplifier += amount - 1;
        yield return new WaitForSeconds(duration);
        damageAmplifier -= amount - 1;
    }
}

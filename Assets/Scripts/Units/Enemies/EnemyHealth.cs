using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : UnitHealth
{
    private EnemyHealthbar healthbar;


    private void Awake()
    {
        healthbar = GetComponentInChildren<EnemyHealthbar>();
    }
    protected override void InitializeUI()
    {
        healthbar.Initialize(maxHP);
    }

    protected override void UpdateUI()
    {
        healthbar.UpdateHealth(currentHP);
    }
}

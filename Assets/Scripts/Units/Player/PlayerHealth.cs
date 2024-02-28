using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : UnitHealth
{
    protected override void InitializeUI()
    {
        UI.Instance.playerHealthBar.SetMaxHealth(maxHP);
    }

    protected override void UpdateUI()
    {
        UI.Instance.playerHealthBar.SetCurrentHealth(currentHP);
    }
}

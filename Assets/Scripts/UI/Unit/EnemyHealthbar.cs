using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthbar : MonoBehaviour
{
    private Slider slider;
    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void Initialize(float maxValue)
    {
        slider.minValue = 0;
        slider.maxValue = maxValue;
        slider.value = maxValue;
    }
    
    public void UpdateHealth(float amount)
    {
        slider.value = amount;
    }

    public void HideHealthBar()
    {
        gameObject.SetActive(false);
    }
    public void ShowHealthbar()
    {
        gameObject.SetActive(true);
    }
}

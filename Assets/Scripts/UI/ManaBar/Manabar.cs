using UnityEngine;
using UnityEngine.UI;

public class Manabar : MonoBehaviour
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

    public void UpdateMana(float amount)
    {
        slider.value = amount;
    }

    public float GetCurrentValue()
    {
        return slider.value;
    }
}

using UnityEngine;

public class PlayerHealthBar : MonoBehaviour
{
    float maxHP;
    float currentHP;
    float halfHeartValue;

    [SerializeField]
    int maxHeartCount;
    [SerializeField]
    GameObject fullHeart;
    [SerializeField]
    GameObject halfHeart;
    [SerializeField]
    GameObject emptyHeart;


    private void Start()
    {
    }

    public void SetMaxHealth(float maxHealth)
    {
        maxHP = maxHealth;
        halfHeartValue = (float)maxHP / (maxHeartCount * 2);
        Debug.Log($"halfheart value: {halfHeartValue} ");
        SetCurrentHealth(maxHealth);
    }

    public void SetCurrentHealth(float currentHealth)
    {
        currentHP = currentHealth;
        if (this.currentHP != currentHealth)
        {
            return;
        }
        int halfHeartcount = Mathf.FloorToInt(currentHP / halfHeartValue);
        for(int j = 0; j < transform.childCount; j++)
        {
            if (transform.childCount != 0) { Destroy(transform.GetChild(j).gameObject); }
        }
        for (int i = 0; i < maxHeartCount*2; i++)
        {
            if(i%2  == 0 && (i+1) < halfHeartcount)
            {
                Instantiate(fullHeart, transform);
            }
            else if (i%2 == 0 && (i+1) == halfHeartcount)
            {
                Instantiate(halfHeart, transform);
            }
            else if(i%2 == 0 && (i+1) > halfHeartcount)
            {
                if(i == 0)
                {
                    Instantiate(halfHeart, transform);
                }
                else
                {
                    Instantiate(emptyHeart, transform);
                }
            }
        }
    }
}

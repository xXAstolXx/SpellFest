using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    [SerializeField]
    private Color basicAttackColor;
    [SerializeField]
    private Color iceAttackColor;
    [SerializeField]
    private Color fireAttackColor;


    private TextMeshProUGUI textDamageNumber;
    private DestroyInSeconds destroyInSeconds;

    private void Awake()
    {
        textDamageNumber = GetComponentInChildren<TextMeshProUGUI>();
        destroyInSeconds = GetComponent<DestroyInSeconds>();
    }

    public void PopUp(string text, AttackType attackType)
    {

        textDamageNumber.text = text;
        switch (attackType)
        {
            case AttackType.NONE:
            case AttackType.BASEATTACK:
                textDamageNumber.color = basicAttackColor;
                break;
            case AttackType.FIREATTACK:
                textDamageNumber.color = fireAttackColor;
                break;
            case AttackType.ICEATTACK:
                textDamageNumber.color = iceAttackColor;
                break;
        }
        destroyInSeconds.DestroyGameObject();
    }
}

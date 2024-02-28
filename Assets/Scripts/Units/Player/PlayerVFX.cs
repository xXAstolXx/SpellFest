using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class PlayerVFX : MonoBehaviour 
{
    [SerializeField]
    ParticleSystem takeDamage;
    [SerializeField]
    ParticleSystem healVFX;
    [SerializeField]
    GameObject changetoFireSpell;
    [SerializeField]
    GameObject changetoIceSpell;
    [Header("Fire charges"), SerializeField]
    GameObject fireChargeLow;
    [SerializeField]
    GameObject fireChargeMedium;
    [SerializeField]
    GameObject fireChargeMax;
    [Header("Ice charges"), SerializeField]
    GameObject iceChargeLow;
    [SerializeField]
    GameObject iceChargeMedium;
    [SerializeField]
    GameObject iceChargeMax;
    [SerializeField]
    GameObject burn;
    [SerializeField]
    GameObject clickIndicator;
    Vector3 clickIndicatorRelativePosition;
    List<ParticleSystem> charges = new List<ParticleSystem>();


    private void Awake()
    {
        charges.Add(fireChargeLow.GetComponent<ParticleSystem>());
        charges.Add(fireChargeMedium.GetComponent<ParticleSystem>());
        charges.Add(fireChargeMax.GetComponent<ParticleSystem>());
        charges.Add(iceChargeLow.GetComponent<ParticleSystem>());
        charges.Add(iceChargeMedium.GetComponent<ParticleSystem>());
        charges.Add(iceChargeMax.GetComponent<ParticleSystem>());
    }

    private void Start()
    {
    }

    private void Update()
    {
        if(clickIndicator.activeSelf) 
        {
            clickIndicator.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(clickIndicatorRelativePosition);
        }
    }


    public void TakeDamage(float amount, float angle)
    {
        var shape = takeDamage.shape;
        Vector3 rotation = new Vector3 (0,0,angle-(takeDamage.shape.arc/2));
        shape.rotation = rotation;
        takeDamage.Play();
    }

    public void ChangeSpell(SpellInput spell)
    {
        switch (spell)
        {
            case SpellInput.FIRE:
            default:
                Instantiate(changetoFireSpell, transform.parent);
                break;
            case SpellInput.ICE:
                Instantiate(changetoIceSpell, transform.parent);
                break;
        }
    }

    public void ChangeCharge(AttackType attackType, int level)
    {
        if(level == 0)
        {
            StopCharges();
            return;
        }
        if (attackType == AttackType.FIREATTACK)
        {
            if(level == 1)
            {
                PlayCharge(0);
            }
            else if (level == 2)
            {
                PlayCharge(1);
            }
            else if (level == 3)
            {
                PlayCharge(2);
            }
        }
        else if (attackType == AttackType.ICEATTACK)
        {
            if (level == 1)
            {
                PlayCharge(3);
            }
            else if (level == 2)
            {
                PlayCharge(4);
            }
            else if (level == 3)
            {
                PlayCharge(5);
            }
        }
    }

    private void PlayCharge(int value)
    {
        for(int i = 0; i < charges.Count; i++)
        {
            if(i == value)
            {
                charges[i].Play();
            }
            else
            {
                charges[i].Stop();
            }
        }
    }

    private void StopCharges()
    {
        for (int i = 0; i < charges.Count; i++)
        {
            charges[i].Stop();
        }
    }

    public void PlayHealVFX()
    {
        healVFX.Play();
    }

    public void StartBurn()
    {
        burn.GetComponent<ParticleSystem>().Play();
    }

    public void StopBurn()
    {
        burn.GetComponent<ParticleSystem>().Stop();
    }

    public void SetPullIndicator()
    {
        clickIndicatorRelativePosition = Input.mousePosition;
    }

    public void ActivatePullIndicator()
    {
        clickIndicator.SetActive(true);
        clickIndicator.GetComponent<ParticleSystem>().Play();
    }

    public void DeactivatePullIndicator()
    {
        clickIndicator.SetActive(false);
    }
}

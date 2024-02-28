using UnityEngine;
using UnityEngine.Events;

public class WendigoHealth : EnemyHealth
{
    public UnityEvent<WendigoPhase> OnHealthPointLimit { get; private set; } = new UnityEvent<WendigoPhase>();
    float hpLimit;


    protected override void Start()
    {
        base.Start();

        hpLimit = (2 / 3f) * maxHP;
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        if(currentHP < hpLimit)
        {
            if(Mathf.Approximately(hpLimit, (2 / 3f) * maxHP))
            {
                Debug.Log(currentHP + " - " + hpLimit + " - "+ (2 / 3f) * maxHP);
                OnHealthPointLimit.Invoke(WendigoPhase.TWO);
                hpLimit = (1 / 3f) * maxHP;
            }
            else
            {
                Debug.Log(currentHP + " - " + hpLimit + " - " + (2 / 3f) * maxHP);
                OnHealthPointLimit.Invoke(WendigoPhase.THREE);
                hpLimit = -1;
            }
        }
    }
}

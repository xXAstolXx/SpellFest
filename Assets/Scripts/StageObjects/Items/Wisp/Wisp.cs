using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wisp : NPC
{
    [SerializeField]
    AttackType type;
    public AttackType Type => type;



    protected override void Start()
    {
        base.Start();

        switch (type)
        {
            case AttackType.FIREATTACK:
                if (Game.Instance.globalData.fireSpellUpgraded)
                {
                    Destroy(gameObject);
                }
                break;
            case AttackType.ICEATTACK:
                if (Game.Instance.globalData.iceSpellUpgraded)
                {
                    Destroy(gameObject);
                }
                break;
        }
        Game.Instance.dialogueSystem.OnEndDialogue.AddListener(DestroyGameObject);
    }

    private void Activate(Collider2D collision)
    {
        Debug.Log("enter3 l");
        var player = collision.transform.parent.gameObject.GetComponent<Player>();
        if (player == null)
        {
            return;
        }
        switch (type)
        {
            case AttackType.FIREATTACK:
                player.UpgradeFireSpell();
                Game.Instance.globalData.fireSpellUpgraded = true;
                break;
            case AttackType.ICEATTACK:
                player.UpgradeIceSpell();
                Game.Instance.globalData.iceSpellUpgraded = true;
                break;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        Debug.Log("Upgrade spell");
        Activate(collision);
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
    }

    void DestroyGameObject()
    {
        if (dialogueShowed)
        {
            Destroy(gameObject);
        }
    }
}

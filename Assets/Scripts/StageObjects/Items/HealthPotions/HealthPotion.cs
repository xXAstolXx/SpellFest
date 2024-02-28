using UnityEngine;

public class HealthPotion : Item
{
    [SerializeField]
    private int healthValue;

    protected override void Activate(Collider2D collision)
    {
        
        var player = collision.transform.parent.GetComponent<Player>();
        if(player == null)
        {
            return;
        }
        Debug.Log($"Player: {player} ");
        player.Heal(healthValue);
        base.Activate(collision);
    }
}

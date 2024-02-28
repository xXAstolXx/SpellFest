using UnityEngine;
using UnityEngine.Events;

public class ArcherProjectile : Projectile
{
    [SerializeField]
    ArcherAttack attack;
    public ArcherAttack Attack => attack;

    [HideInInspector]
    public UnityEvent<float> OnEnemyHit = new UnityEvent<float>();

    public void Initialize(Vector2 direction, Vector3 position, GameObject source)
    {
        this.source = source;
        this.direction = direction;
        this.transform.position = position;
        moveVector = attack.Speed * Time.deltaTime * direction;
        transform.rotation = Quaternion.Euler(Vector3.forward * Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }

    public override void Activate()
    {
        Player target = collision.gameObject.GetComponent<Player>();
        Vector2 vector = (Vector2)(target.transform.position - transform.position);
        target.ReceiveAttack(attack.ImpactDamage,
                             AttackType.NONE,
                             TransformHelper.DirectionToAngle(direction),
                             vector.normalized * attack.KnockBackStrength,
                             source);
    }


    protected override void TargetCollision(Collision2D collision)
    {
        this.collision = collision;
        Activate();
        DestroyProjectile();
    }

    protected override void WallCollision()
    {
        DestroyProjectile();

    }
}
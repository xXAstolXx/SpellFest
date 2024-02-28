using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class BaseProjectile : Projectile
{
    [SerializeField]
    BaseAttack attack;
    BaseAttack defaultAttack;
    [SerializeField]
    ParticleSystem travelVFX;

    [HideInInspector]
    public UnityEvent<float> OnEnemyHit = new UnityEvent<float>();

    [SerializeField]
    private AudioClip impactSound;
    public void Initialize(Vector2 direction, Vector3 position, GameObject source)
    {
        this.source = source;


        this.direction = direction;
        this.transform.position = position;

        moveVector = attack.Speed[0] * Time.deltaTime * direction;

        var shape = travelVFX.shape;
        Vector3 rotation = new Vector3(0, 0, TransformHelper.DirectionToAngle(direction * -1) - (travelVFX.shape.arc / 2));
        transform.rotation = Quaternion.Euler(rotation);

        travelVFX.Play();
    }

    public override void Activate()
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        Vector2 vector = (Vector2)(enemy.transform.position - transform.position);
        enemy.ReceiveAttack(attack.ImpactDamage[0],
                            AttackType.NONE,
                            TransformHelper.DirectionToAngle(vector.normalized),
                            direction * attack.EnemyKnockBackStrength[0],
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

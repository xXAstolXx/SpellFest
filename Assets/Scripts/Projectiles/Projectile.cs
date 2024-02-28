using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Projectile : MonoBehaviour
{
    protected Vector2 direction;
    protected Vector2 moveVector;
    protected bool isTravelling = false;

    protected Rigidbody2D rb;
    protected Collision2D collision;

    protected GameObject source;


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void FixedUpdate()
    {
        if (isTravelling)
        {
            Travel();   
        }
    }

    public virtual void Shoot()
    {
        isTravelling = true;
    }

    protected virtual void Travel()
    {
        rb.MovePosition((Vector2)transform.position + moveVector);
    }

    protected virtual void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if(isTravelling)
        {
            if (collision.gameObject.GetComponent<Unit>())
            {
                TargetCollision(collision);
            }
            else if (collision.gameObject.GetComponent<Destructible>())
            {
                DestructableCollision();
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Walls"))
            {
                WallCollision();
            }
            isTravelling = false;
        }
    }

    protected abstract void TargetCollision(Collision2D collision);

    protected abstract void WallCollision();

    protected virtual void DestructableCollision() { }

    public abstract void Activate();
}

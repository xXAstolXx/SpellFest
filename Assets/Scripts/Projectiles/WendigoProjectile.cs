using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WendigoProjectile : Projectile
{
    [SerializeField]
    WendigoAttack attack;

    [SerializeField]
    ParticleSystem rotateVFX;
    [SerializeField]
    ParticleSystem travelVFX;
    [SerializeField]
    ParticleSystem activationVFX;

    private float travelledDistance = 0;
    private float distance;
    protected int step;

    float circularSpeed;
    float radius;
    Vector2 circleCenter;
    float currentAngle;
    bool isRotating = false;
    float pi = Mathf.PI;
    Vector3 impactPosition;


    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (isRotating )
        {
            Rotate();
        }
    }

    public void Initialize(GameObject source)
    {
        this.source = source;

        this.step = 0;

        activationVFX.GetComponent<DestroyVFX>().OnparticleSystemFinished.AddListener(DestroyProjectile);
    }

    public void StartTravel(Vector3 targetPosition)
    {
        Vector2 travelVector = (Vector2)(targetPosition - transform.position);
        this.distance = travelVector.magnitude;
        this.direction = travelVector.normalized;

        moveVector = attack.Speed[step] * Time.deltaTime * direction;
        var shape = travelVFX.shape;
        Vector3 rotation = new Vector3(0, 0, TransformHelper.DirectionToAngle(direction * -1) - (travelVFX.shape.arc / 2));
        transform.rotation = Quaternion.Euler(rotation);

        isTravelling = true;
        isRotating = false;
        rotateVFX.Stop();
        travelVFX.Play();
    }

    protected override void Travel()
    {
        base.Travel();

        travelledDistance += moveVector.magnitude;
        if (travelledDistance >= distance)
        {
            Activate();
        }
    }

    public void StartRotate(float speed, float radius)
    {
        this.circularSpeed = speed;
        this.radius = radius;

        currentAngle = 0;

        circleCenter = new Vector2(source.transform.position.x, source.transform.position.y + 1f);
        transform.position = circleCenter + new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle)) * radius;

        isRotating = true;
        rotateVFX.Play();
    } 

    private void Rotate()
    {
        currentAngle += circularSpeed * Time.deltaTime;
        if(currentAngle >= 2* pi)
        {
            currentAngle -= 2* pi;
        }

        Vector2 circleVector;
        if(currentAngle <= pi/2f)
        {
            circleVector = new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle));
        }
        else if (currentAngle <= pi)
        {
            circleVector = new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle));
        }
        else if (currentAngle <= (3 / 2f) * pi)
        {
            circleVector = new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle));
        }
        else
        {
            circleVector = new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle));
        }

        circleCenter = new Vector2(source.transform.position.x, source.transform.position.y + 1f);
        rb.MovePosition(circleCenter + (Vector2)circleVector * radius);
    }

    public override void Activate()
    {
        Debug.Log("activate projectile");
        isTravelling = false;

        travelVFX.Stop();
        isTravelling = false;

        impactPosition = new Vector3(Mathf.FloorToInt(travelVFX.transform.position.x) + 0.5f, Mathf.FloorToInt(travelVFX.transform.position.y) + 0.5f);
        AoeEffect();

        activationVFX.transform.position = impactPosition;
        activationVFX.Play();

        StartCoroutine(DelayedSpawnTiles());
    }

    protected virtual void AoeEffect()
    {
        var player = FindObjectOfType<Player>();
        float yPosition = player.transform.position.y + player.GetComponent<BoxCollider2D>().offset.y;
        Vector2 vector = new Vector3(player.transform.position.x, yPosition) - impactPosition;
        Vector2 direction = vector.normalized;

        if (vector.magnitude - new Vector2((player.GetComponent<BoxCollider2D>().size.x / 2f) * Mathf.Abs(direction.x), (player.GetComponent<BoxCollider2D>().size.y / 2f) * Mathf.Abs(direction.y)).magnitude <= attack.AoeRadius[step])
        {
            Game.Instance.cameraShake.TriggerShake(attack.ShakeStrength);

            player.ReceiveAttack(attack.ImpactDamage[step],
                                attack.Type,
                                TransformHelper.DirectionToAngle(vector.normalized),
                                vector.normalized * attack.EnemyKnockBackStrength[step],
                                source);
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collide");
        if (isTravelling)
        {
            if (collision.gameObject.GetComponent<Player>())
            {
                Debug.Log("collide with player");
                TargetCollision(collision);
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Walls"))
            {
                WallCollision();
            }
        }
    }

    protected override void TargetCollision(Collision2D collision)
    {
        Debug.Log("collision call");
        this.collision = collision;
        Activate();
    }

    protected override void WallCollision()
    {
        Activate();
    }

    private IEnumerator DelayedSpawnTiles()
    {
        yield return new WaitForSeconds(attack.TileSpawnDelay);
        Debug.Log("spawn tiles");

        Vector3Int currentTile = new Vector3Int(Mathf.FloorToInt(impactPosition.x), Mathf.FloorToInt(impactPosition.y));
        List<Vector3Int> validTiles = new List<Vector3Int>();
        //validTiles.Add(currentTile);

        int tileRadius = Mathf.RoundToInt(attack.AoeRadius[step]);
        int minX = currentTile.x - tileRadius;
        int maxX = currentTile.x + tileRadius;
        int minY = currentTile.y - tileRadius;
        int maxY = currentTile.y + tileRadius;

        for (int y = minY; y < maxY; y++)
        {
            for (int x = minX; x < maxX; x++)
            {
                Vector3Int nextTile = new Vector3Int(x, y, 0);
                float distance = Vector3.Distance(currentTile, nextTile);
                if (distance < attack.AoeRadius[step])
                {
                    if (UnityEngine.Random.Range(0f, 1f) < attack.TileSpawnChance[Mathf.Clamp(Mathf.RoundToInt(distance) - 1, 0, attack.TileSpawnChance.Count - 1)])
                    {
                        validTiles.Add(nextTile);
                    }
                }
            }
        }

        StartCoroutine(CustomGrid.Instance.ElementAttackMultiple(validTiles, attack.ElementType));
    }
}

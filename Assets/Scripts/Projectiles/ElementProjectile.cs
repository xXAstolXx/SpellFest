using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ElementProjectile : Projectile
{
    [SerializeField]
    protected ElementAttack attackData;
    public ElementAttack AttackData => attackData;
    [SerializeField]
    protected ElementAttack attackUpgradeData;
    public ElementAttack AttackUpgradeData => attackUpgradeData;

    public ElementAttack attack => isUpgraded ? attackUpgradeData : attackData;
    public bool isUpgraded;

    //[SerializeField]
    //ParticleSystem startVFX;
    [SerializeField]
    ParticleSystem travelVFX;
    [SerializeField]
    ParticleSystem activationVFX;
    public GameObject chargeVFXObject;

    [SerializeField]
    List<Color> colorSteps;
    public List<Color> ColorSteps { get => colorSteps; }

    private float travelledDistance = 0;
    private float distance;
    protected int step;

    protected Vector3 impactPosition;
    protected Vector3 offsetVector;


    protected override void Awake()
    {
        base.Awake();

    }

    private void Start()
    {
        switch (attack.Type)
        {
            case AttackType.FIREATTACK:
                if (Game.Instance.globalData.fireSpellUpgraded)
                {
                    isUpgraded = true;
                }
                else
                {
                    isUpgraded = false;
                }
                break;
            case AttackType.ICEATTACK:
                if (Game.Instance.globalData.iceSpellUpgraded)
                {
                    isUpgraded = true;
                }
                else
                {
                    isUpgraded = false;
                }
                break;
        }
    }

    public void Initialize(Vector2 direction, float length, int step, Vector3 position, GameObject source)
    {
        this.source = source;

        this.direction = direction;
        this.step = step;

        if (attack.AoeRadius[step] > 2)
        {
            var startSpeedModule = activationVFX.main.startSpeed;
            var emitter = activationVFX.emission.burstCount;
            emitter *= 2; 
            startSpeedModule = new ParticleSystem.MinMaxCurve(0.4f, 10);
        }
        if (step == 0)
        {
            travelVFX.transform.localScale *= 0.8f;
        }
        else if (step == 1)
        {
            travelVFX.transform.localScale *= 1.1f;
        }
        else
        {
            travelVFX.transform.localScale *= 1.4f;
        }

        this.transform.position = position;

        moveVector = attack.Speed[step] * Time.deltaTime * direction;
        this.distance = length;

        activationVFX.GetComponent<DestroyVFX>().OnparticleSystemFinished.AddListener(DestroyProjectile);

        var shape = travelVFX.shape;
        Vector3 rotation = new Vector3(0, 0, TransformHelper.DirectionToAngle(direction*-1) - (travelVFX.shape.arc / 2));
        transform.rotation = Quaternion.Euler(rotation);

        travelVFX.Play();
    }

    protected override void Travel()
    {
        travelledDistance += moveVector.magnitude;
        if (travelledDistance >= distance)
        {
            Activate();
        }

        base.Travel();
    }

    public override void Activate()
    {
        Destroy(travelVFX.gameObject);
        isTravelling = false;

        impactPosition = new Vector3(Mathf.FloorToInt(travelVFX.transform.position.x) + 0.5f, Mathf.FloorToInt(travelVFX.transform.position.y) + 0.5f);

        activationVFX.transform.position = impactPosition;

        activationVFX.Play();

        StartCoroutine(AoeEffect());
        StartCoroutine(DelayedSpawnTiles());
    }

    protected virtual IEnumerator AoeEffect()
    {
        var enemies = FindObjectsOfType<Enemy>();
        foreach (var enemy in enemies)
        {
            float yPosition = enemy.transform.position.y + enemy.GetComponent<CapsuleCollider2D>().offset.y;
            Vector2 vector = new Vector3(enemy.transform.position.x, yPosition) - impactPosition;
            Vector2 direction = vector.normalized;

            if (vector.magnitude - new Vector2((enemy.GetComponent<CapsuleCollider2D>().size.x/2f)*Mathf.Abs(direction.x), (enemy.GetComponent<CapsuleCollider2D>().size.y/2f) * Mathf.Abs(direction.y)).magnitude <= attack.AoeRadius[step])
            {
                Camera.main.GetComponent<CameraShake>().TriggerShake(attack.ShakeStrength);

                enemy.ReceiveAttack(attack.ImpactDamage[step],
                                    attack.Type,
                                    TransformHelper.DirectionToAngle(vector.normalized),
                                    vector.normalized * attack.EnemyKnockBackStrength[step],
                                    source);

                if (attack.Effects.Count > 0)
                {
                    foreach (var obj in attack.Effects)
                    {
                        GameObject effectObj = Instantiate(obj);
                        SpellEffect effect = effectObj.GetComponent<SpellEffect>();
                        if (enemy.ignoredElementTypes.Contains(effect.Type))
                        {
                            foreach (var type in enemy.ignoredElementTypes)
                            {
                                Debug.Log(type);
                            }
                            continue;
                        }
                        effectObj.GetComponent<SpellEffect>().Apply(enemy);
                    }
                }
                enemy.GetComponentInChildren<SpriteRenderer>().material.SetColor("_DissolveColor", ColorSteps[0]);
            }
        }
        yield return null;
    }

    protected override void TargetCollision(Collision2D collision)
    {
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
                    if (UnityEngine.Random.Range(0f, 1f) < attack.TileSpawnChance[Mathf.Clamp(Mathf.RoundToInt(distance) - 1, 0, attack.TileSpawnChance.Count-1)])
                    {
                        validTiles.Add(nextTile);
                    }
                }
            }
        }

       CustomGrid.Instance.SpawnTiles(validTiles, attack.ElementType);
    }
}

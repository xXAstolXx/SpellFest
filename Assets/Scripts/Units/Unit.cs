using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;


public abstract class Unit : MonoBehaviour, IAttackable
{
    public UnitMovement unitMovement {  get; private set; }
    public UnitHealth health { get; private set; }

    public List<Effect> tileEffects {  get; private set; } = new List<Effect>();
    public List<SpellEffect> spellEffects { get; private set; } = new List<SpellEffect>();

    public List<ElementType> ignoredElementTypes {  get; private set; }

    protected Vector3Int currentCellPosition;
    protected ElementType currentTileElement;

    protected Rigidbody2D rb;
    protected Collider2D col;


    protected virtual void Awake()
    {
        unitMovement = GetComponent<UnitMovement>();
        health = GetComponent<UnitHealth>();
        health.OnDie.AddListener(Die);

        rb = GetComponent<Rigidbody2D>();
        col = rb.GetComponent<Collider2D>();

        tileEffects = new List<Effect>();

        ignoredElementTypes = new List<ElementType>();
    }

    protected virtual void Start()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        Vector3Int cellPosition = new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), 0);
        currentCellPosition = cellPosition;
        currentTileElement = ElementType.NONE;
    }

    protected virtual void Update()
    {
        CheckCurrentTile();
    }

    protected virtual void CheckCurrentTile()
    {
        Vector3Int cellPosition = new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), 0);

        if (currentCellPosition != cellPosition)
        {
            currentCellPosition = cellPosition;
            UpdateEffects(cellPosition);
        }
        else if (CustomGrid.Instance.elementMap.HasTile(cellPosition))
        {
            TileData tileData = new TileData();
            CustomGrid.Instance.elementMap.GetTile(cellPosition).GetTileData(cellPosition, CustomGrid.Instance.elementMap, ref tileData);
            ElementType tileElement = CustomGrid.Instance.GetElement(cellPosition);

            if (tileElement != currentTileElement)
            {
                UpdateEffects(cellPosition);
            }
        }
    }

    protected virtual void UpdateEffects(Vector3Int cellPosition)
    {
        if (CustomGrid.Instance.elementMap.HasTile(cellPosition))
        {
            ElementType newTileElement = GetTileElementType(cellPosition);
            if (newTileElement != currentTileElement)
            {
                RemoveTileEffects();
                currentTileElement = newTileElement;

                TileData tileData = new TileData();
                CustomGrid.Instance.elementMap.GetTile(cellPosition).GetTileData(cellPosition, CustomGrid.Instance.elementMap, ref tileData);
                if(tileData.gameObject.GetComponent<ElementTile>().effects.Count != 0)
                {
                    tileEffects = new List<Effect>(tileData.gameObject.GetComponent<ElementTile>().effects);
                    ApplyTileEffects();
                }
            }
        }
        else
        {
            RemoveTileEffects();
            currentTileElement = ElementType.NONE;
        }
    }

    protected virtual void ApplyTileEffects()
    {
        foreach (Effect effect in tileEffects)
        {
            if (ignoredElementTypes.Contains(effect.Type))
            {
                continue;
            }
            effect.Apply(this);
        }
    }

    protected virtual void RemoveTileEffects()
    {
        foreach (Effect effect in tileEffects)
        {
            if (ignoredElementTypes.Contains(effect.Type))
            {
                continue;
            }
            effect.Remove(this);
        }
        tileEffects.Clear();
    }

    protected virtual ElementType GetTileElementType(Vector3Int cellPosition)
    {
        return CustomGrid.Instance.GetElement(cellPosition);
    }

    public abstract void Die();

    public virtual void StartSliding() { }
    public virtual void StopSliding() { }

    public abstract bool ReceiveAttack(float damage, AttackType damageType, float angle, Vector2 force, GameObject source);
    public abstract bool ReceiveAttack(float damage, AttackType damageType, GameObject source);

    public virtual void Heal(float value)
    {
        health.Heal(value);
    }
}

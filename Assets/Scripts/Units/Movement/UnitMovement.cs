using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class UnitMovement : MonoBehaviour
{
    [SerializeField]
    protected float speed;
    protected float internalSpeed;
    protected float speedModifier = 1;
    protected Vector2 moveDirection;
    protected Vector2 slideDirection;

    public Vector3 faceDirection {  get; protected set; }
    protected Vector3 oldPosition;

    public UnityEvent OnStartSliding { get; protected set; } = new UnityEvent();
    public UnityEvent OnStopSliding { get; protected set; } = new UnityEvent();

    public MovementType type { get; protected set; } = MovementType.NORMAL;

    public Vector3 lastFramePosition {  get; protected set; }

    protected Rigidbody2D rb;


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        lastFramePosition = transform.position;
        internalSpeed = speed;
    }

    protected virtual void Update() { }

    protected virtual void FixedUpdate()
    {
        switch (type)
        {
            case MovementType.NORMAL:
                Move();
                break;
            case MovementType.SLIDE:
                Slide();
                break;
        }
    }

    protected virtual void LateUpdate()
    {
        faceDirection = (transform.position - lastFramePosition).normalized;
        lastFramePosition = transform.position;
    }

    protected abstract void Move();

    protected virtual void Slide()
    {
        Vector2 slideVector = slideDirection * internalSpeed * Mathf.Clamp(speedModifier, GlobalSettings.Instance.UnitSpeedModifierMin, speedModifier) * Time.deltaTime;
        rb.AddForce(slideVector, ForceMode2D.Force);
    }

    public virtual void StartSliding()
    {
        type = MovementType.SLIDE;
        OnStartSliding.Invoke();
        slideDirection = (Vector2)(transform.position - lastFramePosition).normalized;
    }

    public virtual void StopSliding()
    {
        type = MovementType.NORMAL;
        slideDirection = Vector2.zero;
        OnStopSliding.Invoke();
    }

    public void ModifySpeed(float speedModifier)
    {
        this.speedModifier += speedModifier-1;
    }

    public void ResetSpeed(float speedModifier)
    {
        this.speedModifier -= speedModifier-1;
    }

    public virtual IEnumerator ApplyForce(Vector2 force)
    {
        yield return new WaitForFixedUpdate();
        rb.AddForce(force, ForceMode2D.Impulse);
    }
}

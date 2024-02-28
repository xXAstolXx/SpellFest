using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : UnitMovement
{
    float input;

    [SerializeField]
    float autoMoveSpeed;

    //lerp speed
    [SerializeField]
    private float lerpDuration;
    private float lerpT;
    private float lerpFactor;

    private Vector3 autoTargetPosition;
    private bool isAutoMoving;
    public UnityEvent OnTargetReached { get; private set; } = new UnityEvent();


    protected override void Initialize()
    {
        base.Initialize();

        lerpFactor = 0;
        lerpT = 0;
    }

    public void UpdateInput(Vector2 inputVector)
    {
        switch (type)
        {
            case MovementType.NORMAL:
                input = inputVector.magnitude;
                if (inputVector.magnitude != 0)
                {
                    moveDirection = inputVector;
                }
                break;
            case MovementType.SLIDE:
                moveDirection = inputVector;
                if (transform.position == lastFramePosition)
                {
                    slideDirection = inputVector;
                }
                break;
        }
    }

    protected override void Move()
    {
        Vector2 moveVector;

        if (isAutoMoving)
        {
            Vector3 direction = (autoTargetPosition - transform.position).normalized;
            moveVector = direction * internalSpeed * Time.fixedDeltaTime;
            if ((autoTargetPosition - transform.position).magnitude <= moveVector.magnitude)
            {
                isAutoMoving = false;
                OnTargetReached.Invoke();
            }
        }
        else
        {
            UpdateLerpFactor();

            float speed = internalSpeed * Time.fixedDeltaTime * Mathf.Clamp(speedModifier, GlobalSettings.Instance.UnitSpeedModifierMin, speedModifier);
            moveVector = moveDirection * speed * lerpFactor;
        }

        rb.AddForce(moveVector, ForceMode2D.Force);
    }

    public void StartAutoMove(Vector3 position)
    {
        autoTargetPosition = position;
        isAutoMoving = true;
        internalSpeed = autoMoveSpeed;
    }

    private void UpdateLerpFactor()
    {
        if (input > 0)
        {
            if (lerpFactor < 1)
            {
                lerpT += Time.fixedDeltaTime / lerpDuration;
                lerpFactor = Mathf.Lerp(0, 1, Mathf.Clamp01(lerpT));
            }
            else
            {
                lerpFactor = 1;
                lerpT = 1;
            }
        }
        else
        {
            if (lerpFactor > 0)
            {
                lerpT -= Time.fixedDeltaTime / lerpDuration;
                lerpFactor = Mathf.Lerp(0, 1, Mathf.Clamp01(lerpT));
            }
            else
            {
                lerpFactor = 0;
                lerpT = 0;
            }
        }
        Debug.Log("lerpT: "+lerpT);
        Debug.Log("lerpFactor: " + lerpFactor);
        Debug.Log(" Time.fixedDeltaTime / lerpDuration: " + Time.fixedDeltaTime / lerpDuration);
    }
}

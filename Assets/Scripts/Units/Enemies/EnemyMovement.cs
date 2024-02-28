using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyMovement : UnitMovement
{
    NavMeshAgent agent;
    Vector3 targetPosition;
    protected Animator animator;

    public UnityEvent OnAgentRestart {  get; private set; } = new UnityEvent();


    protected override void Awake()
    {
        base.Awake();

        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    protected override void Initialize()
    {
        base.Initialize();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.enabled = false;

        animator.SetBool("isWalking", false);
        animator.SetFloat("Horizontal", 0);
        animator.SetFloat("Vertical", 0);
    }

    protected override void Update()
    {
        base.Update();

        animator.SetFloat("Horizontal", faceDirection.x);
        animator.SetFloat("Vertical", faceDirection.y);
    }

    protected override void FixedUpdate()
    {
        oldPosition = transform.position;

        if (type == MovementType.SLIDE && agent.enabled && transform.position != lastFramePosition)
        {
            slideDirection = (Vector2)(transform.position - lastFramePosition).normalized;
            agent.enabled = false;
        }
        else if (type == MovementType.SLIDE && transform.position == lastFramePosition && agent.enabled == false)
        {
            agent.enabled = true;
            agent.SetDestination(targetPosition);
        }

        base.FixedUpdate();
    }

    protected override void Move()
    {
        if(agent.enabled)
        {
            agent.speed = internalSpeed * Mathf.Clamp(speedModifier, GlobalSettings.Instance.UnitSpeedModifierMin, speedModifier);
            agent.SetDestination(targetPosition);
        }
    }

    public override void StartSliding()
    {
        base.StartSliding();

        GetComponent<NavMeshAgent>().enabled = false;
        internalSpeed = speed * Mathf.Clamp(speedModifier, GlobalSettings.Instance.UnitSpeedModifierMin, speedModifier);
    }

    public override void StopSliding()
    {
        base.StopSliding();

        GetComponent<NavMeshAgent>().enabled = true;
        internalSpeed = speed;
    }

    public void RestartSliding()
    {
        if (agent.enabled && transform.position != lastFramePosition)
        {
            slideDirection = (Vector2)(transform.position - lastFramePosition).normalized;
            agent.enabled = false;
        }
        else if (transform.position == lastFramePosition && agent.enabled == false)
        {
            agent.enabled = true;
            agent.SetDestination(targetPosition);
        }
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        this.targetPosition = (Vector2)targetPosition;
    }

    public void StartMove()
    {
        GetComponent<NavMeshAgent>().enabled = true;
        internalSpeed = speed;
        animator.SetBool("isWalking", true);
    }

    public void StopMove()
    {
        GetComponent<NavMeshAgent>().enabled = false;
        internalSpeed = 0;
        animator.SetBool("isWalking", false);
    }

    public IEnumerator Pause(float time)
    {
        GetComponent<NavMeshAgent>().enabled = false;
        yield return new WaitForSeconds(time);
        GetComponent<NavMeshAgent>().enabled = true;
        OnAgentRestart?.Invoke();
    }
}

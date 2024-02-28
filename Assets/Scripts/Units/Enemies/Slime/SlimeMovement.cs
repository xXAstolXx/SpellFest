using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeMovement : EnemyMovement
{
    [SerializeField]
    protected float dashSpeed;


    public void StartDash()
    {
        internalSpeed = dashSpeed;
        animator.SetBool("isWalking", true);
    }
}

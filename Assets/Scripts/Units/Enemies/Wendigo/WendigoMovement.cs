using UnityEngine;

public class WendigoMovement : EnemyMovement
{
    [SerializeField]
    protected float dashSpeed;
    [SerializeField]
    protected float followSpeed;

    public bool isShaking = false;


    protected override void LateUpdate()
    {
        if(isShaking == false)
        {
            base.LateUpdate();
        }
    }

    public void StartDash(Vector3 targetPosition)
    {
        Debug.Log("Start dash");

        internalSpeed = dashSpeed;
        SetTargetPosition(targetPosition);
        animator.SetBool("isWalking", false);
        animator.SetBool("isSlashing", true);
    }

    public void StartFollow(Vector3 targetPosition)
    {
        Debug.Log("Start dash");

        internalSpeed = followSpeed;
        SetTargetPosition(targetPosition);
        animator.SetBool("isWalking", true);
    }
}

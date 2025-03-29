using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Animator animator;
    public PlayerMovement playerMovement;

    void Update()
    {
        if (playerMovement == null || animator == null)
            return;

        
        animator.SetBool("isWalk", playerMovement.isMoving);

        animator.SetBool("isJumping", !playerMovement.isGrounded);
    }
}

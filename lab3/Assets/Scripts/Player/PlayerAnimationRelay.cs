using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerAnimationRelay : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private PlayerController player;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (animator == null || player == null) return;

        float currentSpeedX = Mathf.Abs(player.Rb.linearVelocity.x);
        animator.SetFloat("velocityX", currentSpeedX);
        animator.SetFloat("velocityY", player.Rb.linearVelocity.y);

        animator.SetBool("isGrounded", player.IsGrounded());
        animator.SetBool("isWallSliding", player.StateMachine.CurrentState == player.WallSlideState);
        animator.SetBool("isDashing", player.StateMachine.CurrentState == player.DashState);

        if (player.InputX > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
        else if (player.InputX < -0.01f)
        {
            spriteRenderer.flipX = true;
        }
    }

    public void TriggerWallJump()
    {
        if (animator != null)
        {
            animator.SetTrigger("wallJumpTrigger");
        }
    }
}
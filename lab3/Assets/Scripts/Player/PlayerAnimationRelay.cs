using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerAnimationRelay : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private PlayerController player;

    private static readonly int VelocityXHash = Animator.StringToHash("velocityX");
    private static readonly int VelocityYHash = Animator.StringToHash("velocityY");
    private static readonly int IsGroundedHash = Animator.StringToHash("isGrounded");
    private static readonly int IsWallSlidingHash = Animator.StringToHash("isWallSliding");
    private static readonly int IsDashingHash = Animator.StringToHash("isDashing");
    private static readonly int WallJumpTriggerHash = Animator.StringToHash("wallJumpTrigger");
    private static readonly int HurtTriggerHash = Animator.StringToHash("Hurt");
    private static readonly int DeathTriggerHash = Animator.StringToHash("Death");
    private static readonly int ActionTriggerHash = Animator.StringToHash("Action");

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
        animator.SetFloat(VelocityXHash, currentSpeedX);
        animator.SetFloat(VelocityYHash, player.Rb.linearVelocity.y);

        animator.SetBool(IsGroundedHash, player.IsGrounded());
        animator.SetBool(IsWallSlidingHash, player.StateMachine.CurrentState == player.WallSlideState);
        animator.SetBool(IsDashingHash, player.StateMachine.CurrentState == player.DashState);

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
        if (animator != null) animator.SetTrigger(WallJumpTriggerHash);
    }

    public void TriggerHurt()
    {
        if (animator != null) animator.SetTrigger(HurtTriggerHash);
    }

    public void TriggerDeath()
    {
        if (animator != null) animator.SetTrigger(DeathTriggerHash);
    }

    public void TriggerAction()
    {
        if (animator != null) animator.SetTrigger(ActionTriggerHash);
    }

    public void OnHurtAnimationFinished()
    {
        Debug.Log("Hurt animation finished.");
    }

    public void OnDeathAnimationFinished()
    {
        Debug.Log("Death animation finished.");
    }

    public void OnJumpAnimationFinished()
    {
        Debug.Log("Jump animation finished.");
    }

    public void OnInteractionEvent(string actionName)
    {
        Debug.Log($"[Animation Event] Interaction executed successfully: {actionName} at timestamp {Time.time}");
    }
}
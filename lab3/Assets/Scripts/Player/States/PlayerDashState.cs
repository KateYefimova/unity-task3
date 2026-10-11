using UnityEngine;

public class PlayerDashState : PlayerState
{
    private float dashTimer;
    private float dashDirection;

    public PlayerDashState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        dashTimer = player.DashDuration;
        player.ResetDashCooldown();

        dashDirection = player.InputX != 0 ? Mathf.Sign(player.InputX) : player.FacingDirection;

        player.Rb.gravityScale = 0f;
        player.Rb.linearVelocity = new Vector2(dashDirection * player.DashSpeed, 0f);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0f)
        {
            if (player.IsGrounded())
                stateMachine.ChangeState(player.GroundedState);
            else
                stateMachine.ChangeState(player.AirState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.Rb.gravityScale = player.InitialGravityScale;
    }
}
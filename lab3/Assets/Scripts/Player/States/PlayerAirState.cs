using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override void HandleInput()
    {
        base.HandleInput();

        if (player.JumpBufferCounter > 0f && player.CoyoteTimeCounter > 0f)
        {
            player.ExecuteJump();
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (player.IsGrounded() && player.Rb.linearVelocity.y <= 0.05f)
        {
            stateMachine.ChangeState(player.GroundedState);
            return;
        }

        if (player.Rb.linearVelocity.y < 0 && player.IsTouchingWall(out int wallDir))
        {
            if ((player.InputX > 0 && wallDir == 1) || (player.InputX < 0 && wallDir == -1))
            {
                stateMachine.ChangeState(player.WallSlideState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        player.ApplyHorizontalMovement(player.AirAcceleration, player.AirDeceleration);
        player.ApplyJumpGravityModifiers();
    }
}
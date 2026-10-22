using UnityEngine;

public class PlayerWallSlideState : PlayerState
{
    public PlayerWallSlideState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (player.JumpBufferCounter > 0f)
        {
            stateMachine.ChangeState(player.WallJumpState);
            return;
        }

        if (player.IsGrounded())
        {
            stateMachine.ChangeState(player.GroundedState);
            return;
        }

        if (!player.IsTouchingWall(out _))
        {
            stateMachine.ChangeState(player.AirState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (player.Rb.linearVelocity.y < -player.WallSlideSpeed)
        {
            player.Rb.linearVelocity = new Vector2(player.Rb.linearVelocity.x, -player.WallSlideSpeed);
        }
    }
}
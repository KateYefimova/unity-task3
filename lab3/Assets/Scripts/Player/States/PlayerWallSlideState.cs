using UnityEngine;

public class PlayerWallSlideState : PlayerState
{
    public PlayerWallSlideState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (player.IsGrounded())
        {
            stateMachine.ChangeState(player.GroundedState);
            return;
        }

        if (!player.IsTouchingWall(out int wallDir) ||
            (wallDir == 1 && player.InputX <= 0) ||
            (wallDir == -1 && player.InputX >= 0))
        {
            stateMachine.ChangeState(player.AirState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        float clampedY = Mathf.Max(player.Rb.linearVelocity.y, -player.WallSlideSpeed);
        player.Rb.linearVelocity = new Vector2(player.Rb.linearVelocity.x, clampedY);
    }
}
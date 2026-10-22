using UnityEngine;

public class PlayerWallJumpState : PlayerState
{
    private float lockTimer;

    public PlayerWallJumpState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        player.ResetHorizontalSmoothing();

        int jumpDirection = -player.LastWallDirection;

        player.Rb.linearVelocity = new Vector2(player.WallJumpForce.x * jumpDirection, player.WallJumpForce.y);

        lockTimer = player.WallJumpControlLockTime;
        player.ConsumeJumpBuffer();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        lockTimer -= Time.deltaTime;

        if (lockTimer <= 0f)
        {
            if (player.IsGrounded())
                stateMachine.ChangeState(player.GroundedState);
            else
                stateMachine.ChangeState(player.AirState);
        }
    }

    public override void PhysicsUpdate()
    {
    }
}
using UnityEngine;

public class PlayerWallJumpState : PlayerState
{
    private float controlLockTimer;

    public PlayerWallJumpState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        controlLockTimer = player.WallJumpControlLockTime;

        int jumpDirection = -player.LastWallDirection;
        player.Rb.linearVelocity = new Vector2(jumpDirection * player.WallJumpForce.x, player.WallJumpForce.y);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        controlLockTimer -= Time.deltaTime;

        if (controlLockTimer <= 0f)
        {
            stateMachine.ChangeState(player.AirState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        player.ApplyJumpGravityModifiers();
    }
}
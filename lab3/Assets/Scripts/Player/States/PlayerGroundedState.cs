using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        player.ResetCoyoteTimer();
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (player.JumpBufferCounter > 0f)
        {
            player.ExecuteJump();
            stateMachine.ChangeState(player.AirState);
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!player.IsGrounded())
        {
            stateMachine.ChangeState(player.AirState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        player.ApplyHorizontalMovement(player.GroundAcceleration, player.GroundDeceleration);
    }
}
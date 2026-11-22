using UnityEngine;

/// <summary>
/// Extension point for enemy behaviours. A behaviour is a separate component,
/// so a new one (Charge, Retreat, Jump...) can be added without changing BaseEnemy.
/// Lifecycle: Initialize -> Enter -> Execute (every frame while IsBusy) -> Exit -> Restart.
/// </summary>
public interface IEnemyBehaviour
{
    /// <summary>True while the behaviour is running (from Enter until Restart).</summary>
    bool IsBusy { get; }

    void Initialize(Rigidbody2D enemyRb, Transform player);
    void Enter();
    void Execute();
    void Exit();
    void Restart();
}

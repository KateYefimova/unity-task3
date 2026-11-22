using UnityEngine;

public interface IEnemyBehaviour
{
    bool IsBusy { get; }

    void Initialize(Rigidbody2D enemyRb, Transform player);
    void Enter();
    void Execute();
    void Exit();
    void Restart();
}

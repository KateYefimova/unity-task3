using UnityEngine;

public class ChargerEnemy : BaseEnemy
{
    private IEnemyBehaviour behaviour;
    private bool behaviourInitialized;

    protected override void Awake()
    {
        base.Awake();
        behaviour = GetComponent<IEnemyBehaviour>();
        rb.gravityScale = 0f;

        if (behaviour == null)
        {
            Debug.LogError($"[ChargerEnemy] {name} has no IEnemyBehaviour component (e.g. ChargeBehaviour).", this);
        }
    }

    protected override void UpdateBehaviour()
    {
        if (behaviour == null) return;

        if (!behaviourInitialized)
        {
            if (playerTransform == null) return;
            behaviour.Initialize(rb, playerTransform);
            behaviourInitialized = true;
        }

        if (behaviour.IsBusy)
        {
            behaviour.Execute();
        }
        else if (PlayerInRange())
        {
            behaviour.Enter();
        }
    }

    protected override void HandlePlayerHit(GameObject target)
    {
        base.HandlePlayerHit(target);
        behaviour?.Exit();
    }

    protected override void ResetBehaviour()
    {
        if (behaviourInitialized) behaviour?.Restart();
    }
}

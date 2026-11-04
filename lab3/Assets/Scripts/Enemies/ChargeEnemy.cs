using UnityEngine;

[RequireComponent(typeof(ChargeBehaviour))]
public class ChargerEnemy : BaseEnemy
{
    private ChargeBehaviour chargeBehaviour;

    protected override void Awake()
    {
        base.Awake();
        chargeBehaviour = GetComponent<ChargeBehaviour>();
        rb.gravityScale = 0f;
    }

    protected override void Start()
    {
        base.Start();
        if (chargeBehaviour != null)
        {
            chargeBehaviour.Initialize(rb, playerTransform);
        }
    }

    protected override void UpdateBehaviour()
    {
        if (chargeBehaviour == null) return;
        
        if (chargeBehaviour.IsBusy)
        {
            chargeBehaviour.Execute();
        }
        else if (PlayerInRange())
        {
            chargeBehaviour.Enter();
        }
    }

    protected override void HandlePlayerHit(GameObject target)
    {
        base.HandlePlayerHit(target);
        if (chargeBehaviour != null)
        {
            chargeBehaviour.Exit();
        }
    }
}
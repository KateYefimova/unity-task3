using UnityEngine;

public class ChargeBehaviour : MonoBehaviour, IEnemyBehaviour
{
    [Header("Charge Settings")]
    [SerializeField] private float chargeSpeed = 8f;
    [SerializeField] private float returnSpeed = 3f;
    [SerializeField] private float windupDuration = 0.4f;
    [SerializeField] private float maxChargeDuration = 1.2f;
    [SerializeField] private float cooldownDuration = 1.2f;

    private enum Phase { Inactive, Windup, Charging, Returning, Cooldown }
    private Phase currentPhase = Phase.Inactive;

    private Rigidbody2D rb;
    private Transform targetPlayer;
    private Vector2 chargeDirection;
    private Vector2 initialPosition;
    private float phaseTimer;

    public bool IsBusy => currentPhase != Phase.Inactive;

    public void Initialize(Rigidbody2D enemyRb, Transform player)
    {
        rb = enemyRb;
        targetPlayer = player;
        initialPosition = rb.position;
    }

    public void Enter()
    {
        if (targetPlayer == null || rb == null) return;

        currentPhase = Phase.Windup;
        phaseTimer = windupDuration;
        rb.linearVelocity = Vector2.zero;
        
        chargeDirection = ((Vector2)targetPlayer.position - rb.position).normalized;

        Debug.Log($"[EnemyBehaviour] ENTER: Charge windup started towards {chargeDirection}");
    }

    public void Execute()
    {
        phaseTimer -= Time.deltaTime;

        switch (currentPhase)
        {
            case Phase.Windup:
                if (phaseTimer <= 0f)
                {
                    currentPhase = Phase.Charging;
                    phaseTimer = maxChargeDuration;
                    Debug.Log("[EnemyBehaviour] EXECUTE: Charging forward!");
                }
                break;

            case Phase.Charging:
                rb.linearVelocity = chargeDirection * chargeSpeed;

                if (phaseTimer <= 0f)
                {
                    Exit();
                }
                break;

            case Phase.Returning:
                Vector2 toStart = initialPosition - rb.position;
                if (toStart.sqrMagnitude > 0.05f)
                {
                    rb.linearVelocity = toStart.normalized * returnSpeed;
                }
                else
                {
                    rb.position = initialPosition;
                    rb.linearVelocity = Vector2.zero;
                    currentPhase = Phase.Cooldown;
                    phaseTimer = cooldownDuration;
                    Debug.Log("[EnemyBehaviour] Cooldown initiated at base position.");
                }
                break;

            case Phase.Cooldown:
                rb.linearVelocity = Vector2.zero;
                if (phaseTimer <= 0f)
                {
                    Restart();
                }
                break;
        }
    }

    public void Exit()
    {
        if (currentPhase == Phase.Returning || currentPhase == Phase.Cooldown || currentPhase == Phase.Inactive) return;

        currentPhase = Phase.Returning;
        rb.linearVelocity = Vector2.zero;
        Debug.Log("[EnemyBehaviour] EXIT: Charge ended. Returning to start position.");
    }

    public void Restart()
    {
        currentPhase = Phase.Inactive;
        phaseTimer = 0f;
        rb.position = initialPosition;
        rb.linearVelocity = Vector2.zero;

        Debug.Log("[EnemyBehaviour] RESTART: Reset complete. Ready for new target.");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentPhase == Phase.Charging && !collision.gameObject.CompareTag("Player"))
        {
            Exit();
        }
    }
}
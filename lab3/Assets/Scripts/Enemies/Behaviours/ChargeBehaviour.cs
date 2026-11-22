using UnityEngine;

/// <summary>
/// Charge behaviour: windup -> dash towards the player -> return to the start point -> cooldown -> ready again.
/// Every lifecycle step writes a [EnemyBehaviour] log so it can be followed in the Console while playing.
/// </summary>
public class ChargeBehaviour : MonoBehaviour, IEnemyBehaviour
{
    [Header("Charge Settings")]
    [SerializeField] private float chargeSpeed = 8f;
    [SerializeField] private float returnSpeed = 3f;
    [SerializeField] private float windupDuration = 0.4f;
    [SerializeField] private float maxChargeDuration = 1.2f;
    [SerializeField] private float maxReturnDuration = 3f;
    [SerializeField] private float cooldownDuration = 1.2f;

    [Header("Windup Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color windupColor = Color.red;
    [SerializeField] private float windupBlinkSpeed = 16f;

    private enum Phase { Inactive, Windup, Charging, Returning, Cooldown }
    private Phase currentPhase = Phase.Inactive;

    private Rigidbody2D rb;
    private Transform targetPlayer;
    private Vector2 chargeDirection;
    private Vector2 initialPosition;
    private float phaseTimer;
    private Color baseColor = Color.white;

    public bool IsBusy => currentPhase != Phase.Inactive;

    public void Initialize(Rigidbody2D enemyRb, Transform player)
    {
        rb = enemyRb;
        targetPlayer = player;
        initialPosition = rb.position;

        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) baseColor = spriteRenderer.color;
    }

    public void Enter()
    {
        if (targetPlayer == null || rb == null || IsBusy) return;

        currentPhase = Phase.Windup;
        phaseTimer = windupDuration;
        rb.linearVelocity = Vector2.zero;

        chargeDirection = ((Vector2)targetPlayer.position - rb.position).normalized;

        Debug.Log($"[EnemyBehaviour] ENTER: {name} charge windup towards {chargeDirection}");
    }

    public void Execute()
    {
        phaseTimer -= Time.deltaTime;

        switch (currentPhase)
        {
            case Phase.Windup:
                BlinkWindup();
                if (phaseTimer <= 0f)
                {
                    SetColor(baseColor);
                    currentPhase = Phase.Charging;
                    phaseTimer = maxChargeDuration;
                    Debug.Log($"[EnemyBehaviour] EXECUTE: {name} charging forward");
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
                // Timeout protects from getting stuck forever if a wall blocks the way back.
                if (toStart.sqrMagnitude > 0.05f && phaseTimer > 0f)
                {
                    rb.linearVelocity = toStart.normalized * returnSpeed;
                }
                else
                {
                    rb.position = initialPosition;
                    rb.linearVelocity = Vector2.zero;
                    currentPhase = Phase.Cooldown;
                    phaseTimer = cooldownDuration;
                    Debug.Log($"[EnemyBehaviour] {name} back at start, cooldown {cooldownDuration}s");
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
        if (currentPhase != Phase.Windup && currentPhase != Phase.Charging) return;

        SetColor(baseColor);
        currentPhase = Phase.Returning;
        phaseTimer = maxReturnDuration;
        rb.linearVelocity = Vector2.zero;
        Debug.Log($"[EnemyBehaviour] EXIT: {name} charge ended, returning to start");
    }

    public void Restart()
    {
        currentPhase = Phase.Inactive;
        phaseTimer = 0f;
        SetColor(baseColor);

        if (rb != null)
        {
            rb.position = initialPosition;
            rb.linearVelocity = Vector2.zero;
        }

        Debug.Log($"[EnemyBehaviour] RESTART: {name} ready for the next charge");
    }

    private void BlinkWindup()
    {
        bool on = Mathf.PingPong(Time.time * windupBlinkSpeed, 1f) > 0.5f;
        SetColor(on ? windupColor : baseColor);
    }

    private void SetColor(Color color)
    {
        if (spriteRenderer != null) spriteRenderer.color = color;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentPhase == Phase.Charging && !collision.gameObject.CompareTag("Player"))
        {
            Exit();
        }
    }
}

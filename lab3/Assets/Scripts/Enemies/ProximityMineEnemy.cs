using UnityEngine;

public class ProximityMineEnemy : BaseEnemy
{
    private enum MineState { Idle, Triggered, Charging, Cooldown }

    [Header("Proximity Settings")]
    [SerializeField] private float warningDuration = 0.45f;
    [SerializeField] private float chargeSpeed = 6f;
    [SerializeField] private float cooldownDuration = 1.2f;

    [Header("Animation & Visuals")]
    [SerializeField] private Animator mineAnimator;
    [SerializeField] private Color idleColor = Color.white;
    [SerializeField] private Color warningColor = Color.red;

    private MineState currentState = MineState.Idle;
    private float timer;
    private Vector2 targetDirection;
    private SpriteRenderer spriteRenderer;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (mineAnimator == null) mineAnimator = GetComponent<Animator>();
        if (spriteRenderer != null) spriteRenderer.color = idleColor;

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
    }

    protected override void UpdateBehaviour()
    {
        switch (currentState)
        {
            case MineState.Idle:
                UpdateAnimatorState(0); // 0 = Idle

                if (PlayerInRange())
                {
                    currentState = MineState.Triggered;
                    timer = warningDuration;
                    if (spriteRenderer != null) spriteRenderer.color = warningColor;
                }
                break;

            case MineState.Triggered:
                UpdateAnimatorState(1); // 1 = Warning 
                timer -= Time.deltaTime;

                if (spriteRenderer != null)
                {
                    spriteRenderer.color = (Mathf.PingPong(Time.time * 14f, 1f) > 0.5f) ? warningColor : idleColor;
                }

                if (timer <= 0f)
                {
                    LaunchCharge();
                }
                break;

            case MineState.Charging:
                UpdateAnimatorState(2); // 2 = Charge
                break;

            case MineState.Cooldown:
                UpdateAnimatorState(0); // 0 = Повернення в Idle під час перезарядки
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    currentState = MineState.Idle;
                    if (spriteRenderer != null) spriteRenderer.color = idleColor;
                }
                break;
        }
    }

    private void LaunchCharge()
    {
        currentState = MineState.Charging;
        UpdateAnimatorState(2);

        if (playerTransform != null)
        {
            targetDirection = ((Vector2)playerTransform.position - rb.position).normalized;
        }
        else
        {
            targetDirection = Vector2.down;
        }

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.linearVelocity = targetDirection * chargeSpeed;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        ResetAfterHit();
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        ResetAfterHit();
    }

    private void ResetAfterHit()
    {
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        currentState = MineState.Cooldown;
        timer = cooldownDuration;
        UpdateAnimatorState(0);
        if (spriteRenderer != null) spriteRenderer.color = idleColor;
    }

    private void UpdateAnimatorState(int stateIndex)
    {
        if (mineAnimator != null)
        {
            mineAnimator.SetInteger("state", stateIndex);
        }
    }
}
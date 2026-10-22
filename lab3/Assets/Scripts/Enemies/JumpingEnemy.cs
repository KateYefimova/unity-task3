using UnityEngine;

public class JumpingEnemy : BaseEnemy
{
    [Header("Jump Tuning")]
    [SerializeField] private float jumpForceY = 10f;
    [SerializeField] private float jumpForceX = 3.5f;
    [SerializeField] private float jumpCooldown = 2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Level 2 / Aggressive Jump Settings")]
    [Tooltip("Чи дозволено подвійний стрибок у повітрі для 2-го рівня")]
    [SerializeField] private bool canDoubleJump = false;

    [Tooltip("Затримка перед другим стрибком у повітрі")]
    [SerializeField] private float doubleJumpDelay = 0.4f;

    [Tooltip("Множник горизонтальної сили, якщо гравець далеко")]
    [SerializeField] private float aggressiveXMultiplier = 1f;

    [Tooltip("Візуальне стискання перед стрибком")]
    [SerializeField] private bool enableJumpSquish = false;

    private float cooldownTimer;
    private bool isGrounded;
    private bool hasDoubleJumped;
    private float airTimer;

    protected override void Awake()
    {
        base.Awake();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 3f;
        rb.freezeRotation = true;
    }

    protected override void UpdateBehaviour()
    {
        cooldownTimer -= Time.deltaTime;
        CheckGrounded();

        if (isGrounded)
        {
            hasDoubleJumped = false;
            airTimer = 0f;

            if (cooldownTimer <= 0f && PlayerInRange())
            {
                if (enableJumpSquish)
                {
                    StartCoroutine(SquishAndJumpRoutine());
                }
                else
                {
                    ExecuteJumpToPlayer();
                }
            }
        }
        else if (canDoubleJump && !hasDoubleJumped && PlayerInRange())
        {
            airTimer += Time.deltaTime;
            if (airTimer >= doubleJumpDelay && rb.linearVelocity.y < 2f)
            {
                ExecuteDoubleJump();
            }
        }
    }

    private void CheckGrounded()
    {
        Vector2 origin = new Vector2(col.bounds.center.x, col.bounds.min.y);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 0.2f, groundLayer);
        isGrounded = hit.collider != null;
    }

    private void ExecuteJumpToPlayer()
    {
        if (playerTransform == null) return;

        float directionToPlayer = Mathf.Sign(playerTransform.position.x - transform.position.x);
        float finalForceX = jumpForceX * aggressiveXMultiplier;

        rb.linearVelocity = new Vector2(directionToPlayer * finalForceX, jumpForceY);
        cooldownTimer = jumpCooldown;
    }

    private void ExecuteDoubleJump()
    {
        if (playerTransform == null) return;

        hasDoubleJumped = true;
        float directionToPlayer = Mathf.Sign(playerTransform.position.x - transform.position.x);

        rb.linearVelocity = new Vector2(directionToPlayer * jumpForceX * 1.3f, jumpForceY * 0.75f);
    }

    private System.Collections.IEnumerator SquishAndJumpRoutine()
    {
        cooldownTimer = jumpCooldown;
        Vector3 origScale = transform.localScale;

        transform.localScale = new Vector3(origScale.x * 1.25f, origScale.y * 0.7f, origScale.z);
        yield return new WaitForSeconds(0.12f);

        transform.localScale = origScale;
        ExecuteJumpToPlayer();
    }
}
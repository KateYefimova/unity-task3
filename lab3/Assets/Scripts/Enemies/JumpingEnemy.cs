using UnityEngine;

public class JumpingEnemy : BaseEnemy
{
    [Header("Jump Tuning")]
    [SerializeField] private float jumpForceY = 10f;
    [SerializeField] private float jumpForceX = 3.5f;
    [SerializeField] private float jumpCooldown = 2f;
    [SerializeField] private LayerMask groundLayer;

    private float cooldownTimer;
    private bool isGrounded;

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

        if (isGrounded && cooldownTimer <= 0f && PlayerInRange())
        {
            ExecuteJumpToPlayer();
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
        rb.linearVelocity = new Vector2(directionToPlayer * jumpForceX, jumpForceY);
        cooldownTimer = jumpCooldown;
    }
}
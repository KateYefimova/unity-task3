using UnityEngine;

public class PatrolEnemy : BaseEnemy
{
    [Header("Patrol Tuning")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float ledgeCheckDistance = 1.0f;
    [SerializeField] private float wallCheckDistance = 0.4f;

    private int direction = 1; 

    protected override void Awake()
    {
        base.Awake();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 2f;
        rb.freezeRotation = true;
    }

    protected override void UpdateBehaviour()
    {
        Patrol();
    }

    private void Patrol()
    {
        Vector2 center = col.bounds.center;

        RaycastHit2D wallHit = Physics2D.Raycast(
            center, 
            Vector2.right * direction, 
            col.bounds.extents.x + wallCheckDistance, 
            groundLayer
        );

        Vector2 ledgeOrigin = new Vector2(center.x + (col.bounds.extents.x * direction), col.bounds.min.y + 0.1f);
        RaycastHit2D ledgeHit = Physics2D.Raycast(
            ledgeOrigin, 
            Vector2.down, 
            ledgeCheckDistance, 
            groundLayer
        );

        if (wallHit.collider != null || ledgeHit.collider == null)
        {
            direction *= -1;
            transform.localScale = new Vector3(direction * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    private void OnDrawGizmos()
    {
        if (col == null) col = GetComponent<Collider2D>();
        if (col == null) return;

        Vector2 center = col.bounds.center;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(center, center + (Vector2.right * direction * (col.bounds.extents.x + wallCheckDistance)));

        Vector2 ledgeOrigin = new Vector2(center.x + (col.bounds.extents.x * direction), col.bounds.min.y + 0.1f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(ledgeOrigin, ledgeOrigin + Vector2.down * ledgeCheckDistance);
    }
}
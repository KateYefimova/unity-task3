using UnityEngine;

public class HomingDroneEnemy : BaseEnemy
{
    [Header("Homing Drone Settings")]
    [SerializeField] private float acceleration = 5.5f;
    [SerializeField] private float drag = 1.5f;
    [SerializeField] private float maxSpeed = 4f;

    private Vector2 homePosition;

    protected override void Awake()
    {
        base.Awake();
        homePosition = transform.position;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    protected override void UpdateBehaviour()
    {
        if (PlayerInRange())
        {
            Vector2 toPlayer = ((Vector2)playerTransform.position - rb.position).normalized;
            Vector2 targetVelocity = toPlayer * maxSpeed;
            rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVelocity, acceleration * Time.deltaTime);
        }
        else
        {
            Vector2 toHome = homePosition - rb.position;
            if (toHome.magnitude > 0.2f)
            {
                Vector2 returnVelocity = toHome.normalized * (moveSpeed * 0.6f);
                rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, returnVelocity, drag * Time.deltaTime);
            }
            else
            {
                rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, Vector2.zero, drag * Time.deltaTime);
            }
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        rb.linearVelocity = Vector2.zero;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        rb.linearVelocity = Vector2.zero;
    }
}
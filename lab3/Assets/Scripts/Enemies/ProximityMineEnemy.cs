using UnityEngine;

public class ProximityMineEnemy : BaseEnemy
{
    private enum MineState { Idle, Triggered, Charging }

    [Header("Proximity Settings")]
    [SerializeField] private float detectionRadius = 5.5f;
    [SerializeField] private float warningDuration = 0.45f;
    [SerializeField] private float chargeSpeed = 16f;

    [Header("Respawn & Damage")]
    [SerializeField] private Transform spawnPoint; // ����� ����������� ������

    [Header("Visual Feedback")]
    [SerializeField] private Color idleColor = Color.white;
    [SerializeField] private Color warningColor = Color.red;

    private MineState currentState = MineState.Idle;
    private float warningTimer;
    private Vector2 targetDirection;
    private SpriteRenderer spriteRenderer;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) spriteRenderer.color = idleColor;

        // � Unity 2D ��� ������ ���������� ������������� bodyType
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    protected override void UpdateBehaviour()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        switch (currentState)
        {
            case MineState.Idle:
                if (distanceToPlayer <= detectionRadius)
                {
                    currentState = MineState.Triggered;
                    warningTimer = warningDuration;
                    if (spriteRenderer != null) spriteRenderer.color = warningColor;
                }
                break;

            case MineState.Triggered:
                warningTimer -= Time.deltaTime;
                // ������� ����� ������
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = (Mathf.PingPong(Time.time * 14f, 1f) > 0.5f) ? warningColor : idleColor;
                }

                if (warningTimer <= 0f)
                {
                    currentState = MineState.Charging;
                    targetDirection = (playerTransform.position - transform.position).normalized;
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    rb.linearVelocity = targetDirection * chargeSpeed;
                }
                break;

            case MineState.Charging:
                break;
        }
    }

    // ��������� ������� ������������
    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandlePlayerHit(collision.gameObject);

        // ������������ �� ������ ��� ����� ������������� ����
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        currentState = MineState.Idle;
        if (spriteRenderer != null) spriteRenderer.color = idleColor;
    }

    // �� ������, ���� ��������� �������� ��� �������
    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandlePlayerHit(collision.gameObject);
    }

    private void HandlePlayerHit(GameObject target)
    {
        if (target.CompareTag("Player") || target.GetComponent<PlayerController>() != null)
        {
            // ���������� ������ �� �����
            if (spawnPoint != null)
            {
                target.transform.position = spawnPoint.position;
            }
            else
            {
                // ����� ����� ������ �� �����, ���� ���� � ���������� ����
                GameObject foundSpawn = GameObject.Find("SpawnPoint");
                if (foundSpawn != null) target.transform.position = foundSpawn.transform.position;
            }

            // ����� ���������� �������� ������
            Rigidbody2D playerRb = target.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
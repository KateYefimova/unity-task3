using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public abstract class BaseEnemy : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField] protected float detectionRadius = 5f;

    [Header("Respawn Settings")]
    [SerializeField] protected Transform spawnPoint;

    protected Rigidbody2D rb;
    protected Collider2D col;
    protected Transform playerTransform;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        FindPlayer();

        if (spawnPoint == null)
        {
            GameObject sp = GameObject.Find("SpawnPoint");
            if (sp != null) spawnPoint = sp.transform;
        }
    }

    private void FindPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) playerTransform = p.transform;
    }

    public bool PlayerInRange()
    {
        if (playerTransform == null)
        {
            FindPlayer();
            if (playerTransform == null) return false;
        }
        return Vector2.Distance(transform.position, playerTransform.position) <= detectionRadius;
    }

    protected abstract void UpdateBehaviour();

    protected virtual void Update()
    {
        if (playerTransform == null) FindPlayer();
        UpdateBehaviour();
    }

    protected virtual void HandlePlayerHit(GameObject target)
    {
        if (!target.CompareTag("Player") && target.GetComponent<PlayerController>() == null) return;

        if (spawnPoint != null)
        {
            target.transform.position = spawnPoint.position;
        }
        else
        {
            GameObject sp = GameObject.Find("SpawnPoint");
            if (sp != null) target.transform.position = sp.transform.position;
        }

        Rigidbody2D prb = target.GetComponent<Rigidbody2D>();
        if (prb != null)
        {
            prb.linearVelocity = Vector2.zero;
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D c) => HandlePlayerHit(c.gameObject);
    protected virtual void OnTriggerEnter2D(Collider2D o) => HandlePlayerHit(o.gameObject);

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
    
    protected virtual void FixedBehaviour() { }

    protected virtual void FixedUpdate()
    {
        FixedBehaviour();
    }
}
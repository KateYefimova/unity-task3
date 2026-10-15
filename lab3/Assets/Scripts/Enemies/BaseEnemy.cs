using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public abstract class BaseEnemy : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField] protected float detectionRadius = 5f;

    [Header("Spawn Settings")]
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

    protected bool PlayerInRange()
    {
        return playerTransform != null &&
               Vector2.Distance(transform.position, playerTransform.position) <= detectionRadius;
    }

    protected abstract void UpdateBehaviour(); 
    protected virtual void FixedBehaviour() { }

    protected virtual void Update()
    {
        if (playerTransform == null) { FindPlayer(); return; }
        UpdateBehaviour();
    }

    protected virtual void FixedUpdate()
    {
        if (playerTransform == null) return;
        FixedBehaviour();
    }

    protected virtual void HandlePlayerHit(GameObject target)
    {
        if (!target.CompareTag("Player") || spawnPoint == null) return;

        target.transform.position = spawnPoint.position;
        Rigidbody2D prb = target.GetComponent<Rigidbody2D>();
        if (prb != null) prb.linearVelocity = Vector2.zero;
    }

    protected virtual void OnCollisionEnter2D(Collision2D c) => HandlePlayerHit(c.gameObject);
    protected virtual void OnTriggerEnter2D(Collider2D o) => HandlePlayerHit(o.gameObject);

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
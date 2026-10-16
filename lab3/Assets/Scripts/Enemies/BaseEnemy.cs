using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class BaseEnemy : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField] protected float detectionRadius = 5f;
    [SerializeField] protected Transform spawnPoint;

    protected Rigidbody2D rb;
    protected Collider2D col;
    protected Transform playerTransform;

    public bool PlayerInRange()
    {
        if (playerTransform == null) return false;
        return Vector2.Distance(transform.position, playerTransform.position) <= detectionRadius;
    }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    protected virtual void Start()
    {
        FindPlayer();
        FindSpawnPoint();
    }

    protected virtual void Update()
    {
        if (playerTransform == null)
        {
            FindPlayer();
        }

        UpdateBehaviour();
    }

    protected virtual void FixedUpdate()
    {
        FixedBehaviour();
    }

    protected abstract void UpdateBehaviour();

    protected virtual void FixedBehaviour() { }

    protected virtual void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    protected virtual void FindSpawnPoint()
    {
        if (spawnPoint == null)
        {
            GameObject sp = GameObject.Find("SpawnPoint");
            if (sp != null)
            {
                spawnPoint = sp.transform;
            }
        }
    }

    protected virtual void HandlePlayerHit(GameObject target)
    {
        PlayerController pc = target.GetComponent<PlayerController>();
        if (!target.CompareTag("Player") && pc == null) return;

        Vector2 targetPos = spawnPoint != null ? (Vector2)spawnPoint.position : Vector2.zero;

        if (pc != null)
        {
            pc.Respawn(targetPos);
        }
        else
        {
            target.transform.position = targetPos;
            Rigidbody2D prb = target.GetComponent<Rigidbody2D>();
            if (prb != null)
            {
                prb.linearVelocity = Vector2.zero;
            }
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerHit(collision.gameObject);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HandlePlayerHit(other.gameObject);
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
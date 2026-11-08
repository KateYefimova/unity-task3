using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    [Header("Fall Death Boundary")]
    [SerializeField] private float fallBoundaryY = -15f;

    private PlayerController player;
    private PlayerAnimationRelay animationRelay; 
    private Rigidbody2D rb;
    private Collider2D col;

    public int CurrentHealth => currentHealth;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        animationRelay = GetComponent<PlayerAnimationRelay>(); 
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (transform.position.y < fallBoundaryY)
        {
            RespawnAtCheckpoint();
        }
    }

    public void TakeDamage(int damageAmount, Vector2 hitPosition)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            RespawnAtCheckpoint();
        }
        else
        {
            ApplyKnockback(hitPosition);

            if (animationRelay != null)
            {
                animationRelay.TriggerHurt();
            }
        }
    }

    private void ApplyKnockback(Vector2 hitPosition)
    {
        if (rb == null) return;

        float direction = transform.position.x > hitPosition.x ? 1f : -1f;
        Vector2 knockbackForce = new Vector2(direction * 7f, 8f);

        rb.linearVelocity = knockbackForce;
    }

    public void RespawnAtCheckpoint()
    {
        currentHealth = maxHealth;
        GameObject sp = GameObject.Find("SpawnPoint");
        Vector2 targetPos = sp != null ? (Vector2)sp.transform.position : Vector2.zero;
        StartCoroutine(DeathAndRespawnRoutine(targetPos));
    }

    private IEnumerator DeathAndRespawnRoutine(Vector2 spawnPosition)
    {
        if (animationRelay != null)
        {
            animationRelay.TriggerDeath();
        }

        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (col == null) col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        yield return new WaitForSeconds(0.4f);

        transform.localScale = Vector3.one;
        transform.position = spawnPosition;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (col != null) col.enabled = true;

        if (player.StateMachine != null && player.AirState != null)
        {
            player.StateMachine.ChangeState(player.AirState);
        }
    }
}
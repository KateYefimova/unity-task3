using UnityEngine;

/// <summary>
/// Entry point for everything that hits the player (enemy contact, hazards).
/// Turns a hit into damage, and if the player survives: knockback + Hurt animation.
/// Short invulnerability prevents one contact from hitting several times in a row.
/// </summary>
[RequireComponent(typeof(PlayerHealth), typeof(Rigidbody2D))]
public class PlayerHitHandler : MonoBehaviour
{
    [SerializeField] private Vector2 knockbackForce = new Vector2(7f, 8f);
    [SerializeField] private float invulnerabilityDuration = 0.5f;

    private PlayerHealth health;
    private Rigidbody2D rb;
    private PlayerAnimationRelay animationRelay;
    private float invulnerableUntil;

    private void Awake()
    {
        health = GetComponent<PlayerHealth>();
        rb = GetComponent<Rigidbody2D>();
        animationRelay = GetComponent<PlayerAnimationRelay>();
    }

    /// <summary>Enemy contact: damage from a point in the world (used for the knockback direction).</summary>
    public void ReceiveHit(int damage, Vector2 sourcePosition)
    {
        if (health.IsDead || Time.time < invulnerableUntil) return;

        invulnerableUntil = Time.time + invulnerabilityDuration;
        health.TakeDamage(damage);

        if (health.IsDead) return;

        ApplyKnockback(sourcePosition);
        if (animationRelay != null) animationRelay.TriggerHurt();
    }

    /// <summary>Hazards such as spikes or pits: always lethal, no knockback.</summary>
    public void ReceiveLethalHit()
    {
        health.Kill();
    }

    private void ApplyKnockback(Vector2 sourcePosition)
    {
        float direction = transform.position.x > sourcePosition.x ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * knockbackForce.x, knockbackForce.y);
    }
}

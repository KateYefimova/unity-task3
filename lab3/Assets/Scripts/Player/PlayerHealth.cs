using System;
using UnityEngine;

/// <summary>
/// Only stores the player's health. Does not move the player, play animations or respawn:
/// other components react to the Damaged / Died events.
///   PlayerHitHandler - contacts with enemies/hazards, knockback, Hurt animation
///   PlayerRespawn    - Death animation, fall boundary, teleport to the spawn point
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;

    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0;

    /// <summary>Health went down but the player is still alive. Argument: health left.</summary>
    public event Action<int> Damaged;
    public event Action Died;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (IsDead || amount <= 0) return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);

        if (IsDead) Died?.Invoke();
        else Damaged?.Invoke(CurrentHealth);
    }

    public void Kill()
    {
        if (IsDead) return;

        CurrentHealth = 0;
        Died?.Invoke();
    }

    public void ResetHealth()
    {
        CurrentHealth = maxHealth;
    }
}

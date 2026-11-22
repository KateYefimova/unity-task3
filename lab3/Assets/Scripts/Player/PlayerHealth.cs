using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;

    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0;

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

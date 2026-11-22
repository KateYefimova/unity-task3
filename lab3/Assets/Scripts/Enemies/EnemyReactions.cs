using UnityEngine;

/// <summary>
/// Hurt / Death reactions for an enemy, driven by the Animator (no health/combat system this week).
/// Hurt:  trigger "Hurt"  -> Hurt clip -> Animation Event OnHurtAnimationFinished -> back to idle.
/// Death: enemy logic and collider are switched off -> trigger "Death" -> Death clip ->
///        Animation Event OnDeathAnimationFinished -> sprite hidden -> optional revive after a delay.
/// The test keys only affect enemies near the player, so one key press does not kill every enemy in the level.
/// </summary>
[RequireComponent(typeof(BaseEnemy))]
public class EnemyReactions : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Test Triggers")]
    [SerializeField] private KeyCode hurtKey = KeyCode.J;
    [SerializeField] private KeyCode deathKey = KeyCode.L;
    [SerializeField] private float testTriggerRadius = 8f;

    [Header("Death")]
    [SerializeField] private bool reviveAfterDeath = true;
    [SerializeField] private float reviveDelay = 1.5f;
    [Tooltip("If the Death Animation Event never arrives (clip/transition changed), finish death after this time.")]
    [SerializeField] private float deathEventTimeout = 1.5f;

    private static readonly int HurtTrigger = Animator.StringToHash("Hurt");
    private static readonly int DeathTrigger = Animator.StringToHash("Death");

    private BaseEnemy enemy;
    private Transform player;
    private Vector3 initialScale;
    private bool isDead;
    private bool deathFinished;

    public bool IsDead => isDead;

    private void Awake()
    {
        enemy = GetComponent<BaseEnemy>();
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        initialScale = transform.localScale;
    }

    private void Update()
    {
        if (!PlayerIsNear()) return;

        if (Input.GetKeyDown(hurtKey)) PlayHurt();
        if (Input.GetKeyDown(deathKey)) PlayDeath();
    }

    public void PlayHurt()
    {
        if (isDead || animator == null) return;

        animator.SetTrigger(HurtTrigger);
        Debug.Log($"[EnemyReaction] HURT start: {name}");
    }

    public void PlayDeath()
    {
        if (isDead) return;

        isDead = true;
        deathFinished = false;
        enemy.SetLogicActive(false);

        if (animator != null)
        {
            animator.ResetTrigger(HurtTrigger);
            animator.SetTrigger(DeathTrigger);
        }

        Debug.Log($"[EnemyReaction] DEATH start: {name}");
        Invoke(nameof(DeathEventTimeout), deathEventTimeout);
    }

    // Animation Event at the end of the Hurt clip.
    public void OnHurtAnimationFinished()
    {
        Debug.Log($"[EnemyReaction] HURT finished: {name}");
    }

    // Animation Event at the end of the Death clip.
    public void OnDeathAnimationFinished()
    {
        if (!isDead || deathFinished) return;
        deathFinished = true;
        CancelInvoke(nameof(DeathEventTimeout));

        if (spriteRenderer != null) spriteRenderer.enabled = false;
        Debug.Log($"[EnemyReaction] DEATH finished: {name}");

        if (reviveAfterDeath) Invoke(nameof(Revive), reviveDelay);
    }

    private void DeathEventTimeout()
    {
        Debug.LogWarning($"[EnemyReaction] {name}: Death Animation Event was not received, finishing death by timeout.");
        OnDeathAnimationFinished();
    }

    private void Revive()
    {
        // Restore scale before Rebind, otherwise the Animator would take the "shrunk" scale as default.
        transform.localScale = initialScale;
        if (spriteRenderer != null) spriteRenderer.enabled = true;

        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }

        isDead = false;
        enemy.SetLogicActive(true);
        Debug.Log($"[EnemyReaction] REVIVED: {name}");
    }

    private bool PlayerIsNear()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj == null) return false;
            player = playerObj.transform;
        }

        return Vector2.Distance(transform.position, player.position) <= testTriggerRadius;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, testTriggerRadius);
    }
}

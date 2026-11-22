using UnityEngine;

/// <summary>
/// Interaction ("Use") scenario of the player. No real action this week, only a Debug.Log:
///   E pressed          -> START, Animator trigger "Action" (Player_Action state)
///   Animation Event    -> OnInteractionImpact at 0.25s: the moment the action "happens" (one log)
///   Animation Event    -> OnInteractionFinished at the end of the clip: interaction can be started again
/// If the Action state is interrupted (jump, death...) the end event never comes,
/// so a safety timeout unlocks the interaction instead of leaving it stuck.
/// </summary>
[RequireComponent(typeof(PlayerAnimationRelay))]
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private string interactionName = "Use";
    [SerializeField] private float safetyTimeout = 1f;

    private PlayerAnimationRelay animationRelay;
    private PlayerHealth health;
    private bool inProgress;
    private bool impactFired;
    private float startedAt;

    private void Awake()
    {
        animationRelay = GetComponent<PlayerAnimationRelay>();
        health = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (inProgress && Time.time - startedAt > safetyTimeout)
        {
            Debug.LogWarning($"[Interaction] '{interactionName}' was interrupted before the end event, unlocking.");
            inProgress = false;
        }

        if (Input.GetKeyDown(interactKey)) TryStart();
    }

    private void TryStart()
    {
        if (inProgress) return;
        if (health != null && health.IsDead) return;

        inProgress = true;
        impactFired = false;
        startedAt = Time.time;

        animationRelay.TriggerAction();
        Debug.Log($"[Interaction] START: '{interactionName}'");
    }

    // Animation Event on Player_Action.anim (t = 0.25s).
    public void OnInteractionImpact()
    {
        if (!inProgress || impactFired) return;

        impactFired = true;
        Debug.Log($"[Interaction] EVENT: player performed '{interactionName}' at {Time.time:F2}s");
    }

    // Animation Event on Player_Action.anim (end of the clip).
    public void OnInteractionFinished()
    {
        if (!inProgress) return;

        inProgress = false;
        Debug.Log($"[Interaction] END: '{interactionName}' finished, back to Idle");
    }
}

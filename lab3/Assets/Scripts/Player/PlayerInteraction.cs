using UnityEngine;

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

    // Animation Event (Player_Action, 0.25s)
    public void OnInteractionImpact()
    {
        if (!inProgress || impactFired) return;

        impactFired = true;
        Debug.Log($"[Interaction] EVENT: player performed '{interactionName}' at {Time.time:F2}s");
    }

    // Animation Event (Player_Action, end)
    public void OnInteractionFinished()
    {
        if (!inProgress) return;

        inProgress = false;
        Debug.Log($"[Interaction] END: '{interactionName}' finished, back to Idle");
    }
}

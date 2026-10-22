using UnityEngine;

public class AnimationFeedbackHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;

    [Header("Keybindings for Testing")]
    [SerializeField] private KeyCode hurtKey = KeyCode.H;
    [SerializeField] private KeyCode deathKey = KeyCode.K;
    [SerializeField] private KeyCode actionKey = KeyCode.E;

    private static readonly int HurtTrigger = Animator.StringToHash("Hurt");
    private static readonly int DeathTrigger = Animator.StringToHash("Death");
    private static readonly int ActionTrigger = Animator.StringToHash("Action");

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (animator == null) return;

        if (Input.GetKeyDown(hurtKey))
        {
            animator.SetTrigger(HurtTrigger);
        }

        if (Input.GetKeyDown(deathKey))
        {
            animator.SetTrigger(DeathTrigger);
        }

        if (Input.GetKeyDown(actionKey))
        {
            animator.SetTrigger(ActionTrigger);
        }
    }
    
    public void OnInteractionEvent(string actionName)
    {
        Debug.Log($"[Animation Event] Interaction executed successfully: {actionName} at timestamp {Time.time}");
    }
}
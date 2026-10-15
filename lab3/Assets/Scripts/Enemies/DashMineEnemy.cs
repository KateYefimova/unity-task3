using UnityEngine;

public class DashMineEnemy : BaseEnemy
{
    [Header("Dash Mine")]
    [SerializeField] private float triggerDelay = 0.5f;
    [SerializeField] private float dashSpeed = 14f;
    [SerializeField] private float dashDuration = 0.6f;
    [SerializeField] private float cooldown = 1.5f;
    [SerializeField] private float returnSpeed = 4f;

    private enum State { Idle, Arming, Dashing, Cooldown, Returning }
    private State state = State.Idle;
    private float timer;
    private Vector2 homePosition;

    protected override void Awake()
    {
        base.Awake();
        homePosition = transform.position;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    protected override void UpdateBehaviour()
    {
        switch (state)
        {
            case State.Idle:
                if (PlayerInRange())
                {
                    state = State.Arming;
                    timer = triggerDelay;
                }
                break;

            case State.Arming:
                timer -= Time.deltaTime;
                if (timer <= 0f) StartDash();
                break;

            case State.Dashing:
                timer -= Time.deltaTime;
                if (timer <= 0f) StopDash();
                break;

            case State.Cooldown:
                timer -= Time.deltaTime;
                if (timer <= 0f) state = State.Returning;
                break;

            case State.Returning:
                Vector2 toHome = homePosition - (Vector2)transform.position;
                if (toHome.magnitude < 0.1f)
                {
                    rb.linearVelocity = Vector2.zero;
                    state = State.Idle;
                }
                else
                {
                    rb.linearVelocity = toHome.normalized * returnSpeed;
                }
                break;
        }
    }

    private void StartDash()
    {
        Vector2 dir = ((Vector2)playerTransform.position - rb.position).normalized;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.linearVelocity = dir * dashSpeed;
        state = State.Dashing;
        timer = dashDuration;
    }

    private void StopDash()
    {
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        state = State.Cooldown;
        timer = cooldown;
    }
}
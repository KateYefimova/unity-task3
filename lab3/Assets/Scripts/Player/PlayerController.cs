using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement (N++ Momentum)")]
    [SerializeField] private float maxSpeed = 13f;
    [SerializeField] private float groundAcceleration = 45f;
    [SerializeField] private float groundDeceleration = 35f;
    [SerializeField] private float airAcceleration = 25f;
    [SerializeField] private float airDeceleration = 20f;

    [Header("Jump Arc & Game Feel")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float fallMultiplier = 3.2f;
    [SerializeField] private float lowJumpMultiplier = 2.4f;
    [SerializeField] private float coyoteTime = 0.12f;
    [SerializeField] private float jumpBufferTime = 0.12f;

    [Header("Wall Slide & Wall Jump")]
    [SerializeField] private float wallSlideSpeed = 2.2f;
    [SerializeField] private Vector2 wallJumpForce = new Vector2(11f, 13f);
    [SerializeField] private float wallJumpControlLockTime = 0.15f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 22f;
    [SerializeField] private float dashDuration = 0.18f;
    [SerializeField] private float dashCooldown = 0.8f;

    [Header("Raycast Detection")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundRayLength = 0.35f;
    [SerializeField] private float wallRayLength = 0.25f;

    public Rigidbody2D Rb { get; private set; }
    public Collider2D Col { get; private set; }
    public PlayerStateMachine StateMachine { get; private set; }

    public PlayerGroundedState GroundedState { get; private set; }
    public PlayerAirState AirState { get; private set; }
    public PlayerWallSlideState WallSlideState { get; private set; }
    public PlayerWallJumpState WallJumpState { get; private set; }
    public PlayerDashState DashState { get; private set; }

    public float InputX { get; private set; }
    public float FacingDirection { get; private set; } = 1f;
    public int LastWallDirection { get; private set; } = 0;
    public float CoyoteTimeCounter { get; private set; }
    public float JumpBufferCounter { get; private set; }
    public float InitialGravityScale { get; private set; }
    private float dashCooldownTimer;
    private float currentVelocityX;

    public float GroundAcceleration => groundAcceleration;
    public float GroundDeceleration => groundDeceleration;
    public float AirAcceleration => airAcceleration;
    public float AirDeceleration => airDeceleration;
    public float WallSlideSpeed => wallSlideSpeed;
    public Vector2 WallJumpForce => wallJumpForce;
    public float WallJumpControlLockTime => wallJumpControlLockTime;
    public float DashSpeed => dashSpeed;
    public float DashDuration => dashDuration;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Col = GetComponent<Collider2D>();
        InitialGravityScale = Rb.gravityScale;

        StateMachine = new PlayerStateMachine();
        GroundedState = new PlayerGroundedState(this, StateMachine);
        AirState = new PlayerAirState(this, StateMachine);
        WallSlideState = new PlayerWallSlideState(this, StateMachine);
        WallJumpState = new PlayerWallJumpState(this, StateMachine);
        DashState = new PlayerDashState(this, StateMachine);
    }

    private void Start()
    {
        StateMachine.Initialize(GroundedState);
    }

    private void Update()
    {
        GatherInput();
        UpdateTimers();

        StateMachine.CurrentState.HandleInput();
        StateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }

    private void GatherInput()
    {
        InputX = Input.GetAxisRaw("Horizontal");
        if (InputX != 0) FacingDirection = Mathf.Sign(InputX);

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
        {
            JumpBufferCounter = jumpBufferTime;
        }

        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && dashCooldownTimer <= 0f)
        {
            StateMachine.ChangeState(DashState);
        }
    }

    private void UpdateTimers()
    {
        CoyoteTimeCounter -= Time.deltaTime;
        JumpBufferCounter -= Time.deltaTime;
        dashCooldownTimer -= Time.deltaTime;
    }

    public void ResetCoyoteTimer() => CoyoteTimeCounter = coyoteTime;
    public void ResetDashCooldown() => dashCooldownTimer = dashCooldown;

    public void ExecuteJump()
    {
        Rb.linearVelocity = new Vector2(Rb.linearVelocity.x, jumpForce);
        JumpBufferCounter = 0f;
        CoyoteTimeCounter = 0f;
    }

    public void ApplyHorizontalMovement(float accel, float decel)
    {
        float targetSpeed = InputX * maxSpeed;
        float smoothTime = IsGrounded() ? 0.08f : 0.15f;
        float newX = Mathf.SmoothDamp(Rb.linearVelocity.x, targetSpeed, ref currentVelocityX, smoothTime);
        Rb.linearVelocity = new Vector2(newX, Rb.linearVelocity.y);
    }

    public void ApplyJumpGravityModifiers()
    {
        if (Rb.linearVelocity.y < 0)
        {
            Rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (Rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space) && !Input.GetButton("Jump"))
        {
            Rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    public bool IsGrounded()
    {
        Bounds b = Col.bounds;
        Vector2 leftOrigin = new Vector2(b.min.x + 0.05f, b.min.y);
        Vector2 rightOrigin = new Vector2(b.max.x - 0.05f, b.min.y);

        RaycastHit2D hitLeft = Physics2D.Raycast(leftOrigin, Vector2.down, groundRayLength, groundLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(rightOrigin, Vector2.down, groundRayLength, groundLayer);

        return hitLeft.collider != null || hitRight.collider != null;
    }

    public bool IsTouchingWall(out int wallDirection)
    {
        Bounds b = Col.bounds;
        Vector2 center = b.center;

        RaycastHit2D hitRight = Physics2D.Raycast(center, Vector2.right, b.extents.x + wallRayLength, groundLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(center, Vector2.left, b.extents.x + wallRayLength, groundLayer);

        if (hitRight.collider != null)
        {
            wallDirection = 1;
            LastWallDirection = 1;
            return true;
        }
        if (hitLeft.collider != null)
        {
            wallDirection = -1;
            LastWallDirection = -1;
            return true;
        }

        wallDirection = 0;
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (Col == null) Col = GetComponent<Collider2D>();
        if (Col == null) return;

        Bounds b = Col.bounds;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector2(b.min.x + 0.05f, b.min.y), new Vector2(b.min.x + 0.05f, b.min.y - groundRayLength));
        Gizmos.DrawLine(new Vector2(b.max.x - 0.05f, b.min.y), new Vector2(b.max.x - 0.05f, b.min.y - groundRayLength));

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(b.center, b.center + Vector3.right * (b.extents.x + wallRayLength));
        Gizmos.DrawLine(b.center, b.center + Vector3.left * (b.extents.x + wallRayLength));
    }
}
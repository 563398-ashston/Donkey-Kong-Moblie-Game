
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;
    [SerializeField] private Animator anim;

    [Header("Player Values")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float jumpingPower = 16f;
    [SerializeField] private float maxJumpAnimationTime = 0.5f;
    [SerializeField] private Transform visualModel;

    private float jumpAnimationTimer;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction runAction;
    private float horizontal;
    private bool isRunning;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        runAction = playerInput.actions["Run"];
    }

    private void Update()
    {
        horizontal = moveAction.ReadValue<Vector2>().x;
        isRunning = runAction.IsPressed();

        Move();
        Jump();

        // Character flipping
        if (horizontal > 0)
        {
            visualModel.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (horizontal < 0)
        {
            visualModel.localRotation = Quaternion.Euler(0, 180, 0);
        }
    }

    private void Move()
    {
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        rb.linearVelocity = new Vector2(horizontal * currentSpeed, rb.linearVelocity.y);

        bool isMoving = Mathf.Abs(horizontal) > 0.1f;
        anim.SetBool("walk", isMoving && !isRunning);
        anim.SetBool("run", isMoving && isRunning);
    }

    private void Jump()
    {
        bool grounded = IsGrounded();
        bool holdingJump = jumpAction.IsPressed();

        if (grounded && jumpAction.WasPressedThisFrame())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
            jumpAnimationTimer = 0f;
        }

        if (grounded)
        {
            anim.SetBool("jump", false);
            anim.SetBool("fall", false);
            jumpAnimationTimer = 0f;
        }
        else
        {
            jumpAnimationTimer += Time.deltaTime;
            bool playJump = holdingJump && jumpAnimationTimer < maxJumpAnimationTime;
            anim.SetBool("jump", playJump);
            anim.SetBool("fall", !playJump);
        }

        /*
        if (!holdingJump && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
        */
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.3f, groundLayer) != null;
    }
}

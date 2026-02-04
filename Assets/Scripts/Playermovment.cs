using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Playermovment : MonoBehaviour
{
    // Загальні змінні для руху
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float crouchSpeedMultiplier = 0.5f;
    public float dashSpeed = 10f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private float dashTime;
    private float dashCooldownTimer;

    // Змінні для стрибків
    [Header("Jumping")]
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask whatIsGround;
    private bool isGrounded;

    // Змінні для присідання
    [Header("Crouching")]
    public Transform crouchCeilingCheck;
    public float crouchCeilingCheckRadius = 0.2f;
    public Image crouchIndicator;
    private bool isCrouching = false;
    private CapsuleCollider2D capsuleCollider;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    // Змінні для підкатування
    [Header("Sliding")]
    public float slideSpeed = 7f;
    public float slideDuration = 0.5f;
    private float slideTime;
    private bool isSliding;

    // Змінні для Wall Slide та Wall Jump
    [Header("Wall Interaction")]
    public Transform wallCheck;
    public float wallCheckRadius = 0.2f;
    public LayerMask whatIsWall;
    public float wallSlideSpeed = 2f;
    public float wallJumpForceX = 8f;
    public float wallJumpForceY = 10f;
    public float wallJumpTime = 0.3f;
    private bool isTouchingWall;
    private bool isWallSliding;
    private float wallJumpTimer;

    // Компоненти
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 moveInput;

    // Змінна для визначення напрямку (для повороту персонажа)
    private bool facingRight = true;

    // Візуалізація Gizmos для зручності
    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        if (wallCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadius);
        }
        if (crouchCeilingCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(crouchCeilingCheck.position, crouchCeilingCheckRadius);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        originalColliderSize = capsuleCollider.size;
        originalColliderOffset = capsuleCollider.offset;

        // Перевіряємо, чи існує індикатор, і вимикаємо його на старті
        if (crouchIndicator != null)
        {
            crouchIndicator.enabled = false;
        }
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");

        if (dashTime > 0) dashTime -= Time.deltaTime;
        if (dashCooldownTimer > 0) dashCooldownTimer -= Time.deltaTime;
        if (slideTime > 0) slideTime -= Time.deltaTime;
        if (wallJumpTimer > 0) wallJumpTimer -= Time.deltaTime;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, whatIsWall);

        if (isTouchingWall && !isGrounded && wallJumpTimer <= 0 && !isCrouching)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }

        if (Input.GetButtonDown("Jump") && !isCrouching)
        {
            if (isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
            else if (isWallSliding && !isGrounded)
            {
                wallJumpTimer = wallJumpTime;
                float jumpDirection = facingRight ? 1f : -1f;
                rb.velocity = new Vector2(-jumpDirection * wallJumpForceX, wallJumpForceY);
                Flip();
            }
        }

        // КНОПКА ПОМІНЯНА З LeftShift НА LeftAlt ТУТ
        if (Input.GetKeyDown(KeyCode.LeftAlt) && dashCooldownTimer <= 0 && !isWallSliding && !isCrouching)
        {
            StartDash();
        }

        if (Input.GetKeyDown(KeyCode.S) && isGrounded && !isSliding)
        {
            StartSlide();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCrouch();
        }
    }

    void FixedUpdate()
    {
        if (wallJumpTimer > 0)
        {
            return;
        }

        float currentMoveSpeed = moveSpeed;
        if (isCrouching)
        {
            currentMoveSpeed *= crouchSpeedMultiplier;
        }

        if (dashTime > 0)
        {
            float dashDirection = facingRight ? 1f : -1f;
            rb.velocity = new Vector2(dashDirection * dashSpeed, rb.velocity.y);
        }
        else if (isSliding)
        {
            float slideDirection = facingRight ? 1f : -1f;
            rb.velocity = new Vector2(slideDirection * slideSpeed, rb.velocity.y);
        }
        else if (isWallSliding)
        {
            rb.velocity = new Vector2(0, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
        }
        else
        {
            rb.velocity = new Vector2(moveInput.x * currentMoveSpeed, rb.velocity.y);
        }

        if (moveInput.x > 0 && !facingRight && !isSliding && dashTime <= 0)
        {
            Flip();
        }
        else if (moveInput.x < 0 && facingRight && !isSliding && dashTime <= 0)
        {
            Flip();
        }
    }

    void StartDash()
    {
        dashTime = dashDuration;
        dashCooldownTimer = dashCooldown;
    }

    void StartSlide()
    {
        isSliding = true;
        slideTime = slideDuration;
        Invoke("StopSlide", slideDuration);
    }

    void StopSlide()
    {
        isSliding = false;
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    void ToggleCrouch()
    {
        if (isCrouching)
        {
            if (!Physics2D.OverlapCircle(crouchCeilingCheck.position, crouchCeilingCheckRadius, whatIsGround))
            {
                isCrouching = false;
                capsuleCollider.size = originalColliderSize;
                capsuleCollider.offset = originalColliderOffset;
                if (crouchIndicator != null) crouchIndicator.enabled = false;
            }
        }
        else
        {
            isCrouching = true;
            capsuleCollider.size = new Vector2(originalColliderSize.x, originalColliderSize.y / 2f);
            capsuleCollider.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - (originalColliderSize.y / 4f));
            if (crouchIndicator != null) crouchIndicator.enabled = true;
        }
    }
}
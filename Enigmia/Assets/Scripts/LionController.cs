using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class LionController : MonoBehaviour
{
    [Header("Movimento")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;

    [Header("Chão")]
    public LayerMask groundLayer;          // marque a layer "Ground" no chão
    public float groundCheckDistance = 0.1f;

    Rigidbody2D rb;
    CapsuleCollider2D col;
    Animator anim;
    SpriteRenderer sr;

    float moveInput;
    bool jumpPressed;
    bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        ReadInput();

        // Vira o sprite conforme a direção (se o sprite original olha para a direita)
        if (moveInput > 0.01f) sr.flipX = false;
        else if (moveInput < -0.01f) sr.flipX = true;

        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(moveInput));
            anim.SetBool("IsGrounded", isGrounded);
            anim.SetFloat("YVelocity", rb.linearVelocity.y);
        }
    }

    void FixedUpdate()
    {
        CheckGround();

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (jumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            if (anim != null) anim.SetTrigger("Jump");
        }
        jumpPressed = false;
    }

    void ReadInput()
    {
#if ENABLE_INPUT_SYSTEM
        var kb = Keyboard.current;
        if (kb == null) return;

        moveInput = 0f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) moveInput -= 1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) moveInput += 1f;
        if (kb.spaceKey.wasPressedThisFrame || kb.wKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame)
            jumpPressed = true;
#else
        moveInput = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump")) jumpPressed = true;
#endif
    }

    void CheckGround()
    {
        RaycastHit2D hit = Physics2D.CapsuleCast(
            col.bounds.center, col.bounds.size, col.direction, 0f,
            Vector2.down, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null;
    }
}

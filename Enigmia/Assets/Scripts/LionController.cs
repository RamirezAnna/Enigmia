using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class LionController : MonoBehaviour
{
    [Header("Movimento")]
    public float moveSpeed = 3f;     // velocidade andando
    public float runSpeed = 6f;      // velocidade correndo (segurando Shift)
    public float jumpForce = 8f;

    [Header("Chão")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;

    Rigidbody2D rb;
    CapsuleCollider2D col;
    Animator anim;
    SpriteRenderer sr;

    // Guarda os parâmetros que existem no Animator, para não dar erro
    // se algum ainda não tiver sido criado.
    readonly HashSet<int> animParams = new HashSet<int>();

    float moveInput;
    bool runHeld;
    bool jumpPressed;
    bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true;

        if (anim != null)
            foreach (var p in anim.parameters)
                animParams.Add(p.nameHash);
    }

    void Update()
    {
        ReadInput();

        bool moving = Mathf.Abs(moveInput) > 0.01f;
        bool isRunning = moving && runHeld;

        // Vira o sprite conforme a direção
        if (moveInput > 0.01f) sr.flipX = false;
        else if (moveInput < -0.01f) sr.flipX = true;

        if (anim != null)
        {
            SetFloat("Speed", Mathf.Abs(moveInput));
            SetBool("IsRunning", isRunning);
            SetBool("IsGrounded", isGrounded);
            SetFloat("YVelocity", rb.linearVelocity.y);
        }
    }

    void FixedUpdate()
    {
        CheckGround();

        float speed = runHeld ? runSpeed : moveSpeed;
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        if (jumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            SetTrigger("Jump");
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

        runHeld = kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed;

        if (kb.spaceKey.wasPressedThisFrame || kb.wKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame)
            jumpPressed = true;
#else
        moveInput = Input.GetAxisRaw("Horizontal");
        runHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
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

    // --- Atalhos seguros para o Animator ---
    void SetFloat(string name, float v)
    {
        if (animParams.Contains(Animator.StringToHash(name))) anim.SetFloat(name, v);
    }
    void SetBool(string name, bool v)
    {
        if (animParams.Contains(Animator.StringToHash(name))) anim.SetBool(name, v);
    }
    void SetTrigger(string name)
    {
        if (animParams.Contains(Animator.StringToHash(name))) anim.SetTrigger(name);
    }
}

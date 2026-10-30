using UnityEngine;

public class Andar : MonoBehaviour
{
    public float velocidadeAndar = 5f;
    public float velocidadeCorrer = 9f;
    public float forcaPulo = 12f;
    public Transform verificadorChao;
    public float raioChao = 0.2f;
    public LayerMask camadaChao;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float horizontal;
    private bool noChao;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        noChao = Physics2D.OverlapCircle(verificadorChao.position, raioChao, camadaChao);

        if (Input.GetKeyDown(KeyCode.Space) && noChao)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, forcaPulo);
        }

        if (horizontal > 0) sr.flipX = false;
        else if (horizontal < 0) sr.flipX = true;
    }

    void FixedUpdate()
    {
        float velocidade = Input.GetKey(KeyCode.LeftShift) ? velocidadeCorrer : velocidadeAndar;
        rb.linearVelocity = new Vector2(horizontal * velocidade, rb.linearVelocity.y);
    }
}
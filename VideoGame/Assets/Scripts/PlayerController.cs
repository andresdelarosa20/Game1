using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 7f;

    [Header("Salto")]
    public float jumpForce = 10f;         // Impulso inicial (salto corto)
    public float jumpHoldForce = 25f;     // Fuerza extra si mantienes presionado
    public float jumpHoldDuration = 0.2f; // Tiempo máximo que se puede mantener
    public float fallMultiplier = 3f;     // Caída más rápida
    public float lowJumpMultiplier = 5f;  // Caída rápida si sueltas el salto pronto

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Muerte")]
    public GameObject deathScreen; // Arrastra aquí tu panel de muerte en el Inspector

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isJumping;
    private float jumpHoldTimer;
    private bool jumpButtonHeld;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (deathScreen != null)
            deathScreen.SetActive(false);
    }

    void Update()
    {
        CheckGrounded();
        HandleMovement();
        HandleJumpInput();
        ApplyBetterGravity();
    }

    // ─────────────────────────────────────────
    // MOVIMIENTO HORIZONTAL
    // ─────────────────────────────────────────
    void HandleMovement()
    {
        float input = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(input * moveSpeed, rb.velocity.y);

        // Voltear sprite según dirección
        if (input > 0.01f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (input < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    // ─────────────────────────────────────────
    // SALTO VARIABLE (mantener = saltar más alto)
    // ─────────────────────────────────────────
    void HandleJumpInput()
    {
        // Presiona salto estando en el suelo → impulso inicial
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
            jumpHoldTimer = 0f;
        }

        // Mantener presionado → aplicar fuerza extra hacia arriba
        if (Input.GetButton("Jump") && isJumping)
        {
            if (jumpHoldTimer < jumpHoldDuration)
            {
                rb.AddForce(Vector2.up * jumpHoldForce * Time.deltaTime, ForceMode2D.Force);
                jumpHoldTimer += Time.deltaTime;
            }
        }

        // Soltó el botón → deja de aplicar fuerza extra
        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }
    }

    // ─────────────────────────────────────────
    // GRAVEDAD MEJORADA (caída más natural)
    // ─────────────────────────────────────────
    void ApplyBetterGravity()
    {
        if (rb.velocity.y < 0)
        {
            // Cae rápido
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            // Si soltaste el botón mientras subías, cae más rápido también
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    // ─────────────────────────────────────────
    // DETECCIÓN DE SUELO
    // ─────────────────────────────────────────
    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
            isJumping = false;
    }

    // ─────────────────────────────────────────
    // COLISIONES CON ENEMIGOS
    // ─────────────────────────────────────────
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Revisar si el jugador viene desde arriba (pisa al enemigo)
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f) // Normal apunta hacia arriba → jugador encima
                {
                    // El jugador pisó al enemigo → pequeño rebote
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce * 0.7f);
                    return; // No muere, el enemigo es quien maneja su propia muerte
                }
            }

            // Lo tocó por los lados o desde abajo → jugador muere
            Die();
        }
    }

    // ─────────────────────────────────────────
    // MUERTE DEL JUGADOR
    // ─────────────────────────────────────────
    public void Die()
    {
        this.enabled = false;
        StartCoroutine(DeathDelay());
    }

    System.Collections.IEnumerator DeathDelay()
    {
        // Congela TODO el juego (enemigos, físicas, animaciones, etc.)
        Time.timeScale = 0f;

        // WaitForSecondsRealtime ignora el timeScale, así el timer sigue corriendo
        yield return new WaitForSecondsRealtime(3f);

        // Mostrar pantalla de muerte
        if (deathScreen != null)
            deathScreen.SetActive(true);
    }

    // ─────────────────────────────────────────
    // DEBUG (visualizar el groundCheck en Scene)
    // ─────────────────────────────────────────
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
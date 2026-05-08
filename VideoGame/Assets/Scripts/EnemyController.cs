using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 2f;
    public bool patrolling = true;         // ¿Patrulla de un lado al otro?

    [Header("Detección de borde / pared")]
    public Transform wallCheck;
    public Transform edgeCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Rebote al ser pisado")]
    public float stompBounce = 8f; // Impulso que le da al jugador al pisarlo

    // ── Estado interno ──
    private Rigidbody2D rb;
    private bool movingRight = true;
    private Vector3 startPosition;        // Posición inicial para el retry

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
    }

    void Update()
    {
        if (patrolling)
            Patrol();
    }

    // ─────────────────────────────────────────
    // PATRULLAJE AUTOMÁTICO
    // ─────────────────────────────────────────
    void Patrol()
    {
        float direction = movingRight ? 1f : -1f;
        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

        // Voltear sprite
        transform.localScale = new Vector3(movingRight ? 1 : -1, 1, 1);

        bool hitWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, groundLayer);
        bool atEdge  = !Physics2D.OverlapCircle(edgeCheck.position, checkRadius, groundLayer);

        if (hitWall || atEdge)
            Flip();
    }

    void Flip()
    {
        movingRight = !movingRight;
    }

    // ─────────────────────────────────────────
    // COLISIÓN CON EL JUGADOR
    // ─────────────────────────────────────────
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Normal apunta hacia arriba → el jugador viene desde arriba (lo pisa)
            if (contact.normal.y < -0.5f)
            {
                // Dar impulso de rebote al jugador
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                    playerRb.velocity = new Vector2(playerRb.velocity.x, stompBounce);

                // Matar al enemigo
                Die();
                return;
            }
        }

        // Contacto lateral o desde abajo → matar al jugador
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
            player.Die();
    }

    // ─────────────────────────────────────────
    // MUERTE DEL ENEMIGO
    // ─────────────────────────────────────────
    void Die()
    {
        // Podés agregar una animación aquí antes de desactivar
        gameObject.SetActive(false);
    }

    // ─────────────────────────────────────────
    // RESET (llamado por el GameManager al Retry)
    // ─────────────────────────────────────────
    public void ResetEnemy()
    {
        transform.position = startPosition;
        movingRight = true;
        rb.velocity = Vector2.zero;
        gameObject.SetActive(true);
    }

    // ─────────────────────────────────────────
    // DEBUG
    // ─────────────────────────────────────────
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (wallCheck != null) Gizmos.DrawWireSphere(wallCheck.position, checkRadius);

        Gizmos.color = Color.yellow;
        if (edgeCheck != null) Gizmos.DrawWireSphere(edgeCheck.position, checkRadius);
    }
}

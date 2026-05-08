using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    [Header("Puntos de patrullaje")]
    public Transform pointA;   // Arrastra un GameObject vacío como punto de inicio
    public Transform pointB;   // Arrastra otro GameObject vacío como punto final

    [Header("Movimiento")]
    public float moveSpeed = 3f;
    public float waitTime = 0.5f; // Tiempo que espera al llegar a cada punto

    [Header("Rebote al ser pisado")]
    public float stompBounce = 8f;

    private Vector3 startPosition;
    private bool goingToB = true;
    private float waitTimer = 0f;
    private bool waiting = false;
    private Rigidbody2D rb;

    void Start()
    {
        startPosition = transform.position;

        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // El enemigo volador no debe caer por gravedad
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }
    }

    void Update()
    {
        Patrol();
        FlipSprite();
    }

    // ─────────────────────────────────────────
    // PATRULLAJE ENTRE PUNTO A Y PUNTO B
    // ─────────────────────────────────────────
    void Patrol()
    {
        if (pointA == null || pointB == null) return;

        if (waiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
                waiting = false;
            return;
        }

        Transform target = goingToB ? pointB : pointA;
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        // Llegó al destino
        if (Vector3.Distance(transform.position, target.position) < 0.05f)
        {
            goingToB = !goingToB;
            waiting = true;
            waitTimer = waitTime;
        }
    }

    // ─────────────────────────────────────────
    // VOLTEAR SPRITE SEGÚN DIRECCIÓN HORIZONTAL
    // ─────────────────────────────────────────
    void FlipSprite()
    {
        if (pointA == null || pointB == null) return;

        Transform target = goingToB ? pointB : pointA;
        float dirX = target.position.x - transform.position.x;

        if (dirX > 0.01f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (dirX < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    // ─────────────────────────────────────────
    // COLISIÓN CON EL JUGADOR
    // ─────────────────────────────────────────
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Jugador viene desde arriba → lo pisa
            if (contact.normal.y < -0.5f)
            {
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                    playerRb.velocity = new Vector2(playerRb.velocity.x, stompBounce);

                Die();
                return;
            }
        }

        // Contacto lateral o desde abajo → mata al jugador
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
            player.Die();
    }

    // ─────────────────────────────────────────
    // MUERTE DEL ENEMIGO VOLADOR
    // ─────────────────────────────────────────
    void Die()
    {
        gameObject.SetActive(false);
    }

    // ─────────────────────────────────────────
    // RESET (llamado por el GameManager al Retry)
    // ─────────────────────────────────────────
    public void ResetEnemy()
    {
        transform.position = startPosition;
        goingToB = true;
        waiting = false;
        waitTimer = 0f;

        if (rb != null)
            rb.velocity = Vector2.zero;

        gameObject.SetActive(true);
    }

    // ─────────────────────────────────────────
    // DEBUG — muestra la línea entre los puntos
    // ─────────────────────────────────────────
    void OnDrawGizmosSelected()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawWireSphere(pointA.position, 0.2f);
            Gizmos.DrawWireSphere(pointB.position, 0.2f);
        }
    }
}

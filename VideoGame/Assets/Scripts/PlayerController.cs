using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 7f;

    [Header("Salto")]
    public float jumpForce = 10f;
    public float jumpHoldForce = 25f;
    public float jumpHoldDuration = 0.2f;
    public float fallMultiplier = 3f;
    public float lowJumpMultiplier = 5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Muerte")]
    public GameObject deathScreen;

    private Rigidbody2D rb;
    private Animator anim;

    private bool isGrounded;
    private bool isJumping;
    private float jumpHoldTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (deathScreen != null)
            deathScreen.SetActive(false);
    }

    void Update()
    {
        CheckGrounded();
        HandleMovement();
        HandleJumpInput();
        ApplyBetterGravity();
        UpdateAnimations();
    }

    void HandleMovement()
    {
        float input = Input.GetAxis("Horizontal");

        rb.velocity = new Vector2(input * moveSpeed, rb.velocity.y);

        if (input > 0.01f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (input < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
            jumpHoldTimer = 0f;
        }

        if (Input.GetButton("Jump") && isJumping)
        {
            if (jumpHoldTimer < jumpHoldDuration)
            {
                rb.AddForce(Vector2.up * jumpHoldForce * Time.deltaTime, ForceMode2D.Force);
                jumpHoldTimer += Time.deltaTime;
            }
        }

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }
    }

    void ApplyBetterGravity()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        if (isGrounded)
            isJumping = false;
    }

    void UpdateAnimations()
    {
        float speed = Mathf.Abs(rb.velocity.x);

        anim.SetFloat("Speed", speed);
        anim.SetBool("IsGrounded", isGrounded);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce * 0.7f);
                    return;
                }
            }

            Die();
        }
    }

    public void Die()
    {
        this.enabled = false;
        StartCoroutine(DeathDelay());
    }

    System.Collections.IEnumerator DeathDelay()
    {
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(3f);

        if (deathScreen != null)
            deathScreen.SetActive(true);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
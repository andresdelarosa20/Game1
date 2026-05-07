using UnityEngine;

/// Si el jugador es el 1 WASD, si es el 2 arrow keys.
public class PlayerController : MonoBehaviour
{
    [Header("Identificación del Jugador")]
    [Tooltip("1 = Jugador 1 (WASD) | 2 = Jugador 2 (Flechas)")]
    public int playerNumber = 1;

    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Detección de suelo")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Visual (Sprite + Animator)")]
    [Tooltip("Arrastra aquí el hijo que contiene el SpriteRenderer y Animator")]
    [SerializeField] private Transform visual;

    private Rigidbody2D rb;
    private Animator animator;

    private bool isGrounded;
    private GameObject activePlatform;

    private KeyCode keyLeft;
    private KeyCode keyRight;
    private KeyCode keyJump;
    private KeyCode keyPlatform;

    // Animator 
    private int speedHash;
    private int groundedHash;
    private int yVelocityHash;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();

        AssignKeys();

        // Inicializar hashes
        speedHash = Animator.StringToHash("Speed");
        groundedHash = Animator.StringToHash("isGrounded");
        yVelocityHash = Animator.StringToHash("yVelocity");
    }

    void Update()
    {
        CheckGrounded();
        HandleMovement();
        HandleJump();
        UpdateAnimations();
    }

    void AssignKeys()
    {
        if (playerNumber == 1)
        {
            keyLeft = KeyCode.A;
            keyRight = KeyCode.D;
            keyJump = KeyCode.W;
            keyPlatform = KeyCode.S;
        }
        else
        {
            keyLeft = KeyCode.LeftArrow;
            keyRight = KeyCode.RightArrow;
            keyJump = KeyCode.UpArrow;
            keyPlatform = KeyCode.DownArrow;
        }
    }

    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void HandleMovement()
    {
        float horizontal = 0f;

        if (Input.GetKey(keyLeft)) horizontal = -1f;
        if (Input.GetKey(keyRight)) horizontal = 1f;

        rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);

        // Voltear SOLO el visual (sin romper escala)
        if (horizontal != 0 && visual != null)
        {
            Vector3 scale = visual.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(horizontal);
            visual.localScale = scale;
        }
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(keyJump) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void UpdateAnimations()
    {
        float speed = Mathf.Abs(rb.velocity.x);

        animator.SetFloat(speedHash, speed);
        animator.SetBool(groundedHash, isGrounded);
        animator.SetFloat(yVelocityHash, rb.velocity.y);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
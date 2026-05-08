using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Referencias")]
    public PlayerController player;
    public GameObject deathScreenUI;

    [Header("Enemigos")]
    public EnemyController[] enemies;         // Enemigos caminadores
    public FlyingEnemy[] flyingEnemies;       // Enemigos voladores

    private Vector3 playerStartPosition;
    private Quaternion playerStartRotation;

    void Awake()
    {
        // Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Guardar posición inicial del jugador
        playerStartPosition = player.transform.position;
        playerStartRotation = player.transform.rotation;

        if (deathScreenUI != null)
            deathScreenUI.SetActive(false);
    }

    // ─────────────────────────────────────────
    // LLAMADO DESDE EL BOTÓN "RETRY" EN LA UI
    // ─────────────────────────────────────────
    public void Retry()
    {
        // 1. Restaurar el tiempo del juego
        Time.timeScale = 1f;

        // 2. Ocultar pantalla de muerte
        if (deathScreenUI != null)
            deathScreenUI.SetActive(false);

        // 3. Resetear posición y estado del jugador
        player.transform.position = playerStartPosition;
        player.transform.rotation = playerStartRotation;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 1f;
        }

        player.enabled = true;

        // 4. Resetear enemigos caminadores
        foreach (EnemyController enemy in enemies)
        {
            if (enemy != null)
                enemy.ResetEnemy();
        }

        // 5. Resetear enemigos voladores
        foreach (FlyingEnemy flying in flyingEnemies)
        {
            if (flying != null)
                flying.ResetEnemy();
        }
    }

    // ─────────────────────────────────────────
    // ALTERNATIVA: recargar toda la escena
    // (más simple, descomenta si preferís esto)
    // ─────────────────────────────────────────
    // public void Retry()
    // {
    //     SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    // }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalStar : MonoBehaviour
{

    public string creditsSceneName = "Credits";


    public float delayBeforeLoad = 1.5f;
    public AudioSource collectSound;


    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activated) return;

        if (collision.CompareTag("Player"))
        {
            activated = true;

            // Desactivar estrella visual/collider
            GetComponent<Collider2D>().enabled = false;

            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.enabled = false;

            // Sonido opcional
            if (collectSound != null)
                collectSound.Play();


            // Desactivar movimiento del player
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
                player.enabled = false;

            Invoke(nameof(LoadCredits), delayBeforeLoad);
        }
    }

    void LoadCredits()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(creditsSceneName);
    }
}
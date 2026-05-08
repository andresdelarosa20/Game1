using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float parallaxEffect = 0.5f;

    private float startPosX;

    void Start()
    {
        startPosX = transform.position.x;

        // Si no asignas cámara, usa la Main Camera
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        float distance = cameraTransform.position.x * parallaxEffect;

        transform.position = new Vector3(
            startPosX + distance,
            transform.position.y,
            transform.position.z
        );
    }
}

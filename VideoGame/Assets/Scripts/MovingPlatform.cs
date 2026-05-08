using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private Vector3 direction = Vector3.right;
    [SerializeField] private float distance = 5f;
    [SerializeField] private float speed = 2f;

    private Vector3 startPoint;
    private Vector3 endPoint;

    void Start()
    {
        startPoint = transform.position;
        endPoint = startPoint + direction.normalized * distance;
    }

    void Update()
    {
        // Movimiento de ida y vuelta
        float pingPong = Mathf.PingPong(Time.time * speed, 1f);

        transform.position = Vector3.Lerp(startPoint, endPoint, pingPong);
    }

    // Dibuja la ruta en el editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Vector3 start = Application.isPlaying ? startPoint : transform.position;
        Vector3 end = start + direction.normalized * distance;

        Gizmos.DrawLine(start, end);

        Gizmos.DrawSphere(start, 0.2f);
        Gizmos.DrawSphere(end, 0.2f);
    }
}
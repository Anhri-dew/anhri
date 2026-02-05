using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float waitTime = 1f;
    public Transform[] points;

    private int currentPoint = 0;
    private float waitCounter = 0f;
    private bool isWaiting = false;

    void Start()
    {
        if (points.Length == 0)
        {
            // Создаем 2 точки по умолчанию
            points = new Transform[2];

            GameObject point1 = new GameObject("Point1");
            point1.transform.position = transform.position + Vector3.left * 5f;
            point1.transform.parent = transform.parent;
            points[0] = point1.transform;

            GameObject point2 = new GameObject("Point2");
            point2.transform.position = transform.position + Vector3.right * 5f;
            point2.transform.parent = transform.parent;
            points[1] = point2.transform;
        }
    }

    void FixedUpdate()
    {
        if (points.Length == 0) return;

        if (isWaiting)
        {
            waitCounter += Time.deltaTime;
            if (waitCounter >= waitTime)
            {
                isWaiting = false;
                currentPoint = (currentPoint + 1) % points.Length;
            }
            return;
        }

        // Движение к точке
        Vector3 targetPos = points[currentPoint].position;
        Vector3 moveDirection = (targetPos - transform.position).normalized;

        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Проверка достижения точки
        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            isWaiting = true;
            waitCounter = 0f;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Делаем игрока дочерним при попадании на платформу
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Убираем родителя при сходе с платформы
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }

    void OnDrawGizmos()
    {
        if (points != null && points.Length > 0)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i] != null)
                {
                    Gizmos.DrawCube(points[i].position, Vector3.one * 0.5f);
                    if (i < points.Length - 1 && points[i + 1] != null)
                    {
                        Gizmos.DrawLine(points[i].position, points[i + 1].position);
                    }
                }
            }
        }
    }
}
using UnityEngine;

public class MovingSaw : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 500f;
    public MovementType movementType = MovementType.BackAndForth;

    [Header("Waypoints (for Patrol)")]
    public Transform[] waypoints;
    public float waypointThreshold = 0.5f;

    [Header("Damage Settings")]
    public int damagePerHit = 20;
    public float damageCooldown = 1f; // Задержка между уроном

    private Vector3 startPosition;
    private int currentWaypoint = 0;
    private float lastDamageTime = 0f;
    private Rigidbody rb;

    public enum MovementType
    {
        BackAndForth,   // Туда-обратно по одной линии
        Circular,       // По кругу
        Patrol,         // По точкам
        RotateOnly      // Только вращение на месте
    }

    void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody>();

        if (movementType == MovementType.Patrol && waypoints.Length == 0)
        {
            Debug.LogWarning("No waypoints assigned for patrol movement!");
        }
    }

    void Update()
    {
        // Вращение пилы
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // Движение в зависимости от типа
        switch (movementType)
        {
            case MovementType.BackAndForth:
                MoveBackAndForth();
                break;

            case MovementType.Circular:
                MoveCircular();
                break;

            case MovementType.Patrol:
                MovePatrol();
                break;

            case MovementType.RotateOnly:
                // Только вращение, без движения
                break;
        }
    }

    void MoveBackAndForth()
    {
        // Простое движение вперед-назад по оси X
        float newX = startPosition.x + Mathf.PingPong(Time.time * moveSpeed, 6f) - 3f;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    void MoveCircular()
    {
        // Движение по кругу
        float angle = Time.time * moveSpeed;
        float x = Mathf.Cos(angle) * 3f;
        float z = Mathf.Sin(angle) * 3f;

        transform.position = new Vector3(
            startPosition.x + x,
            transform.position.y,
            startPosition.z + z
        );
    }

    void MovePatrol()
    {
        if (waypoints.Length == 0) return;

        // Движение к текущей точке
        Transform target = waypoints[currentWaypoint];
        Vector3 direction = (target.position - transform.position).normalized;

        rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);

        // Проверка достижения точки
        if (Vector3.Distance(transform.position, target.position) < waypointThreshold)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyDamage(other.gameObject);
        }
    }

    void OnTriggerStay(Collider other)
    {
        // Непрерывный урон при нахождении внутри
        if (other.CompareTag("Player") && Time.time - lastDamageTime >= damageCooldown)
        {
            ApplyDamage(other.gameObject);
        }
    }

    void ApplyDamage(GameObject player)
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damagePerHit);
            lastDamageTime = Time.time;

            Debug.Log($"Пила нанесла урон: {damagePerHit}. Здоровье игрока: {health.currentHealth}");

            // Визуальная обратная связь
            StartCoroutine(FlashPlayer(player));
        }
    }

    System.Collections.IEnumerator FlashPlayer(GameObject player)
    {
        Renderer playerRenderer = player.GetComponentInChildren<Renderer>();
        if (playerRenderer != null)
        {
            Material originalMat = playerRenderer.material;
            playerRenderer.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            playerRenderer.material = originalMat;
        }
    }

    void OnDrawGizmos()
    {
        // Визуализация пути для патрулирования
        if (movementType == MovementType.Patrol && waypoints.Length > 0)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] != null)
                {
                    Gizmos.DrawSphere(waypoints[i].position, 0.3f);
                    if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
                    {
                        Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                    }
                }
            }
        }
    }
}

using UnityEngine;
using System.Collections;

public class BarrelWithChildEffect : MonoBehaviour
{
    [Header("Settings")]
    public float explosionRadius = 3f;
    public float explosionForce = 800f;
    public int damageToPlayer = 50;
    public float explosionDelay = 0.3f;

    [Header("Child References")]
    public GameObject barrelVisual;    // Дочерний объект с мешем
    public ParticleSystem explosionEffect; // Дочерний Particle System

    private bool isActivated = false;
    private bool damageApplied = false; // ФЛАГ: урон уже нанесен
    private Renderer visualRenderer;
    private Color originalColor;
    private Collider triggerCollider;
    private Collider physicsCollider;
    private Rigidbody rb;

    void Start()
    {
        // Находим дочерние компоненты если не назначены
        if (barrelVisual == null)
        {
            barrelVisual = transform.Find("BarrelVisual")?.gameObject;
            if (barrelVisual == null)
            {
                barrelVisual = GetComponentInChildren<MeshRenderer>()?.gameObject;
            }
        }

        if (explosionEffect == null)
        {
            explosionEffect = GetComponentInChildren<ParticleSystem>();
        }

        // Получаем компоненты
        if (barrelVisual != null)
        {
            visualRenderer = barrelVisual.GetComponent<Renderer>();
            if (visualRenderer != null)
            {
                originalColor = visualRenderer.material.color;
            }
        }

        // Получаем коллайдеры
        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider col in colliders)
        {
            if (col.isTrigger) triggerCollider = col;
            else physicsCollider = col;
        }

        rb = GetComponent<Rigidbody>();

        // Отключаем эффект взрыва на старте
        if (explosionEffect != null)
        {
            explosionEffect.Stop();
            explosionEffect.gameObject.SetActive(false);
        }

        Debug.Log("Бочка готова. Триггер радиус: " +
            (triggerCollider as SphereCollider)?.radius);
    }

    void OnTriggerEnter(Collider other)
    {
        if (isActivated) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("Игрок вошел в зону бочки");
            isActivated = true; // Сразу ставим флаг активации
            StartCoroutine(ExplodeSequence());
        }
    }

    IEnumerator ExplodeSequence()
    {
        // 1. Визуальная индикация (мигание)
        yield return StartCoroutine(FlashBeforeExplosion());

        // 2. Взрыв
        Explode();
    }

    IEnumerator FlashBeforeExplosion()
    {
        if (visualRenderer == null) yield break;

        float flashInterval = 0.1f;

        for (int i = 0; i < 3; i++)
        {
            visualRenderer.material.color = Color.red;
            yield return new WaitForSeconds(flashInterval);

            visualRenderer.material.color = originalColor;
            yield return new WaitForSeconds(flashInterval);
        }
    }

    void Explode()
    {
        Debug.Log("?? Бочка взрывается!");

        // 1. Активируем и запускаем эффект взрыва
        if (explosionEffect != null)
        {
            explosionEffect.gameObject.SetActive(true);
            explosionEffect.Play();

            // Автоуничтожение эффекта после проигрывания
            Destroy(explosionEffect.gameObject, explosionEffect.main.duration + 1f);
        }

        // 2. Скрываем визуальную часть бочки
        if (barrelVisual != null)
        {
            barrelVisual.SetActive(false);
        }

        // 3. Отключаем коллайдеры
        if (triggerCollider != null) triggerCollider.enabled = false;
        if (physicsCollider != null) physicsCollider.enabled = false;

        // 4. Отключаем физику
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
        }

        // 5. Взрывная волна (один раз!)
        ApplyExplosionForce();

        // 6. Уничтожаем родительский объект через время
        Destroy(gameObject, 3f); // Даем время на проигрывание эффекта
    }

    void ApplyExplosionForce()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in hits)
        {
            if (hit.transform == transform) continue;

            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 2f);
            }

            // Урон наносим ТОЛЬКО ЗДЕСЬ и ТОЛЬКО ОДИН РАЗ
            if (hit.CompareTag("Player") && !damageApplied)
            {
                PlayerHealth health = hit.GetComponent<PlayerHealth>();
                if (health != null && !health.IsDead())
                {
                    health.TakeDamage(damageToPlayer);
                    damageApplied = true; // Ставим флаг, что урон нанесен
                    Debug.Log($"Урон игроку от взрыва: {damageToPlayer}. Здоровье: {health.currentHealth}");
                }
            }
        }

        // Если урон не был нанесен (игрок вышел из радиуса), все равно ставим флаг
        damageApplied = true;
    }

    void OnDrawGizmosSelected()
    {
        // Радиус триггера
        Gizmos.color = Color.yellow;
        SphereCollider trigger = GetComponent<SphereCollider>();
        if (trigger != null && trigger.isTrigger)
        {
            Gizmos.DrawWireSphere(transform.position + trigger.center, trigger.radius);
        }

        // Радиус взрыва
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    // Метод для принудительного взрыва (тестирование)
    public void TestExplode()
    {
        if (!isActivated)
        {
            isActivated = true;
            StartCoroutine(ExplodeSequence());
        }
    }
}

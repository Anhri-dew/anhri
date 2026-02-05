using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI References")]
    public Slider healthSlider;
    public TextMeshProUGUI healthText;
    public GameObject gameOverPanel;

    [Header("Visual Feedback")]
    public Material damageMaterial;
    public float flashDuration = 0.1f;

    private Material originalMaterial;
    private Renderer playerRenderer;
    private bool isDead = false;
    private PlayerVFX playerVFX;

    void Start()
    {
        currentHealth = maxHealth;
        playerRenderer = GetComponentInChildren<Renderer>();

        if (playerRenderer != null)
        {
            originalMaterial = playerRenderer.material;
        }

        UpdateHealthUI();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        playerVFX = GetComponent<PlayerVFX>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        // Эффект при получении урона
        if (playerVFX != null)
        {
            playerVFX.PlayDamageEffect();
        }

        Debug.Log($"Player took {damage} damage. Health: {currentHealth}");

        // Визуальная обратная связь
        StartCoroutine(FlashDamage());

        // Обновляем UI
        UpdateHealthUI();

        // Проверяем смерть
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    System.Collections.IEnumerator FlashDamage()
    {
        if (playerRenderer != null && damageMaterial != null)
        {
            playerRenderer.material = damageMaterial;
            yield return new WaitForSeconds(flashDuration);
            playerRenderer.material = originalMaterial;
        }
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"HP: {currentHealth}/{maxHealth}";
        }
    }

    void Die()
    {
        isDead = true;

        // Эффект смерти
        if (playerVFX != null)
        {
            playerVFX.PlayDeathEffect();
        }

        Debug.Log("Player died!");

        // Отключаем управление
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.enabled = false;
        }

        // Включаем физику "трупа"
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.AddExplosionForce(500f, transform.position + Vector3.down, 5f);
        }

        // Показываем экран Game Over
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Опционально: уничтожаем через несколько секунд
        // Destroy(gameObject, 3f);
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        UpdateHealthUI();
    }

    public bool IsDead()
    {
        return isDead;
    }
}
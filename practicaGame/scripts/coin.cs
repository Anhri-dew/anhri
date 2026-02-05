using UnityEngine;

public class coin : MonoBehaviour
{
    public int coinValue = 1;
    private Animator animator;

    void Start()
    {
        // Ищем Animator в дочернем объекте CoinVisual
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("Animator not found in children!");
        }
    }

    // Этот метод вызывается из CoinCollector (при касании маленького триггера)
    public void Collect(Collider player)
    {
        wallet playerWallet = player.GetComponent<wallet>();
        if (playerWallet != null)
        {
            playerWallet.AddCoins(coinValue);
        }
        Destroy(gameObject);
    }

    // Для анимации (большой триггер)
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Это большой SphereCollider для анимации
            if (animator != null)
            {
                animator.SetBool("PlayerNear", true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (animator != null)
            {
                animator.SetBool("PlayerNear", false);
            }
        }
    }
}

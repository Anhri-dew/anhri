using TMPro;
using UnityEngine;

public class wallet : MonoBehaviour
{
    public static wallet Instance; // Синглтон для легкого доступа из других скриптов

    public int currentCoins = 0;
    public TextMeshProUGUI coinText; // Ссылка на UI Text

    void Awake()
    {
        // Настройка синглтона
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // Раскомментировать, если нужно сохранять между сценами
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateCoinText(); // Обновляем UI при старте
    }

    // Метод для добавления монет
    public void AddCoins(int amount)
    {
        currentCoins += amount;
        UpdateCoinText();

        // Можно добавить звук или эффект здесь
        Debug.Log($"Added {amount} coins. Total: {currentCoins}");
    }

    // Метод для обновления UI
    void UpdateCoinText()
    {
        if (coinText != null)
        {
            coinText.text = $"Coins: {currentCoins}";
        }
    }
}

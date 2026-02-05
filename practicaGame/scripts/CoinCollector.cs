using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    // Висит на маленьком BoxCollider (Trigger_Collection)
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Получаем основной скрипт Coin с родительского объекта
            coin coinScript = GetComponentInParent<coin>();
            if (coinScript != null)
            {
                coinScript.Collect(other);
            }
        }
    }
}

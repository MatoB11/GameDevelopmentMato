using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public RectTransform healthBar; // RectTransform pre health bar
    public int coinReward = 10; // Počet coinov za zabitie nepriateľa
    public delegate void DeathDelegate();
    public event DeathDelegate OnDeath;

    private CoinManager coinManager;

    void Start()
    {
        currentHealth = maxHealth;
        coinManager = FindObjectOfType<CoinManager>();
        UpdateHealthBar(); // Inicializuj health bar
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0f)
        {
            currentHealth = 0f;
        }
        UpdateHealthBar(); // Aktualizuj health bar po poškodení

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public bool IsDead()
    {
        return currentHealth <= 0f;
    }

    void Die()
    {
        // Pridaj coiny, keď nepriateľ zomrie
        if (coinManager != null)
        {
            coinManager.AddCoins(coinReward);
        }

        // Pridajte sem všetky efekty alebo logiku, ktorá sa má vykonať, keď nepriateľ zomrie
        OnDeath?.Invoke();
        Destroy(gameObject);
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            float healthPercentage = currentHealth / maxHealth;
            healthBar.sizeDelta = new Vector2(healthPercentage * healthBar.sizeDelta.x, healthBar.sizeDelta.y);
        }
    }

    void OnDestroy()
    {
        OnDeath?.Invoke();
    }
}

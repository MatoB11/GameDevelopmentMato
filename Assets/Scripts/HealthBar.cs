using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBarImage;
    private EnemyHealth enemyHealth;

    void Start()
    {
        enemyHealth = GetComponentInParent<EnemyHealth>();
    }

    void Update()
    {
        if (enemyHealth != null)
        {
            healthBarImage.fillAmount = enemyHealth.currentHealth / enemyHealth.maxHealth;
        }
    }
}

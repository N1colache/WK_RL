using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 10;
    private int currentHealth;

    public bool IsDead { get; private set; }

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log(gameObject.name + " prend " + amount + " dégâts");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (IsDead) return;

        IsDead = true;
        Debug.Log(gameObject.name + " est mort");

        Destroy(gameObject);
    }

    public void Heal(int amount)
    {
        if (IsDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
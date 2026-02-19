using UnityEngine;
using UnityEngine.VFX;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    
    public bool canHeal = true;

    public System.Action<int, int> OnHealthChanged;

    public GameObject blood;
    private ParticleSystem bloodEffect;

    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        bloodEffect = blood.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        NoHealth();
    }

    public void TakeDamage(int amount)
    {
        if (bloodEffect != null)
        {
            bloodEffect.Play();
        }
        else
        {
            Debug.Log("No vfx");
        }
        
        currentHealth -= amount;

        if (currentHealth < 0)
            currentHealth = 0;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void NoHealth()
    {
        if (currentHealth == 0)
        {
            Destroy(this.gameObject);
        }
    }
}
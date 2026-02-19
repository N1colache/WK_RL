using UnityEngine;
using UnityEngine.VFX;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    
    public bool canHeal = true;

    public System.Action<int, int> OnHealthChanged;

    public GameObject blood;
    private GameObject deathScreen;
    private ParticleSystem bloodEffect;
    
    public AudioSource audioVoice;

    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        bloodEffect = blood.GetComponent<ParticleSystem>();
        deathScreen = GameObject.FindGameObjectWithTag("Death");
    }

    void Update()
    {
        NoHealth();
    }

    public void TakeDamage(int amount)
    {
        audioVoice.Play();
        
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
            if (gameObject.tag == "Player")
            {
               deathScreen.SetActive(true);
            }
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    private Slider slider;
    private Health playerHealth;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void Update()
    {
        if (slider == null)
            return;

        if (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                playerHealth = player.GetComponent<Health>();
            }
            else
            {
                return;
            }
        }

        if (playerHealth != null)
        {
            slider.maxValue = playerHealth.maxHealth;
            slider.value = playerHealth.currentHealth;
        }
    }
}
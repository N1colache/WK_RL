using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    [SerializeField] private Slider healthBarSlider;
    private Health health;

    void Awake()
    {
        health = GetComponent<Health>();
    }

    void Start()
    {
        healthBarSlider.maxValue = health.GetCurrentHealth();
        healthBarSlider.value = health.GetCurrentHealth();
    }

    void Update()
    {
        healthBarSlider.value = health.GetCurrentHealth();
    }
}

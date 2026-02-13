using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    public Slider healthBarSlider;
    public int maxHP = 100;
    public int currentHP;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        healthBarSlider.value = currentHP;
        healthBarSlider.maxValue = maxHP;
    }
}

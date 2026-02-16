using TMPro;
using UnityEngine;

public class PlayerCurrency : MonoBehaviour
{
    public TextMeshProUGUI textCurrency;
    public int currency;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        textCurrency.text = currency.ToString();
    }
}

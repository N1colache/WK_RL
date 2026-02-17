using TMPro;
using UnityEngine;

public class PlayerCurrencyUI : MonoBehaviour
{
    private TextMeshProUGUI textCurrency;
    public PlayerCurrency playerCurrency;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textCurrency = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (textCurrency == null)
            return;

        if (playerCurrency == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                playerCurrency = player.GetComponent<PlayerCurrency>();
            }
            else
            {
                return;
            }
        }

        if (playerCurrency != null)
        {
            textCurrency.text = playerCurrency.currency.ToString();
        }
    }
}

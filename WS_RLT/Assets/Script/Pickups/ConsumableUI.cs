using UnityEngine;
using UnityEngine.UI;

public class ConsumableUI : MonoBehaviour
{
    private Texture2D _textureConsumable;
    [SerializeField] public Texture2D bloodBagTexture;
    [SerializeField] public Texture2D transparentTexture;
    
    private RawImage _consumable;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _consumable = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        Health playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        if (playerHealth.canHeal == false)
        {
            _consumable.texture = transparentTexture;
            Debug.Log("HealUsed");
        }
        else
        {
            _consumable.texture = bloodBagTexture;
            Debug.Log("HealAvailable");
        }

    }
}

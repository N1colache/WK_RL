using TMPro;
using UnityEngine;

public class Shop : MonoBehaviour
{
   private OpenShop openShop;
    public float moneyStore;

    public GameObject blood;
    public GameObject ammo;
    private Controller controller;
    private PlayerCurrency playerCurrency;
    private Transform playerTransform;
    
    void Awake()
    {
        openShop = GetComponentInParent<OpenShop>();
    }
    void Start()
    {
        playerCurrency = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCurrency>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
         moneyStore = playerCurrency.currency;
         if (openShop.PlayerTransform != null)
         {
              playerTransform = openShop.PlayerTransform;
         }
    }

    public void UpgradeSpeed()
    {
        if (moneyStore >= 10)
        {
            moneyStore -= 10;
            // shootSpeed = shootSpeed + speedUpgrade
        }
    }

    public void UpgradeDamage()
    {
        if (moneyStore >= 15)
        {
            moneyStore -= 15;
            //shootDamage = shootDamage + damageUpgrade
        }
    }

    public void BuyAmmo()
    {
        if (moneyStore >= 5)
        {
            moneyStore -= 5;
            Instantiate(ammo, playerTransform.position, Quaternion.identity);
        }
    }

    public void BuyBlood()
    {
        if (moneyStore >= 10)
        {
            moneyStore -= 10;
            Instantiate(blood, playerTransform.position, Quaternion.identity);
        }
    }

     public void BuyRifle()
     {
         if (moneyStore >= 50)
         {
             moneyStore -= 50;
             //weapons =
         }
     }
}
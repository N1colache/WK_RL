using TMPro;
using UnityEngine;

public class Shop : MonoBehaviour
{
   private OpenShop openShop;
    public float moneyStore;

    [SerializeField] private GameObject blood;
    [SerializeField] private GameObject ammo;
    private Controller controller;
    private PlayerCurrency playerCurrency;
    private Transform playerTransform;
    private WeaponData weaponPlayer;
    
    void Awake()
    {
        openShop = GetComponentInParent<OpenShop>();
    }
    void Start()
    {
        playerCurrency = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCurrency>();
        weaponPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponData>();
        
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
        if (playerCurrency.currency >= 10)
        {
            playerCurrency.currency -= 10;
            // shootSpeed = shootSpeed + speedUpgrade
        }
    }

    public void UpgradeDamage()
    {
        if (playerCurrency.currency >= 15)
        {
            playerCurrency.currency -= 15;
            //shootDamage = shootDamage + damageUpgrade
        }
    }

    public void BuyAmmo()
    {
        if (playerCurrency.currency >= 5)
        {
            playerCurrency.currency -= 5;
            Instantiate(ammo, playerTransform.position, Quaternion.identity);
        }
    }

    public void BuyBlood()
    {
        if (playerCurrency.currency >= 10)
        {
            playerCurrency.currency -= 10;
            Instantiate(blood, playerTransform.position, Quaternion.identity);
        }
    }

     public void BuyRifle()
     {
         if (playerCurrency.currency >= 50)
         {
             playerCurrency.currency -= 50;
             weaponPlayer.UnlockBurst();
         }
     }
}
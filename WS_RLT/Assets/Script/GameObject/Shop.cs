using TMPro;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    public float moneyStore;
    public float speedUpgrade = 3f;
    public float damageUpgrade = 5f;
    public GameObject blood;
    public GameObject ammo;
    private Controller controller;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moneyText.text = ("Money" + moneyStore);
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
            //ammoTot = ammoTot + newAmmo
        }
    }

    public void BuyBlood()
    {
        if (moneyStore >= 10)
        {
            moneyStore -= 10;
            //bloodInv = bloodInv + newBlood
        }
    }

    /* public void BuyRifle()
     {
         if (moneyStore >= 50)
         {
             moneyStore -= 50;
             //weapons =
         }
     }*/
}
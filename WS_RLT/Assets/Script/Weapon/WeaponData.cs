using UnityEngine;

public class WeaponData : MonoBehaviour
{
    [SerializeField] private GameObject pistolPrefab;
    [SerializeField] private GameObject burstPrefab;
    [SerializeField] private GameObject shotgunPrefab;

    private GameObject pistol;
   
    private GameObject shotgun;

    private GameObject currentWeapon;

    [SerializeField] private bool burstUnlocked = false;
    [SerializeField] private Transform firePoint;


    void Start()
    {
        pistol = Instantiate(pistolPrefab, firePoint);
       
        shotgun = Instantiate(shotgunPrefab, firePoint);

        SetupWeapon(pistol);
       
        SetupWeapon(shotgun);

        EquipWeapon(pistol);
    }

    void Update()
    {
        // Clique gauche → Pistol
        if (Input.GetMouseButtonDown(0))
        {
            EquipWeapon(pistol);
        }

        // Clique droit → Burst si acheté
        if (Input.GetMouseButtonDown(1))
        {
            if (burstUnlocked)
            {
                EquipWeapon(shotgun);
            }
            else
            {
                Debug.Log("Burst non acheté !");
            }
        }
    }

    void SetupWeapon(GameObject weapon)
    {
        weapon.SetActive(false);
        weapon.transform.localPosition = new Vector3(0, 0, 0);
        weapon.transform.localRotation = Quaternion.identity;
        
        
    }

    void EquipWeapon(GameObject weaponToEquip)
    {
        pistol.SetActive(false);
      
        shotgun.SetActive(false);

        weaponToEquip.SetActive(true);
        currentWeapon = weaponToEquip;
    }

    // Appelé par le Shop
    public void UnlockBurst()
    {
        burstUnlocked = true;
        
        Debug.Log("Burst débloqué !");
    }
}
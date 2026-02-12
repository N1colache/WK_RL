using UnityEngine;

public class WeaponData : MonoBehaviour
{
    [SerializeField] private GameObject pistolPrefab;
    [SerializeField] private GameObject burstPrefab;
    [SerializeField] private GameObject shotgunPrefab;

    private GameObject pistol;
    private GameObject burst;
    private GameObject shotgun;

    private GameObject currentWeapon;

    [SerializeField] private bool burstUnlocked = false;

    void Start()
    {
        pistol = Instantiate(pistolPrefab, transform);
        burst = Instantiate(burstPrefab, transform);
        shotgun = Instantiate(shotgunPrefab, transform);

        SetupWeapon(pistol);
        SetupWeapon(burst);
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
                EquipWeapon(burst);
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
        weapon.transform.localPosition = new Vector3(1.5f, 2.8f, 0);
        weapon.transform.localRotation = Quaternion.identity;
        weapon.transform.localScale = Vector3.one;
        
    }

    void EquipWeapon(GameObject weaponToEquip)
    {
        pistol.SetActive(false);
        burst.SetActive(false);
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
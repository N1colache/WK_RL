using UnityEngine;
[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/WeaponData")]
public class WeaponData : MonoBehaviour
{
    public enum WeaponType
    {
        Pistol,
        Burst,
        shotgun
        
    }

    
    
    
    public string weaponName;
    public WeaponType weaponType;

    [Header("Projectile")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 20f;

    [Header("Ammo")] 
    public int CurrentAmo;
    public int maxAmmo = 30;
    public float reloadTime = 2f;

    [Header("Fire Rate")]
    public float fireRate = 0.2f;

    [Header("Burst Settings")]
    public int bulletsPerBurst = 3;
    public float timeBetweenBullets = 0.1f;
    

}

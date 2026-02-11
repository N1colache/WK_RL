using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public WeaponData currentWeapon;

    public Transform firePoint;

    private Inputs inputs;

    private float fireCooldown = 0f;

    // Variables rafale
    private bool isBursting = false;
    private int bulletsLeftInBurst = 0;
    private float burstTimer = 0f;

    private int currentAmmo;

    void Start()
    {
        inputs = GetComponent<Inputs>();

        if (currentWeapon != null)
            currentAmmo = currentWeapon.maxAmmo;
    }

    void Update()
    {
        if (currentWeapon == null) return;

        fireCooldown -= Time.deltaTime;

        // Lancement du tir
        if (inputs._shoot && fireCooldown <= 0f)
        {
            StartShooting();
            inputs._shoot = false;
        }

        HandleBurst();
    }

    void StartShooting()
    {
        if (currentAmmo <= 0)
        {
            Reload();
            return;
        }

        switch (currentWeapon.weaponType)
        {
            case WeaponData.WeaponType.Pistol:
                ShootOneBullet();
                break;

            case WeaponData.WeaponType.Burst:
                StartBurst();
                break;
        }

        fireCooldown = currentWeapon.fireRate;
    }

    void StartBurst()
    {
        isBursting = true;
        bulletsLeftInBurst = currentWeapon.bulletsPerBurst;
        burstTimer = 0f;
    }

    void HandleBurst()
    {
        if (!isBursting) return;

        burstTimer += Time.deltaTime;

        if (burstTimer >= currentWeapon.timeBetweenBullets)
        {
            ShootOneBullet();
            bulletsLeftInBurst--;
            burstTimer = 0f;

            if (bulletsLeftInBurst <= 0)
            {
                isBursting = false;
            }
        }
    }

    void ShootOneBullet()
    {
        if (currentAmmo <= 0) return;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(firePoint.position).z;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        Vector3 direction = (worldPos - firePoint.position).normalized;

        GameObject bullet = Instantiate(
            currentWeapon.bulletPrefab,
            firePoint.position,
            Quaternion.identity
        );

        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (rb != null)
            rb.linearVelocity = direction * currentWeapon.bulletSpeed;

        bullet.transform.forward = direction;

        currentAmmo--;
    }

    void Reload()
    {
        currentAmmo = currentWeapon.maxAmmo;
    }
}

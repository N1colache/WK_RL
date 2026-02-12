using UnityEngine;

public class Fire : MonoBehaviour
{
    private Inputs inputs;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform barrel;

    private float bulletSpeed = 500f;

    [SerializeField] private string currentWeapon;

    void Start()
    {
        inputs = GetComponentInParent<Inputs>();
    }

    void Update()
    {
        if (inputs._shoot)
        {
            WeaponFire(currentWeapon);
        }
    }

    private void WeaponFire(string weaponName)
    {
        if (weaponName == "Pistol")
        {
            FireSingleBullet(barrel.forward);
        }
        else if (weaponName == "Shotgun")
        {
            FireShotgun();
        }
    }

    void FireSingleBullet(Vector3 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, barrel.position, barrel.rotation);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(direction * bulletSpeed);
        }
    }

    void FireShotgun()
    {
        float spreadAngle = 10f;

        for (int i = -1; i <= 1; i++)
        {
            Quaternion spreadRotation =
                Quaternion.Euler(0, i * spreadAngle, 0) * barrel.rotation;

            GameObject bullet = Instantiate(bulletPrefab, barrel.position, spreadRotation);

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(spreadRotation * Vector3.forward * bulletSpeed);
            }
        }
    }
}
using UnityEngine;

public class Fire : MonoBehaviour
{
    private Inputs _inputs;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform barrel;
    [SerializeField] private string currentWeapon = "Pistol";
    private float bulletSpeed = 500f;

    // Burst variables
    [SerializeField] private int burstCount = 3;
    [SerializeField] private float burstDelay = 0.08f;
    private int burstRemaining = 0;
    private float burstTimer = 0f;
    private bool isBursting = false;

    void Start()
    {
        _inputs = GetComponentInParent<Inputs>();
    }

    void Update()
    {
        // Timer pour burst
        if (isBursting)
        {
            burstTimer -= Time.deltaTime;
            if (burstTimer <= 0f && burstRemaining > 0)
            {
                FireSingleBullet(GetMouseDirectionX());

                burstRemaining--;
                burstTimer = burstDelay;
            }
            if (burstRemaining == 0)
                isBursting = false;
        }

        // Input pour tirer
        if (currentWeapon == "Pistol" && _inputs._shootLeft)
        {
            FireSingleBullet(GetMouseDirectionX());

            _inputs._shootLeft = false;
        }
        else if (currentWeapon == "Burst" && _inputs._shootRight && !isBursting)
        {
            StartBurst();
            _inputs._shootRight = false;
        }
    }

    void StartBurst()
    {
        isBursting = true;
        burstRemaining = burstCount;
        burstTimer = 0f; // tir immédiat
    }

    // Direction vers la souris uniquement en Z
    Vector3 GetMouseDirectionX()
    {
        if (Camera.main == null)
            return Vector3.right;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, barrel.position); // plan horizontal

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);

            float xDirection = hitPoint.x - barrel.position.x;

            if (Mathf.Abs(xDirection) < 0.01f)
                return Vector3.right;

            return new Vector3(Mathf.Sign(xDirection), 0, 0);
        }

        return Vector3.right;
    }


    void FireSingleBullet(Vector3 direction)
    {
        if (direction == Vector3.zero) direction = Vector3.forward;
        
        Quaternion rotation = Quaternion.LookRotation(direction);

        GameObject bullet = Instantiate(bulletPrefab, barrel.position, rotation);

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetOwner(transform.root.gameObject);
        }

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
            rb.AddForce(direction * bulletSpeed);
    }
}

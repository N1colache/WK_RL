using UnityEngine;

public class Fire : MonoBehaviour
{
    public Inputs _inputs;
    private PlayerAnimator animator;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform barrel;
    [SerializeField] private string currentWeapon = "Pistol";
    [SerializeField] private float bulletSpeed = 30f;

    // Burst variables
    [SerializeField] private int burstCount = 3;
    [SerializeField] private float burstDelay = 0.08f;
    private int burstRemaining = 0;
    private float burstTimer = 0f;
    private bool isBursting = false;
    
    [Header("Ammo")]
    [SerializeField] private int magazineSize = 5;
    [SerializeField] private int magazineCapacity = 20;
    [SerializeField] private float reloadTime = 10;
    private float reloadTimer = 0.6f ;
    
    [Header("Shotgun")]
    [SerializeField] private int pelletCount = 5;      // nombre de projectiles
    [SerializeField] private float spreadAmount = 0.2f; // largeur du spread

    private int currentAmmo;
    private bool isReloading = false;

    private float waitForAnnim;

    void Start()
    {
        _inputs = GetComponentInParent<Inputs>();
        currentAmmo = magazineSize;
        
    }

    void Update()
    {
        
        
            // Si _inputs est null, c’est probablement un ennemi → ne pas exécuter la partie joueur
            if (_inputs != null)
            {
                waitForAnnim -= Time.deltaTime;
                if (waitForAnnim <= 0)
                HandlePlayerInput();
                
            }

            HandleBurstTimer();
            HandleReloadTimer();
        


        // Gestion reload timer
        if (isReloading)
        {
            reloadTimer -= Time.deltaTime;

            if (reloadTimer <= 0f)
            {
                int bulletsNeeded = magazineSize - currentAmmo;
                int bulletsToLoad = Mathf.Min(bulletsNeeded, magazineCapacity);

                currentAmmo += bulletsToLoad;
                magazineCapacity -= bulletsToLoad;

                isReloading = false;

                //Debug.Log("Reload terminé");
            }

            return; // empêche de tirer pendant reload
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
            rb.AddForce(direction * bulletSpeed, ForceMode.Impulse);
    }

    public void StartReload()
    {
        if (isReloading) return;
        if (magazineCapacity <= 0) return;
        if (currentAmmo == magazineSize) return;

        isReloading = true;
        reloadTimer = reloadTime;
        
        currentAmmo = magazineSize;
        magazineCapacity = magazineCapacity - magazineSize;

    }
    public void ShootAt(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - barrel.position).normalized;
        direction.y = 0;

        FireSingleBullet(direction);
    }
    public void ShootShotgunAt(Vector3 targetPosition)
    {
        if (currentAmmo <= 0) return;

        // Direction principale sur X uniquement
        float directionX = Mathf.Sign(targetPosition.x - barrel.position.x);

        for (int i = 0; i < pelletCount; i++)
        {
            // Spread vertical (axe Y uniquement)
            float randomSpreadY = Random.Range(-spreadAmount, spreadAmount);

            Vector3 direction = new Vector3(
                directionX,
                randomSpreadY,
                0
            ).normalized;

            GameObject bullet = Instantiate(bulletPrefab, barrel.position, Quaternion.identity);

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
                bulletScript.SetOwner(transform.root.gameObject);

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddForce(direction * bulletSpeed, ForceMode.Impulse);
        }

        currentAmmo--;
    }

    Vector3 GetShotgunTargetPosition()
    {
        if (Camera.main == null)
            return barrel.position + Vector3.up;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.right, barrel.position); 

        if (plane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }

        return barrel.position + Vector3.up;
    }
    void HandlePlayerInput()
    {
        // Pistol
        if (currentWeapon == "Pistol" && _inputs._shootLeft && !isReloading)
        {
            if (currentAmmo > 0)
            {
                FireSingleBullet(GetMouseDirectionX());
                currentAmmo--;
            }
            else
            {
                Debug.Log("Plus de munitions !");
                StartReload();
            }
            _inputs._shootLeft = false;
        }

        // Burst
        if (currentWeapon == "Burst" && _inputs._shootRight && !isBursting)
        {
            StartBurst();
            _inputs._shootRight = false;
        }

        // Shotgun
        if (currentWeapon == "Shotgun" && _inputs._shootLeft && !isReloading)
        {
            if (currentAmmo > 0)
            {
                ShootShotgunAt(GetShotgunTargetPosition());
            }
            else
            {
                Debug.Log("Plus de munitions !");
                StartReload();
            }
            _inputs._shootLeft = false;
        }
       
        
    }
    void HandleBurstTimer()
    {
        if (!isBursting) return;

        burstTimer -= Time.deltaTime;
        if (burstTimer <= 0f && burstRemaining > 0)
        {
            if (currentAmmo > 0)
            {
                FireSingleBullet(GetMouseDirectionX());
                currentAmmo--;
                burstRemaining--;
                burstTimer = burstDelay;
            }
            else
            {
                Debug.Log("Plus de munitions !");
                if (!isReloading)
                    StartReload();

                isBursting = false;
            }
        }
        if (burstRemaining == 0)
            isBursting = false;
    }

    void HandleReloadTimer()
    {
        if (!isReloading) return;

        reloadTimer -= Time.deltaTime;
        if (reloadTimer <= 0f)
        {
            int bulletsNeeded = magazineSize - currentAmmo;
            int bulletsToLoad = Mathf.Min(bulletsNeeded, magazineCapacity);

            currentAmmo += bulletsToLoad;
            magazineCapacity -= bulletsToLoad;

            isReloading = false;
            Debug.Log("Reload terminé");
        }
    }


    public void AddAmmo(int amount)
    {
        currentAmmo += amount;
    }
    
}

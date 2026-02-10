using System;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    private Controller controller;
    private Inputs inputs;
    
    [Header("Tir")]
    public GameObject bulletPrefab;     // Prefab de la balle
    public Transform firePoint;         // Point d'origine du tir
    public float bulletSpeed = 20f;     // Vitesse de la balle
    
    [Header("Rafale")]
    [SerializeField] private int bulletsPerBurst = 3;      // 3 balles par rafale
    [SerializeField] private float timeBetweenBullets = 0.1f; // délai entre chaque balle
    private int bulletsLeftInBurst = 0;  // compteur de balles restantes
    private float bulletTimer = 0f;  
    private float timeBetweenBurst = 0f;
    private bool isBursting = false;
    
    [Header("Ragdoll")]
    [SerializeField] private int amo = 3;
    [SerializeField] private int maxAmo;
    [SerializeField] private float reloadTime = 2f;
    private bool isReloading = false;

    [Header("Aim Gizmo")]
    public bool showGizmo = true;       // Afficher Gizmo
    public float gizmoSize = 0.2f;      // Taille du cube du Gizmo

    [SerializeField] private bool haveShoot;

    private void Start()
    {
        inputs = GetComponent<Inputs>();
    }

    void Update()
    {
       
        {
            if (firePoint == null || bulletPrefab == null || Camera.main == null) return;

            // Début de la rafale quand le joueur clique
            if (inputs != null && inputs._shoot && !isBursting)
            {
                if (amo >= 4)
                {
                    
                    bulletsLeftInBurst = bulletsPerBurst;
                    isBursting = true;
                    bulletTimer = 0f;
                    inputs._shoot = false; // reset l'input
                }
                else
                {
                    StartReloading();
                }
                
            }

            // Tir de la rafale
            if (isBursting)
            {
                bulletTimer += Time.deltaTime;

                if (bulletTimer >= timeBetweenBullets)
                {
                    ShootOneBullet();
                    bulletsLeftInBurst--;
                    bulletTimer = 0f;

                    // Rafale terminée
                    if (bulletsLeftInBurst <= 0)
                    {
                        isBursting = false;
                    }
                }
            }
        }

        
    }

    void ShootOneBullet()
    {
        // Position de la souris
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(firePoint.position).z;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        // Calcul de la direction
        Vector3 direction = (worldPos - firePoint.position).normalized;

        // Instanciation de la balle
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // Appliquer la vitesse
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = direction * bulletSpeed;

        // Orienter la balle
        bullet.transform.forward = direction;

        amo = amo - 1;
    }

    void StartReloading()
    {
        
    }
}

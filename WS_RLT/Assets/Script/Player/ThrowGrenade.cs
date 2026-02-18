using TreeEditor;
using UnityEngine;

public class ThrowGrenade : MonoBehaviour
{
    [Header("Grenade Settings")]
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float throwForce = 10f;

    [Header("Cooldown")]
    [SerializeField] private float cooldown = 1.5f;
    
    private float nextThrowTime = 0f;
    private Transform playerTransform;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
    
    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Q) && Time.time >= nextThrowTime)
        {
            Throw();
            nextThrowTime = Time.time + cooldown;
        }
    }

    private void Throw()
    {
        //play animation
        
        GameObject grenade = Instantiate(grenadePrefab, firePoint.position, Quaternion.identity);

        Rigidbody2D rb = grenade.GetComponentInChildren<Rigidbody2D>();

        Vector2 throwDirection = playerTransform.rotation.y > 0 ? Vector2.right : Vector2.left;

        rb.AddForce((throwDirection + Vector2.up) * throwForce, ForceMode2D.Impulse);
    }
}

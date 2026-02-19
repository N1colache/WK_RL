using UnityEngine;
using UnityEngine.VFX;

public class Grenade : MonoBehaviour
{

    [Header("Explosion Settings")]
    [SerializeField] private float explosionDelay = 1f;
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private int damage = 20;
    [SerializeField] private LayerMask enemyLayer;

    private bool hasExploded = false;
    public GameObject parent;

    public GameObject vfx;
    private VisualEffect explosion_VFX;
    
    public AudioSource audioExplode;
    
    void Start()
    {
        Invoke("VFXExplosion", 1.1f);
        Invoke("Explode", explosionDelay);
    }

    private void VFXExplosion()
    {
        vfx.SetActive(true);
        explosion_VFX = vfx.GetComponent<VisualEffect>();
        explosion_VFX.Play();
    }
    
    void Explode()
    {
        audioExplode.Play();
        
        hasExploded = true;
        Rigidbody2D rb = GetComponentInChildren<Rigidbody2D>();
        
        rb.bodyType = RigidbodyType2D.Static;
        
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayer);

        foreach (Collider2D enemy in enemies)
        {
            Health health = enemy.GetComponent<Health>();

            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
        
        // MeshRenderer renderer = GetComponent<MeshRenderer>();
        // renderer.enabled = false;
        
        Destroy(parent, 0.8f);
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Explode();
        }
    }
}
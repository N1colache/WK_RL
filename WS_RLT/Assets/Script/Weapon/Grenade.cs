using UnityEngine;

public class Grenade : MonoBehaviour
{

    [Header("Explosion Settings")]
    [SerializeField] private float explosionDelay = 2f;
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private int damage = 20;
    [SerializeField] private LayerMask enemyLayer;

    private bool hasExploded = false;
    public GameObject parent;
    
    void Start()
    {
        Invoke("Explode", explosionDelay);
    }

    void Explode()
    {
        //if (hasExploded) return;
        hasExploded = true;
        
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayer);

        foreach (Collider2D enemy in enemies)
        {
            Health health = enemy.GetComponent<Health>();

            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }

        
        //ajouter vfx ici
        Destroy(parent);
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Explode();
        }
    }
}
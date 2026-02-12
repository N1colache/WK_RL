using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifeTime = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime); // détruit si ne touche rien
    }
    
    private GameObject owner;

    public void SetOwner(GameObject shooter)
    {
        owner = shooter;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == owner)
            return;

        Health health = collision.gameObject.GetComponent<Health>();

        if (health != null)
        {
            health.TakeDamage(damage);
        }

        Destroy(gameObject);
    }

}
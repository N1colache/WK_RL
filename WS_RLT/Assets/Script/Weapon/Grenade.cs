using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float speedGrenade = 4f;
    public bool thrown = false;
    public Vector3 launchOffset;
    
    
    public void throwGrenade()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("key pressed");
            if (thrown)
            {
                var direction = transform.right + Vector3.up;
                GetComponent<Rigidbody2D>().AddForce(direction * speedGrenade, ForceMode2D.Impulse);
            }
            transform.Translate(launchOffset);
            Destroy(gameObject, 5f);
        }
       
    }
       
    

    // Update is called once per frame
    void Update()
    {
        if (!thrown)
        {
            transform.position += transform.right * speedGrenade * Time.deltaTime;

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        /*var enemy = collision.collider.GetComponent<EnemyBehaviour>();
        if (enemy)
        {
            enemy.TakeDamage(10);
        }*/
        Destroy(gameObject);
    }

}
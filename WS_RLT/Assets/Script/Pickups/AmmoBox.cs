using UnityEngine;

public class AmmoBox : MonoBehaviour
{

    private Collider _collider;
    private GameObject _player;
    private Fire _ammo;
    [SerializeField] private int ammoToGive;


    public void Start()
    {
        _collider = GetComponent<SphereCollider>();
        _player = GameObject.FindGameObjectWithTag("Player");
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _ammo = _player.GetComponent<Fire>();
            _ammo.AddAmmo(ammoToGive);
            
            Destroy(this.gameObject);
        }
    }
    
    
}

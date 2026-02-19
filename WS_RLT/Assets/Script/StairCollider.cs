using UnityEngine;

public class StairCollider : MonoBehaviour
{

    public void DisableCollider()
    {
        this.gameObject.GetComponent<Collider>().enabled = false;
        Debug.Log("Stair Collider/ DisabledStairs");
    }

    public void EnableCollider()
    {
        this.gameObject.GetComponent<Collider>().enabled = true;
        Debug.Log("Stair Collider/EnabledStairs");
    }
    
    
    
    
}

using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    [SerializeField] public float distance = 0.5f;       
    [SerializeField] public float radius = 0.2f;         
    [SerializeField] private LayerMask groundLayer;  
    [SerializeField] private LayerMask stairLayer;

    public bool touched;  
    public bool  stairTouched;
    private CharacterController controller;
  

    void Start()
    {
        controller = GetComponentInParent<CharacterController>();
    }

    void Update()
    {
        
        Vector3 start = transform.position;

        touched = controller.isGrounded;
        stairTouched = Physics.SphereCast(
            start, 
            radius, 
            Vector3.down, 
            out RaycastHit hit, 
            distance, 
            stairLayer, 
            QueryTriggerInteraction.Ignore);
         
        if (stairTouched)
        {
            Debug.Log("Touché : " + hit.collider.name);
        }
            

        
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = stairTouched ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * distance);
        Gizmos.DrawSphere(transform.position + Vector3.down * distance, radius);
    }
}
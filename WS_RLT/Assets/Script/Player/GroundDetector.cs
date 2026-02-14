using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    [SerializeField] public float distance = 0.5f;       
    [SerializeField] public float radius = 0.2f;         
    [SerializeField] private LayerMask groundLayer;  
    [SerializeField] private LayerMask stairLayer;

    public bool touched;  
    public bool  stairTouched;

    private Collider selfCollider;                        // pour s'auto-ignorer

    void Start()
    {
        selfCollider = GetComponentInParent<Collider>();
    }

    void Update()
    {
        
        Vector3 start = transform.position;

        
        touched = Physics.SphereCast(start, radius, Vector3.down, out RaycastHit hitGround, distance, groundLayer, QueryTriggerInteraction.Ignore);
        stairTouched = Physics.SphereCast(start, radius, Vector3.down, out RaycastHit hitStair, distance, stairLayer, QueryTriggerInteraction.Ignore);
         

        if (touched && hitGround.collider == selfCollider)
        {
          touched = false;  
        }

        if (stairTouched && hitStair.collider == selfCollider)
        {
            stairTouched = false;
        }
            

        Debug.Log(stairTouched);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = stairTouched ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * distance);
        Gizmos.DrawSphere(transform.position + Vector3.down * distance, radius);
    }
}
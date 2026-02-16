using System;
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
    public GameObject _collider;
    

    private Controller ControllerRef;

    private float stairTimer;
    [SerializeField] private float stairBufferTime = 0.15f;
    void Start()
    {
        controller = GetComponentInParent<CharacterController>();
        ControllerRef = GetComponentInParent<Controller>();
    }

    void Update()
    {
        Vector3 start = transform.position;
        Vector3 startUp = transform.position ;

        if (Physics.SphereCast(
                start,
                radius,
                Vector3.down,
                out RaycastHit hit,
                distance,
                stairLayer,
                QueryTriggerInteraction.Ignore))
        {
            stairTouched = true;
            _collider = hit.collider.gameObject;
            stairTimer = stairBufferTime;
        }
        else
        {
            stairTimer -= Time.deltaTime;

            if (stairTimer <= 0f)
            {
                stairTouched = false;
                _collider = null;
                ControllerRef.disableStair = true;
                
            }
        }
       
        touched = controller.isGrounded;
       
        
    }

    

    private void OnDrawGizmos()
    {
        Gizmos.color = stairTouched ? Color.green : Color.red;
     
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * distance);
        
        Gizmos.DrawSphere(transform.position + Vector3.down * distance, radius);
      
    }
}
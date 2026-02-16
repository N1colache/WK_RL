using System;
using UnityEngine;

public class GroundDetectorUp : MonoBehaviour
{
          
    [SerializeField] public float distanceUp = 0.5f;       
    [SerializeField] public float radius = 0.2f;         
    [SerializeField] private LayerMask groundLayer;  
    [SerializeField] private LayerMask stairLayer;

    public bool touched;  
    
    public bool  stairUpTouched;
    private CharacterController controller;
   
    public GameObject _colliderUp;

    private Controller ControllerRef;

    private float stairTimer;
    [SerializeField] private float stairBufferTime = 0.15f;
    void Start()
    {
        controller = GetComponentInParent<CharacterController>();
    }

    void Update()
    {
        
        Vector3 startUp = transform.position ;
        
        if (Physics.SphereCast(
                startUp ,
                radius,
                Vector3.up,
                out RaycastHit hitUp,
                distanceUp,
                stairLayer,
                QueryTriggerInteraction.Ignore))
        {
            stairUpTouched = true;
            _colliderUp = hitUp.collider.gameObject;
            stairTimer = stairBufferTime;
        }
        else
        {
            stairTimer -= Time.deltaTime;

            if (stairTimer <= 0f)
            {
                stairUpTouched = false;
                _colliderUp = null;
                ControllerRef.disableStair = true;
                
            }
        }
        
       
        
    }

    

    private void OnDrawGizmos()
    {
       
        Gizmos.color = stairUpTouched ? Color.green : Color.red;
        
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * distanceUp);
        
        Gizmos.DrawSphere(transform.position + Vector3.up * distanceUp, radius);
    }
}
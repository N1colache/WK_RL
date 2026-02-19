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
        Vector3 point = transform.position;
        stairTouched = Physics.CheckSphere(point,radius,stairLayer,QueryTriggerInteraction.Ignore);

        if (stairTouched)
        {
            Collider col = Physics.OverlapSphere(point,radius,stairLayer)[0];_collider = col.gameObject;
        }
        else
        {
            _collider = null;
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
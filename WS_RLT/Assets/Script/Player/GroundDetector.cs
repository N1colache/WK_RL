using System;
using UnityEngine;

// ReSharper disable once InconsistentNaming
public class GroundDetector : MonoBehaviour
{
    [SerializeField] private float distance = 0.5f;
    
    public bool touched;
    
    [SerializeField] private LayerMask groundLayer;

    void Update()
    {
        touched = Physics.Raycast(transform.position, Vector3.down, distance, groundLayer);
        Debug.Log(touched);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = touched ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * distance);
        Gizmos.DrawSphere(transform.position + Vector3.down * distance, 0.05f);
        
    }
}
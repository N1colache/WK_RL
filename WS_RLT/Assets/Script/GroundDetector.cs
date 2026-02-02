using System;
using UnityEngine;

// ReSharper disable once InconsistentNaming
public class GroundDetector : MonoBehaviour
{
    [SerializeField] private float distance = 0.5f;
    
    public bool touched;
    
    // Update is called once per frame
    void Update()
    {
        touched = Physics.Raycast(transform.position, Vector3.down, distance);
    }

    private void OnDrawGizmos()
    {
        if (touched)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.yellow;
        }
        Gizmos.DrawRay(transform.position, Vector3.down * distance);
        
    }
}
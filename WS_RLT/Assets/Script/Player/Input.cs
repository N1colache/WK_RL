using System;
using UnityEngine;
using UnityEngine.InputSystem;

// ReSharper disable once InconsistentNaming
public class Inputs : MonoBehaviour
{

    public Vector2 Move;
    public bool Jump;
 

    public void OnMove(InputValue value)
    {
        Move = value.Get<Vector2>();
    }
    public void OnJump(InputValue value)
    {
        Jump = value.isPressed;
    }
   
    
    
}
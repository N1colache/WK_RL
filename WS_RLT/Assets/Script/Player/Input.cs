using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

// ReSharper disable once InconsistentNaming
public class Inputs : MonoBehaviour
{
    private Controller _controller;

    public Vector2 Move;
    public bool canMove;
    public bool Jump;
    public float horizontal;
    public bool _dashing;

    private void Start()
    {
        _controller = GetComponent<Controller>();
    }

    public void OnMoveHorizontal(InputValue value)
    {
        horizontal = value.Get<float>();
        //canMove = value.isPressed;
    }
    public void OnMove(InputValue value)
    {
        Move = value.Get<Vector2>();
        //canMove = value.isPressed;
        
    }
    public void OnJump(InputValue value)
    {
        Jump = value.isPressed;
        Debug.Log(Jump);
    }

    public void OnDash(InputValue value)
    {
        _dashing = value.isPressed;
        Debug.Log("Dash");
       
    }
    
   
    
    
}
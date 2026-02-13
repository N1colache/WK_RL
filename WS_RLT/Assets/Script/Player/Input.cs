using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

// ReSharper disable once InconsistentNaming
public class Inputs : MonoBehaviour
{
    private Controller _controller;

    public Vector2 Move;
    public bool Jump;
    public float horizontal;
    
    public bool _dashing;

    public bool _shootLeft;
    public bool _shootRight;

    private void Start()
    {
        _controller = GetComponent<Controller>();
    }

    public void OnMoveHorizontal(InputValue value)
    {
        horizontal = value.Get<float>();
        
    }
    public void OnMove(InputValue value)
    {
        Move = value.Get<Vector2>();
        
        
    }
    public void OnJump(InputValue value)
    {
        Jump = value.isPressed;
    }

    public void OnDash(InputValue value)
    {
        _dashing = value.isPressed;
    }

    public void OnShootLeft(InputValue value)
    {
        if (value.isPressed)
        _shootLeft = true;
        Debug.Log("Shoot");
    }
    public void OnShootRight(InputValue value)
    {
        if (value.isPressed) 
            _shootRight = true;
        Debug.Log("Shoot");
    }
   
    
    
}
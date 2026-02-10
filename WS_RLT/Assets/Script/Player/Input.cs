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

    public bool _shoot;

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

    public void OnShoot(InputValue value)
    {
        _shoot = value.isPressed;
        Debug.Log("Shoot");
    }
    
   
    
    
}
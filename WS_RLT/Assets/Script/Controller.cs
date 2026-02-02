using System;
using UnityEngine;
using UnityEngine.InputSystem;

// ReSharper disable once InconsistentNaming
public class TPS_Controller : MonoBehaviour
{
    [SerializeField] private float groundSpeed = 10;
    [SerializeField] private float airSpeed = 8; // TODO : Air Control
    [SerializeField] private float sneakSpeed = 3;
    [SerializeField] private float turnSpeed = 10;

    [SerializeField] [Tooltip("Obsolete, use jump height")]
    private float jumpForce = 0.15f;

    [SerializeField] private float jumpHeight = 1f; // TODO : Constant height jump
    [SerializeField] private float jumpTimeTotal = 0.1f;

    // TODO : Fast falling
    [SerializeField] private float fastFallingFactor = 2;

    [SerializeField] private GroundDetector groundDetector;

    private Rigidbody _rb;
    private Inputs _inputs;
    //private TPS_Animator _animator;

    private float _jumpTimeDelta;
    private Vector3 _initialGravity;
    public bool _landingDone = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        //_inputs = GetComponent<TPS_Inputs>();
        //_animator = GetComponent<TPS_Animator>();
        _jumpTimeDelta = 0;
        _initialGravity = Physics.gravity;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 horizontalMove = Vector3.zero;
        Vector3 verticalMove = Vector3.zero;
        Vector3 rotationMove = Vector3.zero;


        if (groundDetector.touched)
            _jumpTimeDelta -= Time.deltaTime;
        else
            _jumpTimeDelta = jumpTimeTotal;

        // Vertical move
        if (_inputs.Jump && groundDetector.touched && _jumpTimeDelta <= 0.0f)
        {
            // TODO : Constant height jump
            // Jump force obsolete, use jump height for constant height jump
            // verticalMove = jumpForce * Vector3.up;
            verticalMove = new Vector3(0, Mathf.Sqrt(jumpHeight * -2.0f * _initialGravity.y), 0);
            _jumpTimeDelta = jumpTimeTotal;
        }

        // Horizontal move -------------------------------------------------------------------------------------
        // TODO : Air Control
        // Also available for Strafe movement
        float realSpeed = groundDetector.touched ? groundSpeed : airSpeed;

        

        // TODO : Fast falling
        float realGravity = _rb.linearVelocity.y >= 0 ? _rb.linearVelocity.y : fastFallingFactor * _rb.linearVelocity.y;
        // TODO : Fast falling, but not too fast
        realGravity = Mathf.Max(realGravity, -40);

        // Apply to physics ------------------------------------------------------------
        _rb.linearVelocity = new Vector3(0, _rb.linearVelocity.y, 0) + horizontalMove + verticalMove;
        _rb.angularVelocity = rotationMove;
        
        // Animations -----------------------------------------------------------------
        Vector3 localVelocity = transform.InverseTransformDirection(_rb.linearVelocity);
        
        
        
    }
    
    void OnLandingBegin() => _landingDone = false;
    void OnLandingEnd() => _landingDone = true;
    
}
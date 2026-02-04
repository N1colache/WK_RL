using UnityEditor.Rendering.LookDev;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Controller : MonoBehaviour
{
    private Inputs _inputs;

    [Header("Déplacement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] public float dashSpeed;
    [SerializeField] private float dashDecaySpeed;
    [SerializeField] private bool _dashing;

    [Header("Saut")]
    [SerializeField] private float _jumpHeight = 2f;
    [SerializeField] private float _gravity = -9.81f;
    private float _jumpTimeDelta;
    [SerializeField] private float jumpTimeTotal = 0.1f;


    private Vector3 velocity;

    [SerializeField] private CharacterController controller;
    [SerializeField] private GroundDetector _groundDetector;
    public Rigidbody _rb;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        _inputs = GetComponent<Inputs>();
    }
   
    
    void Update()
    {
        
        if (_groundDetector.touched)
        {
            _jumpTimeDelta -= Time.deltaTime;
        
        }
        else
        {
            _jumpTimeDelta = jumpTimeTotal;
        }
        // Movement
        Vector2 input = _inputs.Move;

        Vector3 move = new Vector3(input.x, 0, 0);
        

        controller.Move(move * moveSpeed * Time.deltaTime);
        if (move.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(move);
        }

        // Jump
        if (_inputs.Jump && _groundDetector.touched && _jumpTimeDelta <= 0.1f)
        {
            velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
        }

        // Gravity
        velocity.y += _gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
        
        
    }
   
  
}
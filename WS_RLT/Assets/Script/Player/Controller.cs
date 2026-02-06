
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Controller : MonoBehaviour
{
    private Inputs _inputs;

    [Header("Déplacement")]
    [SerializeField] private float moveSpeed;

    [Header("Saut")]
    [SerializeField] private float _jumpHeight = 2f;
    [SerializeField] private float _gravity = -9.81f;
    private float _jumpTimeDelta;
    [SerializeField] private float jumpTimeTotal = 0.1f;
    
    [Header("Dash")]
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    private bool isDashing = false;
    private float dashTimer;
    private float cooldownTimer;
    private Vector3 dashDirection;



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
        if (_inputs._dashing && !isDashing && cooldownTimer <= 0)
        {
            if (!isDashing && _groundDetector)
            {
                 StartDash();
            }
           
        }
       

        if (_groundDetector.touched)
        {
            _jumpTimeDelta -= Time.deltaTime;
        
        }
        else
        {
            _jumpTimeDelta = jumpTimeTotal;
        }
        if (isDashing)
        {
            controller.Move(dashDirection * dashSpeed * Time.deltaTime);

            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0)
            {
                isDashing = false;
            }

            return; // on ignore le reste du mouvement pendant le dash
        }
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
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
    void StartDash()
    {
        isDashing = true;
        dashTimer = dashTime;
        cooldownTimer = dashCooldown;

        Vector2 input = _inputs.Move;

        // Si le joueur ne bouge pas, dash vers l’avant
        if (input.magnitude > 0.1f)
            dashDirection = new Vector3(input.x, 0, input.y).normalized;
        else
            dashDirection = transform.forward;
    }

   
  
}
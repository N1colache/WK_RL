using UnityEngine;

public class Controller : MonoBehaviour
{
    private Inputs _inputs;

    [Header("Déplacement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Saut")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float coyoteTime = 0.2f; // temps après avoir quitté le sol pour encore sauter
    private float coyoteTimer;

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

    void Start()
    {
        controller = GetComponent<CharacterController>();
        _inputs = GetComponent<Inputs>();
    }

    void Update()
    {
        if (controller == null || _inputs == null) return;

        // -----------------------------
        // Dash
        // -----------------------------
        if (_inputs._dashing && !isDashing && cooldownTimer <= 0f && _groundDetector.touched)
        {
            StartDash();
        }

        if (isDashing)
        {
            controller.Move(dashDirection * dashSpeed * Time.deltaTime);
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
            }

            // Ignore le reste du mouvement pendant le dash
            return;
        }

        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        
        // Coyote time pour jump
        
        if (_groundDetector.touched)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;

        
        // Movement
        
        Vector2 input = _inputs.Move;
        Vector3 move = new Vector3(input.x, 0, 0); // 2.5D : horizontal seulement
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Rotation vers la direction du mouvement
        if (move.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(move);

        
        // Jump
        
        if (_inputs.Jump && coyoteTimer > 0f)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            coyoteTimer = 0f; // empêcher double jump
        }

        
        // Gravity
        
        velocity.y += gravity * Time.deltaTime;

        // Appliquer la gravité
        controller.Move(velocity * Time.deltaTime);
    }

    void StartDash()
    {
        isDashing = true;
        dashTimer = dashTime;
        cooldownTimer = dashCooldown;

        Vector2 input = _inputs.Move;

        // Dash dans la direction du mouvement ou vers l’avant
        if (input.sqrMagnitude > 0.01f)
            dashDirection = new Vector3(input.x, 0, input.y).normalized;
        else
            dashDirection = transform.forward;
    }

    
    // Gizmo pour visualiser la souris
    
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        if (Camera.main == null) return;

        Vector3 playerPos = transform.position;
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(playerPos).z;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(playerPos, worldPos);
        Gizmos.DrawCube(worldPos, Vector3.one * 0.2f);
    }
}

using System.Collections;
using UnityEngine;

public class Controller : MonoBehaviour
{
    private Inputs _inputs;

    [Header("Déplacement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Saut")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpTime = 0.2f; // temps après avoir quitté le sol pour encore sauter
    private float jumpTimer;

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
    [SerializeField] private LayerMask stairLayer; 
    [SerializeField] private LayerMask playerLayer; 
    private Collider currentStair;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        _inputs = GetComponent<Inputs>();
    }

    void Update()
    {
        if (controller == null || _inputs == null) return;
        
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
            return;
        }

        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }



        if (_groundDetector.touched)
        {
            jumpTimer = jumpTime;
        }
        else
        {
            jumpTimer -= Time.deltaTime;
        }

        if (_groundDetector.stairTouched)
        {
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, _groundDetector.radius, Vector3.down, out hit, _groundDetector.distance, stairLayer))
            {
                currentStair = hit.collider;
            }
        }
        else
        {
            currentStair = null;
        }

        
        if (currentStair != null && Input.GetKeyDown(KeyCode.C))
        {
            currentStair.enabled = false;
            Physics.IgnoreLayerCollision(playerLayer, stairLayer, true);
        }
 
    
            Debug.Log(currentStair);

        
        // Movement
        
        Vector2 input = _inputs.Move;
        Vector3 move = new Vector3(input.x, 0, 0); 
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Rotation vers la direction du mouvement
        if (move.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(move);

        
        // Jump
        
        if (_inputs.Jump && jumpTimer > 0f)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpTimer = 0f; // empêcher double jump
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
    private IEnumerator ReenableStairCollision(Collider stair, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (stair != null)
            Physics.IgnoreCollision(controller, stair, false);
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

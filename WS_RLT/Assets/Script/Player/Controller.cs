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
    [SerializeField] private GroundDetectorUp _groundDetectorUp;
    [SerializeField] private LayerMask stairLayer; 
    [SerializeField] private LayerMask playerLayer; 
    private Collider currentStair;
    private GameObject currentStairObject;
    private GameObject currentStairUpObject;
    private GameObject OldStairObject;
    private GameObject OldStairUpObject;
    [SerializeField] private Collider stair;
    public bool disableStair;
    public bool disableStairUp;

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
                currentStairObject = _groundDetector._collider;
            
        }
        else
        {
            currentStairObject = null;
        }

        
        if (currentStairObject != null && Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Escalier désactivé");
            currentStairObject.GetComponent<Collider>();
            OldStairObject = currentStairObject;
            DisableStairCollision();
            
            
        }
        if (_groundDetectorUp.stairUpTouched)
        {
            currentStairUpObject = _groundDetectorUp._colliderUp;
            
        }
        else
        {
            currentStairUpObject = null;
        }
        if (currentStairUpObject != null )
        {
            Debug.Log("Escalier désactivé");
            currentStairUpObject.GetComponent<Collider>();
            OldStairUpObject = currentStairUpObject;
            DisableStairUpCollision();


        }
        
            Debug.Log(currentStairObject);
    
            

        
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
    

    private void DisableStairCollision()
    {
        if (currentStairObject != null && !disableStair)
        {
             
             disableStair = true;
             OldStairObject = currentStairObject;
             currentStairObject.SetActive(false); 
             
             StartCoroutine(ReenableStairAfterDelay(1f));
        }
       
        
    } 
    private IEnumerator ReenableStairAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (OldStairObject != null)
        {
            OldStairObject.SetActive(true);
        }

        disableStair = false;
        OldStairObject = null;
    }
    private void DisableStairUpCollision()
    {
        if (currentStairUpObject != null && !disableStairUp)
        {
             
            disableStairUp = true;
            OldStairObject = currentStairUpObject;
            currentStairUpObject.SetActive(false); 
             
            StartCoroutine(ReenableStairUpAfterDelay(1f));
        }
       
        
    } 
    private IEnumerator ReenableStairUpAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (OldStairUpObject != null)
        {
            OldStairUpObject.SetActive(true);
        }

        disableStairUp = false;
        OldStairUpObject = null;
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

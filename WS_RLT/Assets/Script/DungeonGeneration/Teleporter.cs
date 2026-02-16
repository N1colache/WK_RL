using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public enum Direction
    {
        Right,
        Left,
        Up,
        Down
    }
    public Direction direction;

    public Animator transitionAnimator;
    
    public LevelLoader levelLoader;
    
    public float transitionDuration = 1f;
    
    [SerializeField] private Vector3 destination;

    [SerializeField] private float delayBeforeTeleport = 1f;

    private float timer = 0f;
    private bool isTeleporting = false;

    public void  Start()
    {
        levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        transitionAnimator = GameObject.FindGameObjectWithTag("Transition")
            .GetComponent<Animator>();
    }
    
    
    

    public void SetDestination(Vector3 dest)
    {
        destination = dest;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // On reset le timer quand le joueur entre
            timer = 0f;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Si déjà en train de TP, on ne fait rien
        if (isTeleporting)
            return;

        timer += Time.deltaTime;

        if (timer >= delayBeforeTeleport)
        {
            StartCoroutine(TeleportWithTransition(other));
            Debug.Log("Teleporting");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Si le joueur sort du trigger → on annule
            timer = 0f;
        }
    }


    IEnumerator TeleportWithTransition(Collider player)
    {
        Debug.Log("Transition lancée");
        
        isTeleporting = true;
        
        transitionAnimator.SetTrigger("Start");
        
        yield return new WaitForSeconds(transitionDuration);
        
        isTeleporting = true;

        Debug.Log("TELEPORT DU JOUEUR vers " + destination);

        CharacterController cc = player.GetComponent<CharacterController>();
        Rigidbody rb = player.GetComponent<Rigidbody>();

        if (cc != null)
        {
            cc.enabled = false;
            player.transform.position = destination;
            cc.enabled = true;
        }
        else if (rb != null)
        {
            rb.isKinematic = true;
            player.transform.position = destination;
            rb.isKinematic = false;
        }
        else
        {
            
            player.transform.position = destination;

        }
        
        yield return new WaitForSeconds(1f);
        transitionAnimator.SetTrigger("End");
        

        // Reset pour pouvoir re-tp plus tard
        timer = 0f;
        isTeleporting = false;
    }
    
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.5f);

        if (destination != Vector3.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, destination);
        }
    }
}

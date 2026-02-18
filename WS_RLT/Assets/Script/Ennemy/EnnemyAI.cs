using UnityEngine;
using UnityEngine.AI;

public class EnnemyAI : MonoBehaviour
{
    private Fire fire;
    public NavMeshAgent agent;
    public Transform player;

    public LayerMask whatsIsGround, whatsIsPlayer;
    
    public enum EnemyWeapon { Pistol, Shotgun, Burst }
    public EnemyWeapon currentWeapon = EnemyWeapon.Pistol;


    // Patrolling
    public Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange;

    // Attacking
    public float timeBetweenAttacks = 1f;
    private bool alreadyAttacked;

    // States
    public float sightRange = 10f;
    public float attackRange = 5f;
    private bool playerInSightRange, playerInAttackRange;

    // Animation
    private Animator animator;
    public bool idle;
    public bool walk;
    public bool run;
    public bool attack;
    public bool _throw;
    
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = GetComponent<NavMeshAgent>();

        // Cherche le Fire sur l’arme (enfant)
        fire = GetComponentInChildren<Fire>();
        //if (fire == null);
        //Debug.LogError("Le composant Fire n'a pas été trouvé sur l'arme !");
        
        animator = GetComponent<Animator>();
    }
    
    private void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            return;
        }

        
        // Vérifie les distances
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatsIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatsIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patrol();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();


        //Animation tests
        if (idle == true)
        {
            animator.SetBool("Patrol", false);
            animator.SetBool("Chase", false);
            animator.SetBool("Attack", false);
        }
        if (walk == true)
        {
            animator.SetBool("Patrol", true);
            animator.SetBool("Chase", false);
            animator.SetBool("Attack", false);
        }
        if (run == true)
        {
            animator.SetBool("Patrol", false);
            animator.SetBool("Chase", true);
            animator.SetBool("Attack", false);
        }
        if (attack == true)
        {
            animator.SetBool("Patrol", false);
            animator.SetBool("Chase", false);
            animator.SetBool("Attack", true);
        }

        if (_throw == true)
        {
            animator.SetBool("Patrol", false);
            animator.SetBool("Chase", false);
            animator.SetBool("Attack", false);
            animator.SetBool("Throw", true);
        }

        if (agent.velocity.magnitude > 0.1f)
        {
            if (playerInSightRange)
            {
                animator.SetBool("Chase", true);
            }
            else
            {
                animator.SetBool("Patrol", true);
            }
        }
        else
        {
            animator.SetBool("Patrol", false);
            animator.SetBool("Chase", false);
        }
    }

    private void Patrol()
    {
        if (!walkPointSet) SearchWalkPoint();
        //animator.SetBool("Patrol", true);
        if (walkPointSet)
        {
            // Ne bouge que sur X
            Vector3 destination = new Vector3(walkPoint.x, transform.position.y, transform.position.z);
            agent.SetDestination(destination);
            
        }

        if (Vector3.Distance(transform.position, walkPoint) < 1f)
        {
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z);

        // Vérifie qu’il y a du sol
        if (Physics.Raycast(walkPoint + Vector3.up * 2f, Vector3.down, 4f, whatsIsGround))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        // Ne suit que sur X
        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, transform.position.z);
        agent.SetDestination(targetPos);
    }

    private void AttackPlayer()
    {
        animator.SetBool("Attack", true);
        agent.ResetPath(); // Stop mouvement

        Vector3 lookPos = new Vector3(player.position.x, transform.position.y, transform.position.z);
        transform.LookAt(lookPos);

        if (!alreadyAttacked && fire != null)
        {
            switch (currentWeapon)
            {
                case EnemyWeapon.Pistol:
                    fire.ShootAt(player.position);
                    break;
                case EnemyWeapon.Burst:
                    fire.ShootAt(player.position); // déjà géré en burst dans Fire
                    break;
                case EnemyWeapon.Shotgun:
                    fire.ShootShotgunAt(player.position);
                    break;
            }

            alreadyAttacked = true;
            animator.SetBool("Attack", false);
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }


    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    // Gizmos pour visualiser les zones de vue et d’attaque
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

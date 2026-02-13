using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class EnnemyAI : MonoBehaviour
{
  private Fire fire;
  
  public NavMeshAgent agent;

  public Transform player;
  
  public LayerMask whatsIsGround, whatsIsPlayer;
  
  //Patrolling

  public Vector3 walkpoint;
  bool walkpointSet;
  public float walkpointRange;
  
  //Attacking
  public float timeBetweenAttcks;
  private bool alreadyAttacked;
  
  //States
  public float sightRange, attackRange;
  public bool playerInSightRange, playerInAttackRange;

  private void Awake()
  {
    player = GameObject.FindGameObjectWithTag("Player").transform;
    agent = GetComponent<NavMeshAgent>();
    fire = GetComponent<Fire>();
  }

  private void Update()
  {
    if (player == null) return;

    float distanceX = Mathf.Abs(player.position.x - transform.position.x);

    playerInSightRange = distanceX <= sightRange;
    playerInAttackRange = distanceX <= attackRange;

    if (!playerInSightRange)
      Patroling();

    else if (playerInSightRange && !playerInAttackRange)
      ChasePlayer();

    else if (playerInAttackRange)
      AttackPlayer();

  }

  private void Patroling()
  {
    if (!walkpointSet)
      SearchWalkPoint();

    if (walkpointSet)
      MoveToX(walkpoint.x);

    if (Mathf.Abs(transform.position.x - walkpoint.x) < 0.5f)
      walkpointSet = false;
  }
  

  private void SearchWalkPoint()
  {
    float randomX = Random.Range(-walkpointRange, walkpointRange);
    walkpoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z);
    walkpointSet = true;
  }


  private void ChasePlayer()
  {
    MoveToX(player.position.x);
  }


  private void AttackPlayer()
  {
    // Stop movement
    agent.ResetPath();

    FacePlayer();

    if (!alreadyAttacked)
    {
      fire.ShootAt(player.position);

      alreadyAttacked = true;
      Invoke(nameof(ResetAttack), timeBetweenAttcks);
    }
  }


  private void MoveToX(float targetX)
  {
    Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);
    agent.SetDestination(targetPosition);
  }
  
  private void FacePlayer()
  {
    float direction = Mathf.Sign(player.position.x - transform.position.x);

    if (direction > 0)
      transform.rotation = Quaternion.Euler(0, 90, 0);
    else
      transform.rotation = Quaternion.Euler(0, -90, 0);
  }

  private void ResetAttack()
  {
    alreadyAttacked = false;
  }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolAI : MonoBehaviour
{
    public float minSpeed;
    public float minForce;
    public float forceRange;
    public float patrolRadius;
    public float sightRadius;
    public float attackRadius;
    public List<Transform> patrolPoints;
    public Color pathColor;

    private Transform playerLoc;
    private Rigidbody rb;
    private Enemy movingEnemy; // self but obv can't use self
    private int currentDestIndex;
    private Vector3 currentDest;
    private Color currentColor;
    private short mode;

    public const short WANDER_MODE = 0;
    public const short ATTACK_MODE = 1;
    public const short FOLLOW_MODE = 2;
    public const short DEAD_MODE = 3;

    void Start()
    {
        playerLoc = FindObjectOfType<MainPlayerController>().transform;

        rb = GetComponent<Rigidbody>();
        movingEnemy = GetComponent<Enemy>();
        currentDest = patrolPoints[0].position;
        currentDestIndex = 0;
        currentColor = pathColor;
        mode = WANDER_MODE;
        playerLoc = FindObjectOfType<MainPlayerController>().transform;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = currentColor;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

    // failsafe timer!!!
    void FixedUpdate()
    {
        switch (mode)
        {
            case ATTACK_MODE:
                movingEnemy.Attack(playerLoc.position);
                if ((transform.position - playerLoc.position).magnitude > attackRadius)
                {
                    currentColor = Color.black;
                    switchPatrolMode(FOLLOW_MODE);
                }
                break;
            case FOLLOW_MODE:
                float dist_to_player = Vector3.Distance(transform.position, playerLoc.position);
                movingEnemy.Follow(playerLoc.position);
                if (dist_to_player < attackRadius)
                {
                    switchPatrolMode(ATTACK_MODE);
                    currentColor = Color.white;
                    break;
                }
                else if (dist_to_player > sightRadius)
                {
                    switchPatrolMode(WANDER_MODE);
                    currentColor = pathColor;
                    break;
                }
                break;
            case DEAD_MODE:
                break;
            default:
                if (Vector3.Distance(transform.position, playerLoc.position) < sightRadius)
                {
                    switchPatrolMode(FOLLOW_MODE);
                    currentColor = Color.black;
                }
                else if ((currentDest - transform.position).magnitude < patrolRadius)
                {
                    currentDestIndex = (currentDestIndex + 1) % patrolPoints.Count;
                    currentDest = patrolPoints[currentDestIndex].position;
                }
                else if (Mathf.Abs(rb.velocity.magnitude) < minSpeed)
                {
                    movingEnemy.Wander(currentDest);
                }
                break;
        }
    }

    public void switchPatrolMode(short new_mode)
    {
        movingEnemy.SwitchPatrolMode(mode, new_mode);
        mode = new_mode;
    }
}

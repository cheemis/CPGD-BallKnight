using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolAI : MonoBehaviour
{
    public float min_speed;
    public float min_force;
    public float force_range;
    public float patrol_radius;
    public float sight_radius;
    public float attack_radius;
    public List<Transform> patrol_points;
    public Transform player_loc;
    public Color path_color;
    

    private Rigidbody rb;
    private Enemy moving_enemy; // self but obv can't use self
    private int current_dest_index;
    private Vector3 current_dest;
    private Color current_color;
    private short mode;
    private const short WANDER_MODE = 0;
    private const short ATTACK_MODE = 1;
    private const short FOLLOW_MODE = 2;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moving_enemy = GetComponent<Enemy>();
        current_dest = patrol_points[0].position;
        current_dest_index = 0;
        current_color = path_color;
        mode = WANDER_MODE;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = current_color;
        Gizmos.DrawWireSphere(transform.position, sight_radius);
        Gizmos.DrawWireSphere(transform.position, attack_radius);
    }

    // failsafe timer!!!
    void FixedUpdate()
    {
        switch (mode)
        {
            case ATTACK_MODE:
                if ((transform.position - player_loc.position).magnitude > attack_radius)
                {
                    current_color = Color.black;
                    mode = FOLLOW_MODE;
                }
                break;
            case FOLLOW_MODE:
                float dist_to_player = Vector3.Distance(transform.position, player_loc.position);
                if (dist_to_player < attack_radius)
                {
                    mode = ATTACK_MODE;
                    current_color = Color.white;
                    break;
                }
                else if (dist_to_player > sight_radius)
                {
                    mode = WANDER_MODE;
                    current_color = path_color;
                    break;
                }
                moving_enemy.follow(player_loc.position);
                break;
            default:
                if (Vector3.Distance(transform.position, player_loc.position) < sight_radius)
                {
                    mode = FOLLOW_MODE;
                    current_color = Color.black;
                }
                else if ((current_dest - transform.position).magnitude < patrol_radius)
                {
                    current_dest_index = (current_dest_index + 1) % patrol_points.Count;
                    current_dest = patrol_points[current_dest_index].position;
                }
                else if (Mathf.Abs(rb.velocity.magnitude) < min_speed)
                {
                    moving_enemy.wander(current_dest);
                }
                break;
        }
    }
}

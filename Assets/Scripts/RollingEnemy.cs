using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingEnemy : MonoBehaviour, Enemy
{
    public float max_follow_speed;
    public float min_follow_force;
    public float follow_force_range;
    public float min_wander_speed;
    public float min_wander_force;
    public float wander_force_range;

    private Rigidbody rb;
    private float random_range;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        random_range = GetComponent<PatrolAI>().patrol_radius / (float) 2;
    }
    public void attack()
    {

    }

    public void follow(Vector3 player_position)
    {
        if (Mathf.Abs(rb.velocity.magnitude) < max_follow_speed)
        {
            rb.AddForce(Vector3.Normalize(player_position - rb.position) * (min_follow_force + Random.value * follow_force_range));
        }
    }

    public void wander(Vector3 current_dest)
    {
        if (Mathf.Abs(rb.velocity.magnitude) < min_wander_speed)
        {
            // launch to point within patrol sphere
            Vector3 destinationPoint = current_dest + (Random.insideUnitSphere * random_range);
            rb.AddForce(Vector3.Normalize(destinationPoint - rb.position) * (min_wander_force + Random.value * wander_force_range));
        }
    }
}

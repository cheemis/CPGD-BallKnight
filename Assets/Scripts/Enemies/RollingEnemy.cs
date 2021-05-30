using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingEnemy : MonoBehaviour, Enemy
{
    public float max_follow_speed = 5;
    public float min_follow_force = 30;
    public float follow_force_range = 20;
    public float max_repel_speed = 3;
    public float repel_force = 30;
    public float min_wander_speed = 4;
    public float min_wander_force = 300;
    public float wander_force_range = 200;
    public float attack_cooldown = 1.2f;
    public float charge_speed = 5.0f;
    public float attack_power = 50f;
    public GameObject arrow_prefab;
    public float pointer_offset;

    private Rigidbody rb;
    private float random_range;
    private float attack_timer;
    private bool charging = false;
    private float launch_power = 0;
    private bool launched = false;
    private GameObject pointer;
    private Vector3 arrow_scale;
    [SerializeField]
    private int maxHealth = 20;
    private int health;
    private float HIT_THRESHOLD = 10f;
    private Vector3 lastFrameVel;
    [SerializeField]
    private MeshRenderer healthBarRenderer;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        random_range = GetComponent<PatrolAI>().patrol_radius / (float) 2;
        attack_timer = 0;
        pointer = Instantiate(arrow_prefab, transform.position, Quaternion.identity);
        arrow_scale = pointer.transform.localScale;
        lastFrameVel = rb.velocity;
        health = maxHealth;
        GameObject.Destroy(pointer);
    }
    public void Attack(Vector3 player_position)
    {
        if (charging)
        {
            launch_power += charge_speed;
            Vector3 enemy_to_player = player_position - transform.position;
            Vector3 xz_direction = new Vector3(enemy_to_player.x, 0, enemy_to_player.z);
            Quaternion rotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), xz_direction);
            float power_scale = launch_power / attack_power;
            pointer.transform.localScale = new Vector3( power_scale * arrow_scale.x,
                power_scale * arrow_scale.y, power_scale * arrow_scale.z);
            pointer.transform.eulerAngles = rotation.eulerAngles;
            pointer.transform.position = transform.position + rotation * new Vector3(power_scale * pointer_offset, 0, 0);
            pointer.GetComponent<Renderer>().material.SetFloat("Power_proportion", power_scale);
            if (launch_power >= attack_power)
            {
                rb.AddForce(Vector3.Normalize(player_position - rb.position) * attack_power, ForceMode.Impulse);
                charging = false;
                attack_timer = attack_cooldown;
                launched = true;
                GameObject.Destroy(pointer);
            }
        }
        else if (attack_timer < 0)
        {
            charging = true;
            launch_power = 0;
            Vector3 enemy_to_player = player_position - transform.position;
            Vector3 xz_direction = new Vector3(enemy_to_player.x, 0, enemy_to_player.z);
            Quaternion rotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), xz_direction);
            pointer = Instantiate(arrow_prefab,
                transform.position + rotation * (Vector3.right * 0.01f * pointer_offset), rotation);
            pointer.transform.localScale = new Vector3(0.3f * arrow_scale.x, arrow_scale.y, 0.3f * arrow_scale.z);
        }
        else if (!launched && Mathf.Abs(rb.velocity.magnitude) < max_repel_speed)
        {
            rb.AddForce(Vector3.Normalize(player_position - rb.position) * -repel_force);
            attack_timer = Mathf.Max(attack_timer - Time.deltaTime, -1.0f);
        }
        else if (Mathf.Abs(rb.velocity.magnitude) < max_repel_speed)
        {
            launched = false;
        }

    }

    public void Follow(Vector3 player_position)
    {
        if (Mathf.Abs(rb.velocity.magnitude) < max_follow_speed)
        {
            rb.AddForce(Vector3.Normalize(player_position - rb.position) * (min_follow_force + Random.value * follow_force_range));
        }
        attack_timer = Mathf.Max(attack_timer - Time.deltaTime, -1.0f);
    }

    public void Wander(Vector3 current_dest)
    {
        if (Mathf.Abs(rb.velocity.magnitude) < min_wander_speed)
        {
            // launch to point within patrol sphere
            Vector3 destinationPoint = current_dest + (Random.insideUnitSphere * random_range);
            rb.AddForce(Vector3.Normalize(destinationPoint - rb.position) * (min_wander_force + Random.value * wander_force_range));
        }
        attack_timer = Mathf.Max(attack_timer - Time.deltaTime, -1.0f);
    }

    public void SwitchPatrolMode(short old_mode, short new_mode)
    {
        if (old_mode == PatrolAI.ATTACK_MODE && charging)
        {
            GameObject.Destroy(pointer);
            charging = false;
        }
    }

    public float OnCollision(Vector3 other_position, Vector3 other_velocity)
    {
        Vector3 center_to_center = transform.position - other_position;
        Vector3 projected_enemy_vel =  Vector3.Project(lastFrameVel, center_to_center),
            projected_other_vel = Vector3.Project(other_velocity, center_to_center);
        float vel_difference = projected_enemy_vel.magnitude - projected_other_vel.magnitude;
        Debug.Log($"{projected_other_vel.magnitude} {projected_enemy_vel.magnitude} vel_difference: {vel_difference}");
        if (vel_difference < HIT_THRESHOLD)
        {
            health -= 1;
            return vel_difference;
        }
        else if (vel_difference > HIT_THRESHOLD)
        {
            return vel_difference;
        }
        return -1.0f;
    }

    public void LateUpdate()
    {
        lastFrameVel = rb.velocity;
        healthBarRenderer.material.SetFloat("PercentHealth", 0/(float)maxHealth);
        healthBarRenderer.material.SetFloat("PercentHealth", Mathf.Abs(Mathf.Sin(Time.time)));
    }
}
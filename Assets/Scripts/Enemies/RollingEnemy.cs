using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingEnemy : MonoBehaviour, Enemy
{
    public float maxFollowSpeed = 5;
    public float minFollowForce = 30;
    public float followForceRange = 20;
    public float maxRepelSpeed = 3;
    public float repelForce = 30;
    public float minWanderSpeed = 4;
    public float minWanderForce = 300;
    public float wanderForceRange = 200;
    public float attackCooldown = 1.2f;
    public float chargeSpeed = 5.0f;
    public float attackPower = 50f;
    public GameObject arrowPrefab;
    public float pointerOffset;

    private Rigidbody rb;
    private float randomRange;
    private float attackTimer;
    private bool charging = false;
    private float launchPower = 0;
    private bool launched = false;
    private GameObject pointer;
    private Vector3 arrowScale;
    [SerializeField]
    private int maxHealth = 20;
    private int health;
    private float HIT_THRESHOLD = 2f;
    private Vector3 lastFrameVel;
    [SerializeField]
    private GameObject ExplosionEffect;
    [SerializeField]
    private MeshRenderer healthBarRenderer;
    [SerializeField]
    private Material dissolveMaterial;
    [SerializeField]
    private float deathTimerLength;
    private float deathTimer;

    public const float DAMAGE_FACTOR = .333f;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        randomRange = GetComponent<PatrolAI>().patrolRadius / (float) 2;
        attackTimer = 0;
        pointer = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        arrowScale = pointer.transform.localScale;
        lastFrameVel = rb.velocity;
        health = maxHealth;
        Destroy(pointer);
    }

    public void Update()
    {
        if (health < 1)
        {
            deathTimer -= Time.deltaTime;
            transform.localScale = new Vector3(1, 1, 1) * deathTimer / deathTimerLength;
            if (deathTimer < 0)
            {
                GameObject explosion = Instantiate(ExplosionEffect, transform.position, transform.rotation);
                Destroy(transform.parent.gameObject);
            }
        }
    }

    public void Attack(Vector3 playerPosition)
    {
        if (charging)
        {
            launchPower += chargeSpeed;
            Vector3 enemyToPlayer = playerPosition - transform.position;
            Vector3 directionXZ = new Vector3(enemyToPlayer.x, 0, enemyToPlayer.z);
            Quaternion rotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), directionXZ);
            float powerScale = launchPower / attackPower;
            pointer.transform.localScale = new Vector3( powerScale * arrowScale.x,
                powerScale * arrowScale.y, powerScale * arrowScale.z);
            pointer.transform.eulerAngles = rotation.eulerAngles;
            pointer.transform.position = transform.position + rotation * new Vector3(powerScale * pointerOffset, 0, 0);
            pointer.GetComponent<Renderer>().material.SetFloat("Power_proportion", powerScale);
            if (launchPower >= attackPower)
            {
                rb.AddForce(Vector3.Normalize(playerPosition - rb.position) * attackPower, ForceMode.Impulse);
                charging = false;
                attackTimer = attackCooldown;
                launched = true;
                Destroy(pointer);
                pointer = null;
            }
        }
        else if (attackTimer < 0)
        {
            charging = true;
            launchPower = 0;
            Vector3 enemyToPlayer = playerPosition - transform.position;
            Vector3 directionXZ = new Vector3(enemyToPlayer.x, 0, enemyToPlayer.z);
            Quaternion rotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), directionXZ);
            pointer = Instantiate(arrowPrefab,
                transform.position + rotation * (Vector3.right * 0.01f * pointerOffset), rotation);
            pointer.transform.localScale = new Vector3(0.3f * arrowScale.x, arrowScale.y, 0.3f * arrowScale.z);
        }
        else if (!launched && Mathf.Abs(rb.velocity.magnitude) < maxRepelSpeed)
        {
            rb.AddForce(Vector3.Normalize(playerPosition - rb.position) * -repelForce);
            attackTimer = Mathf.Max(attackTimer - Time.deltaTime, -1.0f);
        }
        else if (Mathf.Abs(rb.velocity.magnitude) < maxRepelSpeed)
        {
            launched = false;
        }

    }

    public void Follow(Vector3 player_position)
    {
        if (Mathf.Abs(rb.velocity.magnitude) < maxFollowSpeed)
        {
            rb.AddForce(Vector3.Normalize(player_position - rb.position) * (minFollowForce + Random.value * followForceRange));
        }
        attackTimer = Mathf.Max(attackTimer - Time.deltaTime, -1.0f);
    }

    public void Wander(Vector3 current_dest)
    {
        if (Mathf.Abs(rb.velocity.magnitude) < minWanderSpeed)
        {
            // launch to point within patrol sphere
            Vector3 destinationPoint = current_dest + (Random.insideUnitSphere * randomRange);
            rb.AddForce(Vector3.Normalize(destinationPoint - rb.position) * (minWanderForce + Random.value * wanderForceRange));
        }
        attackTimer = Mathf.Max(attackTimer - Time.deltaTime, -1.0f);
    }

    public void SwitchPatrolMode(short old_mode, short new_mode)
    {
        if (old_mode == PatrolAI.ATTACK_MODE && charging)
        {
            Destroy(pointer);
            charging = false;
        }
    }

    public float OnCollision(Vector3 other_position, Vector3 other_velocity)
    {
        Vector3 centerToCenter = transform.position - other_position;
        Vector3 projectedEnemyVel =  Vector3.Project(lastFrameVel, centerToCenter),
            projectedOtherVel = Vector3.Project(other_velocity, centerToCenter);
        float velDifference = projectedEnemyVel.magnitude - projectedOtherVel.magnitude;
        if (velDifference < -HIT_THRESHOLD)
        {
            takeDamage(-(int) Mathf.Floor(velDifference * DAMAGE_FACTOR));
            return velDifference;
        }
        else if (velDifference > HIT_THRESHOLD)
        {
            return velDifference;
        }
        return -1.0f;
    }

    public void takeDamage(int damage)
    {
        health -= damage;
        health -= maxHealth;
        if (health < 1)
        {
            Renderer renderer = GetComponent<Renderer>();
            renderer.material = dissolveMaterial;
            renderer.material.SetFloat("StartTime", Time.time);
            deathTimer = deathTimerLength;
            if (pointer)
            {
                Destroy(pointer);
                pointer = null;
            }
            if (healthBarRenderer)
            {
                Destroy(healthBarRenderer);
                healthBarRenderer = null;
            }
            GetComponent<PatrolAI>().switchPatrolMode(PatrolAI.DEAD_MODE);
        }
    }

    public void LateUpdate()
    {
        lastFrameVel = rb.velocity;
        if (healthBarRenderer)
        {
            healthBarRenderer.material.SetFloat("PercentHealth", health / (float)maxHealth);
        }
    }
}

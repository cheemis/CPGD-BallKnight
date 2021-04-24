using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BrokenNavmeshPatrol : MonoBehaviour
{
    public Transform[] points;
    public float dest_margin_of_error;
    public float min_speed;
    public float max_force;

    private int pathPoint = 0;
    private int destPoint = 0;
    private Rigidbody rb;
    private NavMeshPath path;
    private Vector3 destination;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        path = new NavMeshPath();
        getPath();
    }

    void getPath()
    {
        if (points.Length == 0)
            return;

        destination = points[destPoint].position;
        NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
        for (int i = 0; i < path.corners.Length - 1; i++)
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
    }

    void launchToPoint()
    {
        Vector3 dir = path.corners[pathPoint] - rb.position;
        float dist = dir.magnitude;
        rb.AddForce(dir * 10);
        Vector3.Normalize(dir);
        dir = dir * (dist > max_force ? max_force : dist);
    }

    // failsafe timer!!!
    void FixedUpdate()
    {
        if (Vector3.Distance(rb.position, destination) < 0.5f)
        {
            pathPoint = 0;
            destPoint = (destPoint + 1) % points.Length;
            Debug.Log("Change Dest");
            getPath();
        }
        else if (Vector3.Distance(rb.position, path.corners[pathPoint]) < 0.5f)
        {
            pathPoint = (pathPoint + 1) % path.corners.Length;
            Debug.Log("Change Path");
        }
        else if (Mathf.Abs(rb.velocity.magnitude - min_speed) < 2f)
        {
            Debug.Log("launching");
            launchToPoint();
        }
    }

}

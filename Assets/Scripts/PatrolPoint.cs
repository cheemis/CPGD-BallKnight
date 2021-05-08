using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoint : MonoBehaviour
{
    public PatrolAI ai;
    // Update is called once per frame
    void OnDrawGizmos()
    {
        Gizmos.color = ai.path_color;
        Gizmos.DrawWireSphere(transform.position, ai.patrol_radius);
    }
}

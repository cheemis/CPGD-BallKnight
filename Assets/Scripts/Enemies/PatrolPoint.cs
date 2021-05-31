using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoint : MonoBehaviour
{
    public PatrolAI ai;
    // Update is called once per frame
    void OnDrawGizmos()
    {
        Gizmos.color = ai.pathColor;
        Gizmos.DrawWireSphere(transform.position, ai.patrolRadius);
    }
}

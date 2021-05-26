using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Transform camera;
    public Transform objectToFollow;

    [SerializeField]
    private Vector3 offset;

    // Update is called once per frame
    void Update()
    {
        transform.position = objectToFollow.position + offset;
        transform.LookAt(2 * transform.position - camera.position);
    }
}

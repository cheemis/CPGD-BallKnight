using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Transform objectToFollow;
    private Transform camera;

    [SerializeField]
    private Vector3 offset;

    private void Start()
    {
        camera = Camera.main.transform;
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = objectToFollow.position + offset;
        transform.eulerAngles = camera.eulerAngles;
    }
}

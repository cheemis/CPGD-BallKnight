using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Transform objectToFollow;

    [SerializeField]
    private Vector3 offset;

    private Transform camera;

    private void Start() {
        camera = Camera.main.transform;

    }

	// Update is called once per frame
	void Update()
    {
        transform.position = objectToFollow.position + offset;
        transform.eulerAngles = camera.eulerAngles;
    }
}

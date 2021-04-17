using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class MainPlayerController : MonoBehaviour {
    public float basicMoveForce = 10.0f;

    private Rigidbody rb;

    private Camera mainCam;
    private Transform cameraTransform;

    private Vector3 initialPos;

    private const float yPosResetCutoff = -5.0f;

    private bool sceneIsLoading = false;

    void Start() {
        rb = GetComponent<Rigidbody>();

        initialPos = transform.position;

        mainCam = Camera.main;
        cameraTransform = mainCam.transform;
    }

	private void Update() {
		if (transform.position.y < yPosResetCutoff) {
            transform.position = initialPos;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
		}

        if (!sceneIsLoading && Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            sceneIsLoading = true;
		}
	}

	void FixedUpdate() {
        Vector3 targetDirection = GetTargetDirection();

        rb.AddForce(targetDirection * basicMoveForce);
    }

    private Vector3 GetTargetDirection() {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0.0f;
        camForward = camForward.normalized;
        camRight.y = 0.0f;
        camRight = camRight.normalized;

        return (input.y * camForward + input.x * camRight).normalized;
    }
}

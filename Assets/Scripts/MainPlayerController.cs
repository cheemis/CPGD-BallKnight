using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class MainPlayerController : MonoBehaviour {
    public float basicMoveForce = 10.0f;
    public float initialJumpLaunchForce = 100.0f;
    public float additionalHoldJumpForce = 200.0f;
    public float maxAdditionalHoldJumpTime = 1.0f;
    public float groundCheckDist = 1.0f;
    public LayerMask groundLayerMask;
    [Range(0.0f, 1.0f)]
    public float groundSphereCastRadiusRatio;

    private Rigidbody rb;
    private SphereCollider sphereColl;

    private Camera mainCam;
    private Transform cameraTransform;

    private Vector3 initialPos;

    private bool isJumping = false;
    private bool isGrounded = false;
    private bool waitingForFallToGroundCheck = false;
    private float jumpActiveTimer = 0.0f;

    private bool sceneIsLoading = false;
    private const float yPosResetCutoff = -5.0f;

    private Vector3 lastFrameVel;
    void Start() {
        rb = GetComponent<Rigidbody>();
        lastFrameVel = rb.velocity;

        sphereColl = GetComponent<SphereCollider>();

        initialPos = transform.position;

        mainCam = Camera.main;
        cameraTransform = mainCam.transform;
    }

	private void Update() {
        if (!waitingForFallToGroundCheck) {
            CheckForGround();
        }
        else {
            if (rb.velocity.y < 0.0f) {
                waitingForFallToGroundCheck = false;
                isJumping = false;
            }
		}

        if (isGrounded) {
            if (Input.GetButtonDown("Jump")) {
                rb.AddForce(Vector3.up * initialJumpLaunchForce, ForceMode.Impulse);
                waitingForFallToGroundCheck = true;
                isGrounded = false;

                jumpActiveTimer = 0.0f;
                isJumping = true;
            }
        }
        if (Input.GetButtonUp("Jump")) {
            isJumping = false;
        }

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

        if (isJumping && !isGrounded) {
            if (jumpActiveTimer < maxAdditionalHoldJumpTime) {
                rb.AddForce(Vector3.up * additionalHoldJumpForce);
                jumpActiveTimer += Time.fixedDeltaTime;
            }
            else {
                isJumping = false;
            }
        }
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
    void Die(){
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex));
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }
    IEnumerator LoadLevel(int level){
        //put fancy animations and stuff here
        yield return new WaitForSeconds(1f);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(level);
        while (!asyncLoad.isDone){
            yield return null;
        }
        //Next scene is done loading
    }

    private void CheckForGround() {
        float groundSphereCastRadius = sphereColl.bounds.extents.y * groundSphereCastRadiusRatio;
        float groundSphereCastMaxDist = sphereColl.bounds.extents.y + groundCheckDist;

        RaycastHit hit;
        isGrounded = Physics.SphereCast(transform.position, groundSphereCastRadius, Vector3.down,
            out hit, groundSphereCastMaxDist, groundLayerMask);

        if (isGrounded) {
            Debug.DrawRay(transform.position + Vector3.right * groundSphereCastRadius, Vector3.down * groundSphereCastMaxDist, Color.green);
            Debug.DrawRay(transform.position + Vector3.left * groundSphereCastRadius, Vector3.down * groundSphereCastMaxDist, Color.green);
            Debug.DrawRay(transform.position + Vector3.forward * groundSphereCastRadius, Vector3.down * groundSphereCastMaxDist, Color.green);
            Debug.DrawRay(transform.position + Vector3.back * groundSphereCastRadius, Vector3.down * groundSphereCastMaxDist, Color.green);
        }
        else {
            Debug.DrawRay(transform.position + Vector3.right * groundSphereCastRadius, Vector3.down * groundSphereCastMaxDist, Color.red);
            Debug.DrawRay(transform.position + Vector3.left * groundSphereCastRadius, Vector3.down * groundSphereCastMaxDist, Color.red);
            Debug.DrawRay(transform.position + Vector3.forward * groundSphereCastRadius, Vector3.down * groundSphereCastMaxDist, Color.red);
            Debug.DrawRay(transform.position + Vector3.back * groundSphereCastRadius, Vector3.down * groundSphereCastMaxDist, Color.red);
        }
	}

    private void OnCollisionEnter(Collision collision)
    {
        Enemy enemy_component = collision.gameObject.GetComponent<Enemy>();
        if (enemy_component != null)
        {
            float vel_difference = enemy_component.OnCollision(transform.position, lastFrameVel);
        }
    }

    public void LateUpdate()
    {
        lastFrameVel = rb.velocity;
    }
}

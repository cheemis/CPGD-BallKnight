using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class MainPlayerController : MonoBehaviour {
    public float basicMoveForce = 100.0f;
    public float dashLaunchForce = 1000.0f;
    public LineRenderer arrowLineRenderer;

    private Rigidbody rb;

    private Camera mainCam;
    private Transform cameraTransform;

    private Vector3 mouseAimDir = Vector3.zero;
    void Start() {
        rb = GetComponent<Rigidbody>();
        mainCam = Camera.main;
        cameraTransform = mainCam.transform;
    }

	private void Update() {
        GetMouseAim();

        if (Input.GetMouseButtonDown(0)) {
            SetArrowRendererPositions();
        }
        if (Input.GetMouseButton(0)) {
            SetArrowRendererPositions();
        }
        if (Input.GetMouseButtonUp(0)) {
            arrowLineRenderer.enabled = false;
            rb.AddForce(mouseAimDir * dashLaunchForce, ForceMode.Impulse);
        }
    }

	void FixedUpdate() {
        Vector3 targetDirection = GetTargetDirection();

        rb.AddForce(targetDirection * basicMoveForce);
    }

    private void SetArrowRendererPositions() {
        if (mouseAimDir.magnitude <= 0.0f) {
            arrowLineRenderer.enabled = false;
            return;
		}
        arrowLineRenderer.enabled = true;

        Vector3[] rendererPositions = new Vector3[] {
            transform.position,
            transform.position + (mouseAimDir * 5.0f)
        };

        arrowLineRenderer.positionCount = 2;
        arrowLineRenderer.SetPositions(rendererPositions);
    }

    private void GetMouseAim() {
        Ray mouseRay = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(mouseRay, out hit)) {
            Debug.DrawRay(mouseRay.origin, mouseRay.direction * hit.distance, Color.green);

            if (Vector3.Distance(hit.point, transform.position) <= 1.0f) {
                mouseAimDir = Vector3.zero;
            }
            else {
                Vector3 aimHitPoint = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                mouseAimDir = (aimHitPoint - transform.position).normalized;
            }
        }
        else {
            Debug.DrawRay(mouseRay.origin, mouseRay.direction * 1000.0f, Color.red);
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
}

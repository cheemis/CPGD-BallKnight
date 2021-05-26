using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowLaunchHandler : MonoBehaviour {
    public float maxDashLaunchForce = 20.0f;
    public LineRenderer arrowLineRenderer;
    public float launchArrowGrowRate = 0.2f;
    public LayerMask aimPlaneLayerMask;

    private Rigidbody rb;

    private ChargeMetersHandler chargeMetersHandler;

    private Camera mainCam;

    private Vector3 mouseAimDir = Vector3.forward;

    private bool launchArrowActive = false;
    private float currentLaunchArrowRatio = 0.0f;

    private const int arrowRendererPointCount = 100;
    void Start() {
        chargeMetersHandler = FindObjectOfType<ChargeMetersHandler>();

        rb = GetComponent<Rigidbody>();

        mainCam = Camera.main;
    }

    private void Update() {
        GetMouseAim();

        if (chargeMetersHandler.CheckIfMinimumMagicValueReached()) {
            if (Input.GetMouseButtonDown(0)) {
                launchArrowActive = true;
                currentLaunchArrowRatio = 0.1f;

                SetArrowRendererPositions();
            }
            if (launchArrowActive) {
                if (Input.GetMouseButton(0)) {
                    currentLaunchArrowRatio += Time.deltaTime * launchArrowGrowRate;
                    currentLaunchArrowRatio = Mathf.Clamp(currentLaunchArrowRatio, 0.0f, 1.0f);

                    SetArrowRendererPositions();
                }
                if (Input.GetMouseButtonUp(0)) {
                    arrowLineRenderer.enabled = false;
                    Vector3 forceVector = mouseAimDir * maxDashLaunchForce
                        * currentLaunchArrowRatio * chargeMetersHandler.GetAdjustedMagicMeterValue();
                    rb.AddForce(forceVector, ForceMode.Impulse);

                    chargeMetersHandler.DrainMagicMeter();

                    currentLaunchArrowRatio = 0.0f;
                }

                if (Input.GetMouseButtonDown(1)) {
                    arrowLineRenderer.enabled = false;
                    currentLaunchArrowRatio = 0.0f;
                    launchArrowActive = false;
                }
            }
        }
        else if (launchArrowActive) {
            arrowLineRenderer.enabled = false;
            currentLaunchArrowRatio = 0.0f;
            launchArrowActive = false;
        }
    }

    private void SetArrowRendererPositions() {
        if (mouseAimDir.magnitude <= 0.0f) {
            arrowLineRenderer.enabled = false;
            return;
        }
        arrowLineRenderer.enabled = true;

        Vector3 arrowStartPos = transform.position;
        Vector3 arrowEndPos = Vector3.Lerp(transform.position, transform.position + (mouseAimDir * 5.0f), currentLaunchArrowRatio);

        Vector3[] rendererPositions = new Vector3[arrowRendererPointCount];

        for (int i = 0; i < arrowRendererPointCount - 1; i++) {
            rendererPositions[i] = Vector3.Lerp(arrowStartPos, arrowEndPos, (float)i / (arrowRendererPointCount - 1));
        }
        rendererPositions[arrowRendererPointCount - 1] = arrowEndPos;

        arrowLineRenderer.positionCount = arrowRendererPointCount;
        arrowLineRenderer.SetPositions(rendererPositions);
    }

    private void GetMouseAim() {
        Ray mouseRay = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(mouseRay, out hit, Mathf.Infinity, aimPlaneLayerMask)) {
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
}

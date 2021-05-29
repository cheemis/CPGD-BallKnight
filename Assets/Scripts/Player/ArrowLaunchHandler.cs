using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowLaunchHandler : MonoBehaviour {
    public float maxDashLaunchForce = 20.0f;
    public LineRenderer arrowLineRenderer;
    public float launchArrowGrowRate = 0.2f;
    public LayerMask aimPlaneLayerMask;
    [ColorUsage(true, true)]
    public Color[] playerBallColors;
    public Vector2 minMaxAttackSpeedCutoffValues = new Vector2(8.0f, 12.0f);
    public ParticleSystem attackParticleSystem;
    public AudioClip launchAudioClip;

    private ParticleSystem.EmissionModule attackParticleSystemEmissionModule;

    private MainPlayerController mainPlayerController;

    private Material mainBallMaterial;
    private SphereCollider sphereColl;
    private Rigidbody rb;

    private ChargeMetersHandler chargeMetersHandler;

    private Camera mainCam;

    private Vector3 mouseAimDir = Vector3.forward;

    private bool launchArrowActive = false;
    private float currentLaunchArrowRatio = 0.0f;

    private Tween attackColorBlendTween = null;
    private bool attackIsActive = false;

    private const int arrowRendererPointCount = 100;

    private Vector2 particleEmissionRange = new Vector2(1.0f, 20.0f);
    void Start() {
        attackParticleSystemEmissionModule = attackParticleSystem.emission;

        Renderer mainBallRenderer = GetComponent<Renderer>();
        mainBallMaterial = new Material(mainBallRenderer.material);
        mainBallMaterial.EnableKeyword("_EMISSION");
        mainBallMaterial.SetColor("_EmissionColor", playerBallColors[0]);

        mainBallRenderer.material = mainBallMaterial;

        sphereColl = GetComponent<SphereCollider>();

        mainPlayerController = GetComponent<MainPlayerController>();

        chargeMetersHandler = FindObjectOfType<ChargeMetersHandler>();

        rb = GetComponent<Rigidbody>();

        mainCam = Camera.main;
    }

    private void Update() {
        if (mainPlayerController.CheckIfInputEnabled()) {
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
                        LaunchFromLaunchArrow();
                    }

                    if (Input.GetMouseButtonDown(1)) {
                        DisableLaunchArrowComponents();
                    }
                }
            }
            else if (launchArrowActive) {
                DisableLaunchArrowComponents();
            }
        }
    }

	private void FixedUpdate() {
        if (mainPlayerController.CheckIfInputEnabled()) {
            HandleAttackMode();
        }
    }

    private void HandleAttackMode() {
        float velocityMagnitude = rb.velocity.magnitude;

        if (velocityMagnitude >= minMaxAttackSpeedCutoffValues.x) {
            velocityMagnitude = Mathf.Clamp(velocityMagnitude, 
                minMaxAttackSpeedCutoffValues.x, minMaxAttackSpeedCutoffValues.y);

            float interpRatio = (minMaxAttackSpeedCutoffValues.y - velocityMagnitude)
                / (minMaxAttackSpeedCutoffValues.y - minMaxAttackSpeedCutoffValues.x);
            interpRatio = Mathf.Clamp(interpRatio, 0.0f, 1.0f);

            Color currentPlayerColor = Color.Lerp(playerBallColors[1], playerBallColors[0], interpRatio);
            mainBallMaterial.SetColor("_EmissionColor", currentPlayerColor);

            attackParticleSystemEmissionModule.rateOverTime = (int)Mathf.Lerp(particleEmissionRange.y, particleEmissionRange.x, interpRatio);

            if (!attackParticleSystem.isEmitting) {
                attackParticleSystem.Play();
            }

            attackIsActive = true;
        }
        else if (attackIsActive) {
            attackParticleSystem.Stop();

            mainBallMaterial.SetColor("_EmissionColor", playerBallColors[0]);

            attackIsActive = false;
        }
	}

    private void DisableLaunchArrowComponents() {
        arrowLineRenderer.enabled = false;
        currentLaunchArrowRatio = 0.0f;
        launchArrowActive = false;
    }

	private void LaunchFromLaunchArrow() {
        GeneralAudioPool.Instance.PlaySound(launchAudioClip, 0.25f, Random.Range(0.9f, 1.1f));

        arrowLineRenderer.enabled = false;
        Vector3 forceVector = mouseAimDir * maxDashLaunchForce
            * currentLaunchArrowRatio * chargeMetersHandler.GetAdjustedMagicMeterValue();
        rb.AddForce(forceVector, ForceMode.Impulse);

        chargeMetersHandler.DrainMagicMeter();

        currentLaunchArrowRatio = 0.0f;
    }

    private void SetArrowRendererPositions() {
        if (mouseAimDir.magnitude <= 0.0f) {
            arrowLineRenderer.enabled = false;
            return;
        }
        arrowLineRenderer.enabled = true;

        Vector3 arrowStartPos = transform.position + mouseAimDir * sphereColl.bounds.extents.x;
        Vector3 arrowEndPos = Vector3.Lerp(arrowStartPos, arrowStartPos + (mouseAimDir * 5.0f), currentLaunchArrowRatio);

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

    public void OnInputDisabled() {
        DisableLaunchArrowComponents();
    }

    private void ExitAttackMode() {
        if (attackColorBlendTween != null && attackColorBlendTween.IsActive()) {
            attackColorBlendTween.Kill();
        }

        mainBallMaterial.SetColor("_EmissionColor", playerBallColors[0]);

        attackIsActive = false;
    }

    private void EnterAttackMode() {
        attackIsActive = true;

        /*mainBallMaterial.SetColor("_EmissionColor", playerBallColors[1]);

		attackColorBlendTween = DOTween.To(
			() => mainBallMaterial.GetColor("_EmissionColor"),
			x => mainBallMaterial.SetColor("_EmissionColor", x),
			playerBallColors[0],
			1.0f
		).SetEase(Ease.InOutSine)
		.OnComplete(ExitAttackMode);*/
	}
}

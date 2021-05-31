using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ChargeMetersHandler : MonoBehaviour {
    public float magicMeterGrowRate = 0.15f;
    public Image magicMeterImage;
    [ColorUsage(true, true)]
    public Color[] magicMeterFillColors;

    private Material magicMeterMaterial;

    private Tween drainMagicMeterTween;

    private float magicMeterValue = 0.0f;

    public bool isDrainingMagicMeter = false;

    private const float maxMagicMeterValue = 0.7f;
    private const float minUsableMagicMeterValue = 0.35f;

    void Start() {
        magicMeterMaterial = new Material(magicMeterImage.material);
        magicMeterImage.material = magicMeterMaterial;
    }

    void Update() {
        IncreaseMagicMeter();
    }

    public bool CheckIfMinimumMagicValueReached() {
        return !isDrainingMagicMeter && (magicMeterValue >= minUsableMagicMeterValue);
    }

    public float GetAdjustedMagicMeterValue() {
        return remap(magicMeterValue, 0.0f, maxMagicMeterValue, 0.0f, 1.0f);
	}

    public float remap(float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public void DetermineMagicMeterColor() {
        if (magicMeterValue < minUsableMagicMeterValue) {
            magicMeterMaterial.SetColor("_FillColor", magicMeterFillColors[0]);
        }
        else {
            Color magicMeterFillColor = Color.Lerp(magicMeterFillColors[1], magicMeterFillColors[2],
                remap(magicMeterValue, minUsableMagicMeterValue, maxMagicMeterValue, 0.0f, 1.0f));
            magicMeterMaterial.SetColor("_FillColor", magicMeterFillColor);
        }
    }

    public void IncreaseMagicMeter() {
        if (!isDrainingMagicMeter && magicMeterValue < maxMagicMeterValue) {
            magicMeterValue += Time.deltaTime * magicMeterGrowRate;
            magicMeterValue = Mathf.Clamp(magicMeterValue, 0.0f, maxMagicMeterValue);

            magicMeterMaterial.SetFloat("_CutoffPos", magicMeterValue);

            DetermineMagicMeterColor();
        }
    }

    public void DrainMagicMeter() {
        isDrainingMagicMeter = true;

        if (drainMagicMeterTween != null && drainMagicMeterTween.IsActive()) {
            drainMagicMeterTween.Kill();
        }

        DetermineMagicMeterColor();

        drainMagicMeterTween = DOTween.To(
            () => magicMeterMaterial.GetFloat("_CutoffPos"),
            x => {
                magicMeterValue = x;
                magicMeterMaterial.SetFloat("_CutoffPos", x);
            },
            0.0f,
            1.0f
        ).SetEase(Ease.InOutSine)
        .OnComplete(() => {
            isDrainingMagicMeter = false;
            magicMeterValue = 0.0f;
            magicMeterMaterial.SetFloat("_CutoffPos", 0.0f);
        });
    }
}

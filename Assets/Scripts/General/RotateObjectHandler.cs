using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RotateObjectHandler : MonoBehaviour {
	public float rotationTime = 2.0f;
	public bool usesRange = false;
	public Vector2 rotationRange = new Vector2(0.0f, 360.0f);
	public Ease tweenEaseType = Ease.Linear;

	void Start() {
		if (!usesRange) {
			transform.DOLocalRotate(Vector3.up * 360.0f, rotationTime, RotateMode.WorldAxisAdd).SetEase(tweenEaseType).SetLoops(-1);
		}
		else {
			float rotationAmount = Mathf.Abs(rotationRange.y - rotationRange.x);

			if (transform.localEulerAngles.z == rotationRange.x) {
				transform.DOLocalRotate(Vector3.up * rotationAmount, rotationTime, RotateMode.WorldAxisAdd).SetEase(tweenEaseType).SetLoops(-1, LoopType.Yoyo);
			}
			else {
				StartCoroutine(RotateIntoPosBeforeLoopCo());
			}
		}
    }

	private IEnumerator RotateIntoPosBeforeLoopCo() {
		float rotationAmount = Mathf.Abs(rotationRange.y - rotationRange.x);
		float rotationAmountFromStartingPos = Mathf.Abs(rotationRange.y - transform.localEulerAngles.z);

		float timeToFirstRotation = rotationTime * (rotationAmountFromStartingPos / rotationAmount);

		Tween initialPosTween =
			transform.DOLocalRotate(Vector3.up * rotationAmountFromStartingPos, timeToFirstRotation, RotateMode.WorldAxisAdd)
			.SetEase(tweenEaseType);

		while (initialPosTween.IsActive())
			yield return null;

		transform.DOLocalRotate(-Vector3.up * rotationAmount, rotationTime, RotateMode.WorldAxisAdd).SetEase(tweenEaseType).SetLoops(-1, LoopType.Yoyo);
	}
}

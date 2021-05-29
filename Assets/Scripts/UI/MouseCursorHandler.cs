using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursorHandler : MonoBehaviour {
    public Canvas parentCanvas;

    public void Start() {
        Vector2 pos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform, Input.mousePosition,
            parentCanvas.worldCamera,
            out pos);
    }

    public void Update() {
        Vector2 movePos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            Input.mousePosition, parentCanvas.worldCamera,
            out movePos);

        transform.position = parentCanvas.transform.TransformPoint(movePos);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackParticleSystemHandler : MonoBehaviour {
    public Transform playerTransform;
    void Start() {
        transform.SetParent(null);
    }

    void Update() {
        transform.position = playerTransform.position;
    }
}

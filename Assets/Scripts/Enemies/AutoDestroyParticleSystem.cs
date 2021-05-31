using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyParticleSystem : MonoBehaviour
{
    private DeathAnimationTimer parent;

    private void Start()
    {
        parent = transform.GetComponentInParent<DeathAnimationTimer>();
    }
    public void OnParticleSystemStopped()
    {
        parent.OnParticelSystemDestoryed();
    }
}

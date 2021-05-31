using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathAnimationTimer : MonoBehaviour
{
    private int childCount = 2;
    // Start is called before the first frame update
    public void OnParticleSystemDestoryed()
    {
        childCount--;
        if (childCount < 1)
        {
            Object.Destroy(gameObject);
        }
    }
}

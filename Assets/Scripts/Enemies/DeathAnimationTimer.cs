using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathAnimationTimer : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnParticelSystemDestoryed()
    {
        if (transform.childCount < 1)
            Object.Destroy(this);
    }
}

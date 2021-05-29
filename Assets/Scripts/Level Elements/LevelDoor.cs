using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDoor : MonoBehaviour
{
    bool keyCollected;
    void Open(){
        Destroy(transform.parent.gameObject);
    }

    void OnTriggerEnter(Collider other){
        if(!other.isTrigger && keyCollected){
            Open();
        }
    }

    public void SetKeyCollected(){
        keyCollected = true;
    }
}

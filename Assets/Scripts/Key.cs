using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] LevelDoor door;
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        //The key should be on PlayerTriggers layer
        if(!other.isTrigger){
            door.SetKeyCollected();
            Destroy(gameObject);
        }
    }
}

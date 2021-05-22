using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameobject.tag == "Player")
        {
            GameObject door = GameObject.FindGameObjectsWithTag("Door");
            door.GetComponent<door>().OpenDoor();
            Destroy(this.gameObject);
        }
    }
}

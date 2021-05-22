using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInteraction : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        //The key should be on PlayerTriggers layer
        if (other.gameObject.tag == "Player")
        {
            GameObject door = GameObject.FindGameObjectWithTag("Door");
            door.GetComponent<Door>().OpenDoor();
            Destroy(this.gameObject);
        }
    }
}

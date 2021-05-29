using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInteraction : MonoBehaviour
{
    public AudioClip keyPickUpClip;

	void OnTriggerEnter(Collider other)
    {
        //The key should be on PlayerTriggers layer
        if (other.gameObject.tag == "Player")
        {
            GeneralAudioPool.Instance.PlaySound(keyPickUpClip, 0.25f, Random.Range(0.9f, 1.1f));

            GameObject door = GameObject.FindGameObjectWithTag("Door");
            door.GetComponent<Door>().OpenDoor();
            Destroy(this.gameObject);
        }
    }
}

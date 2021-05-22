using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    //Animator Variables
    private Animator anim;
    private bool entering = false;

    public float timeToLoadNext = 1f;

    public float belowGround = -3f;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OpenDoor()
    {
        anim.SetBool("isOpen", true);
        GetComponent<Collider>().enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!entering && other.gameObject.tag == "Player")
        {
            GameObject player = other.gameObject;

            entering = true;
            other.enabled = false;
            StartCoroutine(LoadNextLevel(player));
        }
    }

    IEnumerator LoadNextLevel(GameObject player)
    {
        Rigidbody playerRB = player.GetComponent<Rigidbody>();

        while (player.transform.position.y > belowGround)
        {
            yield return null;
        }
        playerRB.collisionDetectionMode = CollisionDetectionMode.Discrete;
        playerRB.isKinematic = true;
        playerRB.velocity = Vector3.zero;
        playerRB.angularVelocity = Vector3.zero;

        yield return new WaitForSeconds(timeToLoadNext);
        //SceneManager.LoadSceneAsync();
    }
}

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

    //Entering Door Variables
    public float slowRoll = 10f;
    public float yPop = 10f;
    public float rotForce = 20f;


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
            StartCoroutine(StopBall(player));
        }
    }


    IEnumerator StopBall(GameObject player)
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        Vector3 towardsHole = (transform.position - player.transform.position);
        Vector3 direction = towardsHole / 1.5f;

        while (rb.velocity.magnitude > .1f)
        {
            rb.velocity -= (rb.velocity * Time.deltaTime * slowRoll);
            yield return null;
        }
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        //impulse force towards hole
        direction += Vector3.up * yPop;
        rb.AddForce(direction, ForceMode.VelocityChange);

        //rotate ball towards hole
        rb.AddTorque(-Vector3.Cross(direction, Vector3.up) * rotForce, ForceMode.VelocityChange);

        //disable collider into hole
        player.GetComponent<Collider>().enabled = false;
        StartCoroutine(LoadNextLevel(rb));
    }

    IEnumerator LoadNextLevel(Rigidbody playerRB)
    {
        
        while (playerRB.position.y > belowGround)
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

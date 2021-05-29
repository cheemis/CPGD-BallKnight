using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelExit : MonoBehaviour
{
    [SerializeField] string NextLevel;
    [SerializeField] Vector3 startLocation;

    //Objects using this script should go on the PlayerTriggers layer so it can only collide with the player
    void OnTriggerEnter(Collider other){
        if(!other.isTrigger){
            DontDestroyOnLoad(this);
            StartCoroutine(LoadLevel(NextLevel));
        }
    }
    IEnumerator LoadLevel(string nextLevel){
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextLevel);
        while (!asyncLoad.isDone){
            yield return null;
        }
        //Next scene is done loading
        Destroy(gameObject);
    }
    
    //only useful if loading the scene additive, not doing right now
    /*IEnumerator UnloadLevel(string previousLevel){
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(previousLevel);
        while (!asyncUnload.isDone){
            yield return null;
        }
        //Current scene is done unloading
        Destroy(gameObject);
    }*/
}

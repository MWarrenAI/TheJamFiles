using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Handles Load state to end the demo of the game
public class DemoEndScript : MonoBehaviour
{
    //I believe this command is called from other scripts
    public void MoveToScene(int sceneID)
    {
        SceneManager.LoadScene(2);
    }
    //Happens only when the player collides
    private void OnTriggerEnter2D(Collider2D other)
    {
        int sceneID = 2;
        SceneManager.LoadScene(sceneID);
    }
}

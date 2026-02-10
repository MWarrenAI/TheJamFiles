using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoEndScript : MonoBehaviour
{
    public void MoveToScene(int sceneID)
    {
        SceneManager.LoadScene(2);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        int sceneID = 2;
        SceneManager.LoadScene(sceneID);
    }
}

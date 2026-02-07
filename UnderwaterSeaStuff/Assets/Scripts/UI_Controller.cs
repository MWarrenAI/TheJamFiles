using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void MoveToScene(int sceneID)
    {
        GameState.shouldStartTutorial = true;
        SceneManager.LoadScene(sceneID);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}

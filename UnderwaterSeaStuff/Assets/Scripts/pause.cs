using System.Collections;
using System.Collections.Generic;
using Unity.Hierarchy;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private float m_PauseDelay = 0.2f;
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    private float m_LastPauseTime;
    private InputAction m_Pause;
    private void Start()
    {
        m_Pause = InputSystem.actions.FindAction("Pause");
    }
    void Update()
    {

        Debug.Log("Frame game...");
        if (m_Pause.IsPressed() && Time.unscaledTime > m_LastPauseTime + m_PauseDelay)
        {
            if (GameIsPaused)
            {
                Debug.Log("Resuming game...");
                Resume();
            }
            else
            {
                Pause();
            }
            m_LastPauseTime = Time.unscaledTime;
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }


    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{



    public InputManager Input;

    private void Start()
    {
        if (Input != null) Input.OnMainMenuInput += BackToMainMenu;
    }
    private void OnDestroy()
    {
        if (Input != null) Input.OnMainMenuInput -= BackToMainMenu;

    }
    private void BackToMainMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("MainMenu");

    }
    public void Play()
    {
        SceneManager.LoadScene("Main Demo Scene");
    }

    public void Exit()
    {
        Application.Quit();
    }
}

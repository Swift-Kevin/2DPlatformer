using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // Define the different items used in the script
    [Header("Elements Used")]
    public GameObject ui;
    public string menuSceneName = "MainMenu";
    public SceneFader sceneFader;

    // Update called every frame and will see if the P or Esc keys are pressed
    // If they are then the pause menu will be displayed
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            Toggle();
        }
    }

    // Toggle flips the games time and the pause menu being shown
    public void Toggle()
    {
        ui.SetActive(!ui.activeSelf);

        if (ui.activeSelf)
        {
            // Game will run at no speed
            Time.timeScale = 0f;
        }
        else
        {
            // Game will run at normal speed
            Time.timeScale = 1f;
        }
    }

    // Retry will set the time to normal (because it would still be set to 0 if not)
    // (Set to 0 because the pause menu didnt change it back after, now it is)
    public void Retry()
    {
        Time.timeScale = 1f;
        sceneFader.FadeTo(SceneManager.GetActiveScene().name);
    }

    // Toggles the menu to hide and then fades to the main menu
    public void Menu()
    {
        Toggle();
        sceneFader.FadeTo(menuSceneName);
    }
}

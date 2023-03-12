using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // Sets the level that will load after being pressed
    [Header("Load Level")]
    [Tooltip("Type the level that you want to load.")]
    public string levelToLoad = "MainLevel";

    // Scene fader for transitioning
    [Header("Scene Fader")]
    public SceneFader sceneFader;

    // Fades to the defined scene (called when the button Play is pressed)
    public void Play()
    {
        sceneFader.FadeTo(levelToLoad);
    }

    // Exits the game (called after the button Quit is pressed)
    public void Quit()
    {
        Debug.Log("Exiting...");
        Application.Quit();
    }
}
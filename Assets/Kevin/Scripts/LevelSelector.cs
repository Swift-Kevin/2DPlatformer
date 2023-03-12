using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    // Objects to be inserted in the inspector to define what the fader and level buttons are
    [Header("Scene Fader")]
    public SceneFader fader;
    [Header("Level Select Buttons")]
    public Button[] levelButtons;

    // Stores the level reached by the player, at minimum level 1
    void Start()
    {
        int levelReached = PlayerPrefs.GetInt("levelReached", 1);
        // For loop to decide if the player can access the next level or not by defeating the previous one
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i + 1 > levelReached)
            {
                levelButtons[i].interactable = false;
            }
        }
    }

    // Select method does a fade to the scene passed through it
    public void Select(string levelName)
    {
        fader.FadeTo(levelName);
    }
}

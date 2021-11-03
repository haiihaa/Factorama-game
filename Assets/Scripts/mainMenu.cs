using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Audio;

public class mainMenu : MonoBehaviour
{
    public AudioMixer mixer;
    // Menu's button interactions

    // PLAY Button:
    public void pressPlay()
    {
        var chose = EventSystem.current.currentSelectedGameObject;
        string name = chose.name;
        // Get level as int
        int length;
        // for level 10 and above
        if (name.Length == 8)
        {
            length = 2;
        }
        else { length = 1; }
        int level = int.Parse(name.Substring(6, length));
        // Check if the player hv unlocked the level
        if (level <= levelData.savedLevel)
        {
            levelData.level = level; // Assign the level to levelData to start the game
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            Debug.Log("Level not unlocked yet!");
        }
    }

    // QUIT Button:
    public void pressQuit()
    {
        Debug.Log("Quit the game!");
        Application.Quit();
    }

    // Player presses QUIT while in-game -> return to main menu scene
    public void pressQuitWhileInGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    // Controll the BGM of menu scene
    public void setVolumeMenuScene(float sliderValue)
    {
        mixer.SetFloat("menuScene", Mathf.Log10(sliderValue) * 20);
        // Update the audio setting to the whole game
        levelData.updateAudioVal(sliderValue);
    }
}

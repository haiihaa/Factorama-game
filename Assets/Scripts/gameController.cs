using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class gameController : MonoBehaviour
{

    private float currTime;
    // Start is called before the first frame update
    private bool tired; // Mode of workers: if paid yet?

    // Worker arrays:
    public GameObject[] receivers;
    public GameObject[] pickers;
    public GameObject[] packers;

    private List<Worker> activeWorkers = new List<Worker>();

    // Fin Note and Order Bar
    public GameObject finNote;

    // Pause button
    public Text pauseText;
    private bool isPaused;
    private bool updatedEndGame;
    private bool won;

    // Level #
    public Text levelNo;

    // Sound Controller
    public AudioClip completeClip, fullStorage;
    public Slider audioSlider;

    void Start()
    {
        // Show weight notification if level 4 || 7 reached
        if ((levelData.level == 4 || levelData.level == 7))
        {
            StartCoroutine(Camera.main.GetComponent<notiPanel>().showWeightNoti());
        }

        // Adjust sound slider to be comarable to the slider at manu scene
        audioSlider.value = levelData.audioSliderVal;
        // Control the debuff event
        // only activate debuff events when reach level 3
        debuff debuffScript = Camera.main.GetComponent<debuff>();
        if (levelData.level < 3) { debuffScript.enabled = false; }
        else { debuffScript.enabled = true; }

        // Control the time 
        tired = false;
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        updatedEndGame = false;
        // Initialize level's statistic ********
        levelNo.text = string.Format("Level {0}", levelData.level);
        //levelData.loadLevel(); // - Already loading in statusBar
        // Load time
        currTime = levelData.time + 4; //4s of countdown
        isPaused = false;

        // Load workers
        // Also record list of active workers
        for (int index = 0; index < 3; index++)
        {
            // Receiving workers
            if (index < levelData.receivingNum)
            {
                receivers[index].SetActive(true);
                activeWorkers.Add(receivers[index].GetComponent<Worker>());
            }
            else { receivers[index].SetActive(false); }

            // Picking workers
            if (index < levelData.pickingNum)
            {
                pickers[index].SetActive(true);
                activeWorkers.Add(pickers[index].GetComponent<Worker>());
            }
            else { pickers[index].SetActive(false); }

            // Packing workers
            if (index < levelData.packingNum)
            {
                packers[index].SetActive(true);
                activeWorkers.Add(packers[index].GetComponent<Worker>());
            }
            else { packers[index].SetActive(false); }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // UPDATE THE TIME COUNTDOWN
        currTime -= Time.deltaTime;
        if (inCountDown())
        {
            return;
        }

        // PAUSE THE GAME WHEN TIME IS OVER
        if (currTime <= 0.0f)
        {
            Time.timeScale = 0;
            // Action to determine lose/win status
            if (!updatedEndGame)
            {
                endTheLevel();
                GameObject.FindGameObjectWithTag("AudioController").GetComponent<AudioController>().SwitchLoop(false);
                GameObject.FindGameObjectWithTag("AudioController").GetComponent<AudioController>().PlayerAudioClip(completeClip);
            }
            // notiPanel will handle the after-game behaviors
        }
    }

    public bool inCountDown()
    {
        return currTime > levelData.time;
    }

    public float getTimePassed()
    {
        return (levelData.time - currTime);
    }

    public float getCurrTime()
    {
        return currTime;
    }

    // Mthod to get item types allowing on this level
    public int getMaxItem(int maxlength)
    {
        if (levelData.level >= 7)
        {
            return maxlength; // Heavy items start at index [13] on item list
        }
        if (levelData.level >= 4)
        {
            return 13; // Medium items index at [8-12] on item list
        }
        return 8; // Light items index at [0-7] on item list
    }

    // Method to reset game statistics and update profit from current level
    // called at every ending of a level
    private void endTheLevel()
    {
        float profit = statusBar.earning;
        // Checking the profit if it meets the target one
        if (profit < levelData.targetProfit)
        {
            won = false;
        }
        else
        {
            // Get next level ready
            levelData.level += 1;
            // Only update saved-data when new level unlocked
            if (levelData.savedLevel == levelData.level - 1)
            {
                levelData.savedLevel = levelData.level;
            }
            won = true;
        }
        updatedEndGame = true;

        // Clear active worker list
        activeWorkers.Clear();

        // Clear the order cards on the order bar
        orderBar.clearOrderBar();
    }

    // Player chooses to retry the level OR move to next level
    public void nextAction()
    {
        //Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Method to handle pause and resume
    public void handlePause()
    {
        // If the game is paused => resume it
        if (isPaused)
        {
            Time.timeScale = 1;
            pauseText.text = "Pause";
            isPaused = false;
        }
        else
        {
            Time.timeScale = 0;
            pauseText.text = "Resume";
            isPaused = true;
        }
    }

    public bool isTired()
    {
        return tired;
    }

    public void handleDebuff(bool underDebuff)
    {
        tired = underDebuff;
        // If under debuff
        if (underDebuff)
        {
            // Slow the workers down
            foreach (var worker in activeWorkers)
            {
                worker.getDebuff();
            }
            // Re-set spd of item
            // Find all items and debuff their handling workers
            GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
            for (int i = 0; i < items.Length; i++)
            {
                items[i].GetComponent<Item>().getDebuff();
            }
        }
        // Else, wage paid
        else
        {
            // Speed the workers up
            foreach (var worker in activeWorkers)
            {
                worker.getSalary();
            }
            // Then buff items and workers' speed to normal state
            GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
            for (int i = 0; i < items.Length; i++)
            {
                items[i].GetComponent<Item>().payWage();
            }
        }
    }

    public bool getLevelStatus()
    {
        return won;
    }

    public bool finalizedEndGame()
    {
        return updatedEndGame;
    }
}

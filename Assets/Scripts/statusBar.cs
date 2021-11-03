using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class statusBar : MonoBehaviour
{
    // Financial note
    public GameObject finNote;
    public Text targetProf;
    private Color currOnVanTextColor = new Color(0.01542656f, 0.1719891f, 0.6132076f);

    // Player's statistics ******************
    public Text money, time, currOnVanPanel;
    private static float currMoney, vanSpeed, dir, fuelCost, wagePaid;
    public static float earning = 0;
    public static int timesOfRunningVan = 0;
    private const float maxDistance = 60.0f;

    // Shipping Van const and var ************
    public GameObject insufficientAmountPanel;
    public GameObject isRunningPanel;
    public GameObject isNotReadyPanel;
    public GameObject van;
    private static bool isRunning;
    private Vector3 vanPos;
    private static float totalReward = 0.0f; // total reward from all the items on the van
    private static int currOnVan = 0;

    bool hasRun;
    public AudioClip fullStorage, CashIn;
    float timer;

    public void Start()
    {
        // Reset data
        earning = 0;
        timesOfRunningVan = 0;
        totalReward = 0.0f;
        currOnVan = 0;
        wagePaid = 0.0f;

        // Initialize level's statistic ********
        levelData.loadLevel();

        currMoney = levelData.capital;
        fuelCost = levelData.vanExpense;
        vanSpeed = levelData.vanSpeed;

        // Displaying
        time.text = string.Format("{0:0}:{1:00}", (int)levelData.time / 60, (int)levelData.time % 60);
        money.text = string.Format("{0:c}", currMoney);
        currOnVanPanel.text = string.Format("{0}/{1}", currOnVan, levelData.maximumOnVan);
        currOnVanPanel.color = currOnVanTextColor;
        targetProf.text = string.Format("{0:c}", levelData.targetProfit);

        // Initialize van's variables ********
        vanPos = van.transform.position;
        isRunning = false;
        dir = 1;
    }

    public void Update()
    {
        // UPDATE THE TIME COUNTDOWN AND PLAYER'S MONEY
        //currTime -= Time.deltaTime;
        // Check if's still in countdown
        if (Camera.main.GetComponent<gameController>().inCountDown())
        {
            return;
        }
        setStat();

        // RUNNING THE VAN 
        if (isRunning)
        {
            van.transform.localPosition += Vector3.right * Time.deltaTime * vanSpeed * dir;
            if (van.transform.localPosition.x > maxDistance)
            {
                dir = -1;
            }
            else if (van.transform.localPosition.x < vanPos.x)
            {
                // Van gets back to the shipping port, stop the van and reset all the var
                //Debug.Log("Van arrived, stop now!");
                hasRun = false;
                GetComponent<AudioSource>().volume = 1f;
                dir = 1;
                isRunning = false;
                // Get the rewards when van is arrived
                currMoney += totalReward;
                GetComponent<AudioSource>().PlayOneShot(CashIn);
                // Reset van's properties 
                currOnVan = 0;
                totalReward = 0.0f;
            }
        }
        // UPDATE earningWithoutSpending
        earning = currMoney;

        // PAUSE THE GAME WHEN TIME IS OVER
        // this is temporary for now
        if (Camera.main.GetComponent<gameController>().getTimePassed() <= 0.0f)
        {
            // Update current earning 
            earning = currMoney;
            // Set stat for the last time
            setStat();
        }

        //fade in and fade out van sound effect
        if (hasRun)
        {
            if (GetComponent<AudioSource>().volume > 0f && timer <= 1000f)
            {
                timer += Time.deltaTime;
                GetComponent<AudioSource>().volume = Mathf.Lerp(GetComponent<AudioSource>().volume, 0f, timer / 1000f);
            }
            else
            {
                timer = 0f;
            }
        }

    }

    // Function to run the van when the player clicks the button
    public void runVan()
    {
        // If the van is running or currMoney is not enough to pay the fuel
        // Do nothing
        if (isRunning)
        {
            StartCoroutine(triggerNoti(isRunningPanel));
            return;
        }
        else if (currMoney < fuelCost)
        {
            StartCoroutine(triggerNoti(insufficientAmountPanel));
            return;
        }
        else if (!vanIsReady())
        {
            StartCoroutine(triggerNoti(isNotReadyPanel));
            return;
        }
        // Else, check if the current amount is enough to pay for fuel
        isRunning = true;
        hasRun = true;
        GetComponent<AudioSource>().Play();
        currMoney -= fuelCost;
        timesOfRunningVan += 1;
    }

    // Set the texts to show current statistics
    private void setStat()
    {
        int minute = (int)Camera.main.GetComponent<gameController>().getCurrTime() / 60;
        int second = (int)Camera.main.GetComponent<gameController>().getCurrTime() % 60;
        time.text = string.Format("{0:0}:{1:00}", minute, second);
        money.text = string.Format("{0:c}", currMoney);
        currOnVanPanel.text = string.Format("{0}/{1}", currOnVan, levelData.maximumOnVan);

        // SET COLOR FOR TIME
        if (minute == 0 && second <= 20) { time.color = Color.red; }

        // SET COLOR FOR MONEY
        if (currMoney <= 30) { money.color = Color.red; }
        else { money.color = new Color(0.1981132f, 0.1981132f, 0.1981132f); }

        // SET COLOR FOR CURR ON VAN
        if (currOnVan == levelData.maximumOnVan) { currOnVanPanel.color = Color.red; }
        else { currOnVanPanel.color = currOnVanTextColor; }
    }

    // Check if van is ready to deliver
    // READY STAGE: When no packing workers on work
    private bool vanIsReady()
    {
        GameObject[] packWorkers = GameObject.FindGameObjectsWithTag("Worker - Pack");
        foreach (var worker in packWorkers)
        {
            if (worker.GetComponent<Worker>().getStatus() == Worker.WorkerStatus.PickingUpItem ||
            worker.GetComponent<Worker>().getStatus() == Worker.WorkerStatus.DroppingOffItem)
            {
                return false;
            }
        }
        return true;
    }

    public static bool vanIsFull()
    {
        return (currOnVan == levelData.maximumOnVan);
    }

    public static bool vanOnDelivery()
    {
        return isRunning;
    }

    public static void updateItemOnVan(float reward)
    {
        currOnVan++;
        totalReward += reward;
        //Debug.Log(currOnVan);
    }

    // Trigger when player clicks the button
    public static void payWage()
    {
        float amount = levelData.totalWorker * levelData.wage;
        currMoney -= amount;
        wagePaid += amount;
    }

    // Method to get amount of wage paid
    public static float getWagePaid()
    {
        return wagePaid;
    }

    // Trigger van's noti panel
    IEnumerator triggerNoti(GameObject panel)
    {
        panel.SetActive(true);
        yield return new WaitForSeconds(2f);
        panel.SetActive(false);
    }
}

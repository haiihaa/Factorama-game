using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script to handle debuff events
public class debuff : MonoBehaviour
{
    private float levelTime;
    private float timeScale;
    public GameObject payWageNotiPanel;
    public GameObject notEnoughMoney;
    public GameObject finNote;
    private int timeOfTrigger;

    // Start
    void Start()
    {
        levelTime = levelData.time;
        timeScale = levelData.timeScale;
        timeOfTrigger = 1;
    }
    // Update is called once per frame
    void Update()
    {
        // Ignore if it's in countdown time
        if (Camera.main.GetComponent<gameController>().inCountDown())
        {
            return;
        }

        float timePassed = Camera.main.GetComponent<gameController>().getTimePassed();
        if ((int)timePassed / timeScale > timeOfTrigger)
        {
            // Then trigger the pay wage event
            payWageNotiPanel.SetActive(true);
            Camera.main.GetComponent<gameController>().handleDebuff(true);

            // at the end, update;
            timeOfTrigger++;
        }
    }

    // When player chooses to pay wage
    public void payWage()
    {
        // If not enough money
        if (statusBar.earning < levelData.totalWorker * levelData.wage)
        {
            // Fire another panel says that not enough money
            StartCoroutine(showNotEnoughMoney());
            return;
        }

        payWageNotiPanel.SetActive(false);

        // Update the status bar
        statusBar.payWage();
        Camera.main.GetComponent<gameController>().handleDebuff(false);
    }

    IEnumerator showNotEnoughMoney()
    {
        notEnoughMoney.SetActive(true);
        yield return new WaitForSeconds(2f);
        notEnoughMoney.SetActive(false);
    }
}

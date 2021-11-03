using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class financialNotes : MonoBehaviour
{
    // INCOME 
    public Text capital, earning, targetProf;
    // EXPENSE
    public Text vanExpense, workWage;
    // TOTAL AND NET
    public Text totalIncome, totalExpense, net;

    private float totalExp, totalInc, currTime, finalNet;
    //private float finalNet = 0.0f;

    private int timeScale = levelData.timeScale;

    // Start: Load expense from levelData
    void Start()
    {
        // Assign fixed values
        timeScale = levelData.timeScale;
        currTime = 0;
        totalExp = 0;
        totalInc = levelData.capital;
        finalNet = totalInc - totalExp;

        // TARGET PROFIT
        targetProf.text = string.Format("Target-profit: {0:c}", levelData.targetProfit);

        // INCOME
        capital.text = string.Format("{0:c}", levelData.capital);

        //EXPENSE
        vanExpense.text = string.Format("{0:c}", 0);
        workWage.text = string.Format("{0:c}", 0);

        // TOTAL AND NET
        totalExpense.text = string.Format("{0:c}", totalExp);
        finalNet = statusBar.earning - totalExp;
    }

    // Update: update current capital
    void Update()
    {
        if (Camera.main.GetComponent<gameController>().inCountDown())
        {
            return;
        }
        // only start updating when countdown is done
        // UPDATE CURRENT WAGES
        currTime += Time.deltaTime;

        // UPDATE CURRENT EXPENSE
        float wagePaid = statusBar.getWagePaid();
        workWage.text = string.Format("{0:c}", wagePaid);
        vanExpense.text = string.Format("{0:c}", levelData.vanExpense * statusBar.timesOfRunningVan);
        totalExp = levelData.vanExpense * statusBar.timesOfRunningVan + wagePaid;
        totalExpense.text = string.Format("{0:c}", totalExp);

        // UPDATE CURRENT INCOME
        float earn = statusBar.earning - levelData.capital + totalExp;
        totalInc = statusBar.earning + totalExp; // Income before the expenses
        totalIncome.text = string.Format("{0:c}", totalInc);
        earning.text = string.Format("{0:c}", earn);

        // NET
        finalNet = statusBar.earning; // After subtract all the expenses
        net.text = string.Format("{0:c}", finalNet);
        if (finalNet < 0) { net.color = Color.red; }
        else { net.color = Color.black; }
    }
}

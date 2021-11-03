using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class notiPanel : MonoBehaviour
{
    public GameObject panel;
    public Text displayText;
    public GameObject hintPanel;
    public Text hintPanelText;
    public GameObject noIdleWorkerPanel;
    public GameObject orderBarReminder;

    // level's stat
    private float timePassed;
    public GameObject winPanel;
    public GameObject losePanel;
    private bool panelUp = false;
    private bool endGame;
    public GameObject finNote;

    // Item's weight Noti
    public GameObject weightNoti;
    public Text weightNotiText;
    //private bool weightNotiShowed;

    // Van is delivering
    public GameObject onDeliveryPanel;

    // Method to trigger noti panels depending on type
    public IEnumerator showNoti(string type)
    {
        displayText.text = type + " is full!";
        switch (type)
        {
            case "Storage":
                hintPanelText.text = "Hint: Let the pick worker pick the item first!";
                break;
            case "Van":
                hintPanelText.text = "Hint: Deliver the van!";
                break;
            default:
                hintPanelText.text = "Hint: Empty the last basket first then press SPACE to rerun the belt!";
                break;
        }

        panel.SetActive(true);
        hintPanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        panel.SetActive(false);
        yield return new WaitForSeconds(1f);
        hintPanel.SetActive(false);
    }

    // Method to trigger weight noti panel when player reaches level 4 or 7
    public IEnumerator showWeightNoti()
    {
        switch (levelData.level)
        {
            case 4:
                weightNotiText.text = "medium-weight items are coming. workers move slower when holding these!";
                break;
            case 7:
                weightNotiText.text = "heavy-weight items are coming. workers move slower when holding these!";
                break;
            default:
                break;
        }
        weightNoti.SetActive(true);
        yield return new WaitForSeconds(3f);
        weightNoti.SetActive(false);
    }

    // Show van on delivery noti
    public IEnumerator showOnDeliveryNoti()
    {
        onDeliveryPanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        onDeliveryPanel.SetActive(false);
    }

    // Start
    public void Start()
    {
        //weightNotiShowed = false;
        endGame = false;
    }
    // Update
    public void Update()
    {
        if (Camera.main.GetComponent<gameController>().inCountDown())
        {
            return;
        }

        timePassed = Camera.main.GetComponent<gameController>().getTimePassed();
        endGame = Camera.main.GetComponent<gameController>().finalizedEndGame();
        if (timePassed > levelData.time && !panelUp && endGame)
        {
            // Show up the financial note for viewing
            finNote.SetActive(true);
            // Identify lost/won status
            bool won = Camera.main.GetComponent<gameController>().getLevelStatus();
            if (won)
            {
                winPanel.SetActive(true);
            }
            else
            {
                losePanel.SetActive(true);
            }
            // Stop repeating this trigger
            panelUp = true;
        }
    }

    public IEnumerator showNoIdleWorker()
    {
        noIdleWorkerPanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        noIdleWorkerPanel.SetActive(false);
    }

    public IEnumerator showOrderBarReminder()
    {
        orderBarReminder.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        orderBarReminder.SetActive(false);
    }
}

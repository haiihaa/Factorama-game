using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class countDown : MonoBehaviour
{
    public int countdown;
    public Text countdownDisplay;
    AudioController audioController;

    private void Awake()
    {
        audioController = GameObject.FindGameObjectWithTag("AudioController").GetComponent<AudioController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(countDownToStart());
        audioController.ChangeVolume(0.4f);
        audioController.PlayerAudioClip(audioController.clip1);
    }
    IEnumerator countDownToStart()
    {
        while (countdown > 0)
        {
            countdownDisplay.text = countdown.ToString();
            yield return new WaitForSeconds(1f);
            countdown--;
        }
        // Start the game
        countdownDisplay.text = "GO!";
        yield return new WaitForSeconds(1f);
        audioController.ChangeVolume(0.4f);
        audioController.PlayerAudioClip(audioController.clip2);
        audioController.SwitchLoop(true);
        countdownDisplay.gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioClip clip1, clip2;
    public void PlayerAudioClip(AudioClip audioClip)
    {
        GetComponent<AudioSource>().clip = audioClip;
        GetComponent<AudioSource>().Play();
    }
    public void SwitchLoop(bool On)
    {
        GetComponent<AudioSource>().loop = On;
    }

    public void ChangeVolume(float sliderValue)
    {
        GetComponent<AudioSource>().volume = sliderValue;
        // Update the val
        levelData.updateAudioVal(sliderValue);
    }
}

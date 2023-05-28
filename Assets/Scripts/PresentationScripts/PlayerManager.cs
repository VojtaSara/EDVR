using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public float presentationLength = 50f;
    private float elapsedTime = 0f;
    private bool isPlaying = false;

    // get child slider component and change its value
    public void ChangeSliderValue(float value)
    {
        GetComponentInChildren<UnityEngine.UI.Slider>().value = value;
    }

    private string secondsToTimeStamp(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time % 60;
        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    private void updateTimeStamp()
    {
        GetComponentInChildren<UnityEngine.UI.Text>().text = secondsToTimeStamp(elapsedTime);
    }

    public void Update()
    {
        if (isPlaying)
        {
            elapsedTime += Time.deltaTime;
            ChangeSliderValue(elapsedTime / presentationLength);
            if (elapsedTime > presentationLength)
            {
                elapsedTime = presentationLength;
            }
            updateTimeStamp();
        }
    }

    public void MainPlayPause()
    {
        isPlaying = !isPlaying;
        // search for all gameobjects with the component Playable and call the PlayPause method on them
        foreach (Playable playable in FindObjectsOfType<Playable>())
        {
            playable.PlayPause();
        }
    }

    public void MainScrub(float sliderValue)
    {
        elapsedTime = sliderValue * presentationLength;
        updateTimeStamp();
        // search for all gameobjects with the component Playable and call the Scrub method on them
        foreach (Playable playable in FindObjectsOfType<Playable>())
        {
            playable.Scrub(elapsedTime, presentationLength);
        }
    }
}

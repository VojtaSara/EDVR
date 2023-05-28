using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlAudio : MonoBehaviour
{
    public void PlayPause()
    {
        if (GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().Pause();
        }
        else
        {
            GetComponent<AudioSource>().Play();
        }
    }

    public int GetAudioLength()
    {
        return GetComponent<AudioSource>().clip.samples;
    }

    public int GetAudioPosition()
    {
        return GetComponent<AudioSource>().timeSamples;
    }
}

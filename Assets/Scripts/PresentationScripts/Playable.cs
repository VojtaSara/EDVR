using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playable : MonoBehaviour
{
    public float beginPlayTime = 0f;
    public float animationLength = 0f;

    // Start is called before the first frame update
    void Start()
    {
        // if this gameobject has video player component, pause it
        if (GetComponent<UnityEngine.Video.VideoPlayer>() != null)
        {
            GetComponent<UnityEngine.Video.VideoPlayer>().Pause();
        }

        // if this gameobject has audio source component, pause it
        if (GetComponent<AudioSource>() != null)
        {
            GetComponent<AudioSource>().Pause();
        }

        // if this gameobject has an Animator component, pause it
        if (GetComponent<Animator>() != null)
        {
            GetComponent<Animator>().speed = 0;
        }
    }

    public void PlayPause()
    {
        // if this gameobject has video player component, pause it
        if (GetComponent<UnityEngine.Video.VideoPlayer>() != null)
        {
            if (GetComponent<UnityEngine.Video.VideoPlayer>().isPlaying)
            {
                GetComponent<UnityEngine.Video.VideoPlayer>().Pause();
            }
            else
            {
                GetComponent<UnityEngine.Video.VideoPlayer>().Play();
            }
        }

        // if this gameobject has audio source component, pause it
        if (GetComponent<AudioSource>() != null)
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

        // if this gameobject has an Animator component, pause it
        if (GetComponent<Animator>() != null)
        {
            if (GetComponent<Animator>().speed == 1)
            {
                GetComponent<Animator>().speed = 0;
            }
            else
            {
                GetComponent<Animator>().speed = 1;
            }
        }
    }

    public void Scrub(float time, float presentationLength)
    {
        if (GetComponent<UnityEngine.Video.VideoPlayer>() != null)
        {
            if (Mathf.Abs((float)GetComponent<UnityEngine.Video.VideoPlayer>().time - time) > 0.1f)
            {
                GetComponent<UnityEngine.Video.VideoPlayer>().time = time;
            }
        }

        if (GetComponent<AudioSource>() != null)
        {
            if (Mathf.Abs((float)GetComponent<AudioSource>().time - time) > 0.1f)
            {
                GetComponent<AudioSource>().time = time;
            }
        }

        if (GetComponent<Animator>() != null)
        {
            GetComponent<Animator>().Play(0, 0, time / presentationLength * animationLength);
        }

        if (Mathf.Abs(time - presentationLength) < 0.1f)
        {
            if (GetComponent<UnityEngine.Video.VideoPlayer>() != null)
            {
                GetComponent<UnityEngine.Video.VideoPlayer>().Stop();
            }

            if (GetComponent<AudioSource>() != null)
            {
                GetComponent<AudioSource>().Stop();
            }

            if (GetComponent<Animator>() != null)
            {
                GetComponent<Animator>().speed = 0;
            }
        }
    }
}

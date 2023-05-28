using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Texture Play;
    public Texture Pause;

    private bool playPause;

    public void PlayPause()
    {
        // get the raw image component of this gameobject
        if (playPause)
        {
            GetComponent<UnityEngine.UI.RawImage>().texture = Play;
        }
        else
        {
            GetComponent<UnityEngine.UI.RawImage>().texture = Pause;
        }
        playPause = !playPause;
    }
}

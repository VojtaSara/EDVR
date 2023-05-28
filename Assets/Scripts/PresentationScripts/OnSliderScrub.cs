using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Slider))]
public class OnSliderScrub : MonoBehaviour
{
    public PlayerManager playerManager;
    // on slider change event
    public void OnSliderChange()
    {
        // get the value of this slider
        float value = GetComponent<UnityEngine.UI.Slider>().value;
        playerManager.MainScrub(value);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneName : MonoBehaviour
{
    public void ChangeText()
    {
        // change the text component - This is not working for some reason
        this.GetComponent<Text>().text = "Scene: " + GameObject.Find("Particle System").GetComponent<PointCloudLoader>().CurrSceneName();
    }
}

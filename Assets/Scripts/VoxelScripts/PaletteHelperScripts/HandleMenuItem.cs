using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleMenuItem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HideMenu()
    {
        gameObject.SetActive(false);
    }

    public void ShowMenu()
    {
        gameObject.SetActive(true);
    }
}

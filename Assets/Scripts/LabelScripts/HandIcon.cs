using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(InputData))]
public class HandIcon : MonoBehaviour
{
    private InputData _inputData;
    public string handOrientation;
    public GameObject handOpen;
    public GameObject handClosed;

    // Start is called before the first frame update
    void Start()
    {
        _inputData = GetComponent<InputData>();

    }

    // Update is called once per frame
    void Update()
    {
        // get the trigger value of the right controller
        float triggerValue;
        if (handOrientation == "right")
        {
            _inputData._rightController.TryGetFeatureValue(CommonUsages.trigger, out triggerValue);
        }
        else
        {
            _inputData._leftController.TryGetFeatureValue(CommonUsages.trigger, out triggerValue);
        }
        
        // if the trigger is pressed, show the hand icon
        if (triggerValue > 0.5f)
        {
            // close the hand icon
            handOpen.SetActive(false);
            handClosed.SetActive(true);
        }
        else
        {
            // open the hand icon
            handOpen.SetActive(true);
            handClosed.SetActive(false);
        }
    }
}

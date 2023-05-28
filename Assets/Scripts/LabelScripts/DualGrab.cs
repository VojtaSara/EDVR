using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(InputData))]

public class DualGrab : MonoBehaviour
{
    private InputData _inputData;
    public GameObject labelBrush;

    void Start()
    {
        _inputData = GetComponent<InputData>();
    }

    Vector3 lastRightPosition = Vector3.negativeInfinity;
    Vector3 lastLeftPosition = Vector3.negativeInfinity;
    float lastScale = -1;

    private float scaleDampener = 1f;
    private float moveDampener = 2f;

    // Update is called once per frame
    void Update()
    {
        if (_inputData._rightController.isValid && _inputData._leftController.isValid)
        {
            // right controller
            float rightTrigger;
            _inputData._rightController.TryGetFeatureValue(CommonUsages.trigger, out rightTrigger);
            Vector3 rightPosition;
            _inputData._rightController.TryGetFeatureValue(CommonUsages.devicePosition, out rightPosition);
            // left controller
            float leftTrigger;
            _inputData._leftController.TryGetFeatureValue(CommonUsages.trigger, out leftTrigger);
            Vector3 leftPosition;
            _inputData._leftController.TryGetFeatureValue(CommonUsages.devicePosition, out leftPosition);

            // If right hand is not in hand mode, then do not scale
            if (labelBrush.GetComponent<LabelBrush>().tool != LabelBrush.selectedTool.hand)
            {
                rightTrigger = 0;
            }

            if (rightTrigger > 0.6 && leftTrigger > 0.6)
            {
                float newScale = VectorMagnitude(leftPosition - rightPosition);
                if (lastScale != -1 && lastScale != newScale)
                {
                    Vector3 scaleChange = scaleDampener * ScalarToVector(newScale - lastScale);
                    // Clamp the scale of the grabbed scene and avoid negative scale
                    if ((this.transform.localScale + scaleChange).x > 0.0005f)
                    {
                        this.transform.localScale += scaleChange;
                        if (notInfinity(rightPosition - lastRightPosition))
                        {
                            this.transform.position += moveDampener * (rightPosition - lastRightPosition);
                        }
                        lastRightPosition = rightPosition;
                    }
                }
                lastScale = newScale;
            }
            else if (rightTrigger > 0.6)
            {
                if (notInfinity(rightPosition - lastRightPosition))
                {
                    this.transform.position += moveDampener * (rightPosition - lastRightPosition);
                }
                lastRightPosition = rightPosition;
            }
            else if (leftTrigger > 0.6)
            {
                if (notInfinity(leftPosition - lastLeftPosition))
                {
                    this.transform.position += moveDampener * ( leftPosition - lastLeftPosition);
                }
                lastLeftPosition = leftPosition;
            }
            else
            {
                lastRightPosition = Vector3.negativeInfinity;
                lastLeftPosition = Vector3.negativeInfinity;
                lastScale = -1;
            }
        }
    }

    private bool notInfinity(Vector3 vector3)
    {
        bool positiveInfinity = vector3.x != float.PositiveInfinity && vector3.y != float.PositiveInfinity && vector3.z != float.PositiveInfinity;
        bool negativeInfinity = vector3.x != float.NegativeInfinity && vector3.y != float.NegativeInfinity && vector3.z != float.NegativeInfinity;
        return positiveInfinity && negativeInfinity;
    }

    private Vector3 ScalarToVector(float v)
    {
        return new Vector3(v, v, v);
    }

    private float VectorMagnitude(Vector3 vector3)
    {
        return (float)Math.Sqrt(Math.Pow(vector3.x, 2) + Math.Pow(vector3.y, 2) + Math.Pow(vector3.z, 2));
    }
}



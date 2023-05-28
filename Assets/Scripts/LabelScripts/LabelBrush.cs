using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(InputData))]
public class LabelBrush : MonoBehaviour
{
    private InputData _inputData;
    private float LcursorLength;
    private float RcursorLength;
    private Vector3 offset = new Vector3(0f, -0.02f, 0.1f);
    public Vector3 offset2 = new Vector3(0f, -0.04f, 0.2f);
    private Vector3 rightTip;
    private Vector3 leftTip;
    public LabelClass selectedLabelClass;
    private Vector3 lastPos;
    private float brushSize = 0.1f;

    public GameObject rightHandTool;
    public GameObject LabelCube;
    public GameObject brushSphere;
    public GameObject leftLine;
    public GameObject rightLine;

    public GameObject pointCloud;

    public enum selectedTool
    {
        sphereBrush,
        bboxBrush,
        hand
    }
    public selectedTool tool = selectedTool.hand;
    
    // Start is called before the first frame update
    void Start()
    {
        _inputData = GetComponent<InputData>();
    }

    private bool pressed1 = false;
    private bool pressed2 = false;
    // Update is called once per frame
    void Update()
    {
        if (tool == selectedTool.bboxBrush)
        {
            rightHandTool.SetActive(false);
            LabelCube.SetActive(true);
            leftLine.SetActive(true);
            rightLine.SetActive(true);
            brushSphere.SetActive(false);
            UpdateBboxMode();
        }
        else if (tool == selectedTool.sphereBrush)
        {
            rightHandTool.SetActive(false);
            LabelCube.SetActive(false);
            leftLine.SetActive(false);
            rightLine.SetActive(false);
            brushSphere.SetActive(true);
            UpdateSphereBrushMode();
        }
        else if (tool == selectedTool.hand)
        {
            rightHandTool.SetActive(true);
            LabelCube.SetActive(false);
            leftLine.SetActive(false);
            rightLine.SetActive(false);
            brushSphere.SetActive(false);
        }
    }

    public void ChangeTool(string toolName)
    {
        if (toolName == "sphereBrush")
        {
            tool = selectedTool.sphereBrush;
        }
        else if (toolName == "bboxBrush")
        {
            tool = selectedTool.bboxBrush;
        }
        else if (toolName == "hand")
        {
            tool = selectedTool.hand;
        }
    }

    void UpdateBboxMode()
    {
        if (_inputData._leftController.isValid)
        {
            RenderHandLine(_inputData._leftController, "left");
        }
        if (_inputData._rightController.isValid)
        {
            RenderHandLine(_inputData._rightController, "right");

            bool button1pressed1;
            _inputData._rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out button1pressed1);
            if (!pressed1 && button1pressed1)
            {
                Vector3 position;
                Quaternion rotation;
                _inputData._leftController.TryGetFeatureValue(CommonUsages.devicePosition, out position);
                _inputData._leftController.TryGetFeatureValue(CommonUsages.deviceRotation, out rotation);
                LabelDatabase labelDatabase = GameObject.Find("LabelDatabase").GetComponent<LabelDatabase>();
                labelDatabase.AddLabel(
                    BoxPoints.GetBoxPoints(leftTip, position + rotation * offset, rightTip, rotation),
                    selectedLabelClass.name,
                    selectedLabelClass.color);

                pressed1 = true;
            }
            else if (pressed1 && !button1pressed1) pressed1 = false;

            bool button2pressed1;
            _inputData._rightController.TryGetFeatureValue(CommonUsages.primaryButton, out button2pressed1);
            if (!pressed2 && button2pressed1)
            {
                LabelDatabase labelDatabase = GameObject.Find("LabelDatabase").GetComponent<LabelDatabase>();
                labelDatabase.RemoveLastLabel();
                pressed2 = true;
            }
            else if (pressed2 && !button2pressed1) pressed2 = false;
        }
        RenderBox(_inputData, rightTip);
    }

    void UpdateSphereBrushMode()
    {
        if (_inputData._rightController.isValid)
        {
            float rightTrigger;
            _inputData._rightController.TryGetFeatureValue(CommonUsages.trigger, out rightTrigger);
            Vector3 position;
            Quaternion rotation;
            _inputData._rightController.TryGetFeatureValue(CommonUsages.devicePosition, out position);
            _inputData._rightController.TryGetFeatureValue(CommonUsages.deviceRotation, out rotation);
            if (!pressed1 && rightTrigger > 0.5f)
            {
                //PointsToParticles pointsToParticles = GameObject.Find("Particle System").GetComponent<PointsToParticles>();
                //pointsToParticles.ColorPoints(rightTip, 10*Math.Abs(RcursorLength), Color.red);
                PointCloudRenderer pointCloudRenderer;
                pointCloud.TryGetComponent<PointCloudRenderer>(out pointCloudRenderer);

                if (pointCloudRenderer)
                {
                    if (selectedLabelClass != null)
                    {
                        pointCloudRenderer.ColorPoints(position + rotation * offset2, Math.Abs(brushSize), selectedLabelClass.color);
                    }
                }
                
                lastPos = position;
                pressed1 = true;
            }
            else if (pressed1 && !(rightTrigger > 0.5f)) pressed1 = false;
            if (Vector3.Distance(position, lastPos) > 0.001f)
            {
                pressed1 = false;
            }

            UpdateBrush();
        }
    }

    void UpdateBrush()
    {
        if (_inputData._rightController.isValid)
        {
            Vector2 joystickPosition;
            _inputData._rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out joystickPosition);

            brushSize += 0.01f * joystickPosition.y;
            if (brushSize < 0.01f) brushSize = 0.01f;
            if (brushSize > 2f) brushSize = 2f;

            brushSphere.transform.localScale = new Vector3(brushSize, brushSize, brushSize);

            Vector3 position;
            Quaternion rotation;
            _inputData._rightController.TryGetFeatureValue(CommonUsages.devicePosition, out position);
            _inputData._rightController.TryGetFeatureValue(CommonUsages.deviceRotation, out rotation);
            brushSphere.transform.position = position + rotation * offset2;
        }
    }


    void RenderHandLine(InputDevice controller, string hand)
    {
        if (controller.isValid)
        {
            Vector2 joystickPosition;
            controller.TryGetFeatureValue(CommonUsages.primary2DAxis, out joystickPosition);
            
            if (hand == "left")
            {
                LcursorLength += -0.01f * joystickPosition.y;
                if (LcursorLength > -0.01f) LcursorLength = -0.01f;
            }
            else
            {
                RcursorLength += -0.01f * joystickPosition.y;
                if (RcursorLength > -0.01f) RcursorLength = -0.01f;
            }
            
            Vector3 position;
            Quaternion rotation;
            controller.TryGetFeatureValue(CommonUsages.devicePosition, out position);
            controller.TryGetFeatureValue(CommonUsages.deviceRotation, out rotation);
            
            Vector3 newPosition = position + rotation * offset;


            GameObject child1 = hand == "left" ? leftLine : rightLine;

            rightTip = newPosition + rotation * new Vector3(RcursorLength * 0.01f, RcursorLength,0);

            LineRenderer lineRenderer = child1.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, newPosition);
            lineRenderer.SetPosition(1, 
                newPosition + rotation * new Vector3(
                    (hand == "left" ? LcursorLength : RcursorLength) * 0.01f,
                    (hand == "left" ? LcursorLength : RcursorLength),
                    0));
        }
    }

    void RenderBox(InputData _inputData, Vector3 rightTip)
    {
        Vector3 position;
        Quaternion rotation;
        
        _inputData._leftController.TryGetFeatureValue(CommonUsages.devicePosition, out position);
        _inputData._leftController.TryGetFeatureValue(CommonUsages.deviceRotation, out rotation);
        
        Vector3 newPosition = position + rotation * offset;

        leftTip = newPosition + rotation * new Vector3(LcursorLength * 0.01f, LcursorLength, 0);

        transform.position = leftTip;
        transform.rotation = rotation;
        
        Vector3 boxScale = BoxPoints.GetScale(leftTip, newPosition, rightTip, rotation);
        if (boxScale.x == 0 || boxScale.y == 0 || boxScale.z == 0)
        {
            transform.localScale = new Vector3(0, 0, 0);
        }
        else
        {
            transform.localScale = boxScale;
        }
    }

    public string GetSelectedLabel()
    {
        return selectedLabelClass.name;
    }
}

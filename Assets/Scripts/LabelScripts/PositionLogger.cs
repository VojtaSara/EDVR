using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System.IO;
using System;

// Position Logger Output Format:
//
// | Position in output txt file | Description                                                           |
// |-----------------------------|-----------------------------------------------------------------------|
// | 1                           | Current date and time in the format yyyy-MM-dd HH:mm:ss.fff           |
// | 2                           | X coordinate of the right controller's position                       |
// | 3                           | Y coordinate of the right controller's position                       |
// | 4                           | Z coordinate of the right controller's position                       |
// | 5                           | X component of the right controller's rotation quaternion             |
// | 6                           | Y component of the right controller's rotation quaternion             |
// | 7                           | Z component of the right controller's rotation quaternion             |
// | 8                           | W component of the right controller's rotation quaternion             |
// | 9                           | X coordinate of the left controller's position                        |
// | 10                          | Y coordinate of the left controller's position                        |
// | 11                          | Z coordinate of the left controller's position                        |
// | 12                          | X component of the left controller's rotation quaternion              |
// | 13                          | Y component of the left controller's rotation quaternion              |
// | 14                          | Z component of the left controller's rotation quaternion              |
// | 15                          | W component of the left controller's rotation quaternion              |
// |-----------------------------------------------------------------------------------------------------|

[RequireComponent(typeof(InputData))]
public class PositionLogger : MonoBehaviour
{
    private InputData _inputData;
    

    void Start()
    {
        _inputData = GetComponent<InputData>();
        StartCoroutine(LogPosition());
    }

    private IEnumerator LogPosition()
    {
        string fileName = "position_log.txt";
        string path = Path.Combine(Application.persistentDataPath, fileName);

        while (true)
        {
            Vector3 rightControllerPosition;
            Quaternion rightControllerRotation;
            _inputData._rightController.TryGetFeatureValue(CommonUsages.devicePosition, out rightControllerPosition);
            _inputData._rightController.TryGetFeatureValue(CommonUsages.deviceRotation, out rightControllerRotation);

            Vector3 leftControllerPosition;
            Quaternion leftControllerRotation;
            _inputData._leftController.TryGetFeatureValue(CommonUsages.devicePosition, out leftControllerPosition);
            _inputData._leftController.TryGetFeatureValue(CommonUsages.deviceRotation, out leftControllerRotation);

            Vector3 headsetPosition;
            Quaternion headsetRotation;
            _inputData._HMD.TryGetFeatureValue(CommonUsages.devicePosition, out headsetPosition);
            _inputData._HMD.TryGetFeatureValue(CommonUsages.deviceRotation, out headsetRotation);

            string logMessage = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}\n",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), // current time
                rightControllerPosition.x, rightControllerPosition.y, rightControllerPosition.z, // right controller position
                rightControllerRotation.x, rightControllerRotation.y, rightControllerRotation.z, rightControllerRotation.w, // right controller rotation
                leftControllerPosition.x, leftControllerPosition.y, leftControllerPosition.z, // left controller position
                leftControllerRotation.x, leftControllerRotation.y, leftControllerRotation.z, leftControllerRotation.w // left controller rotation
            );

            File.AppendAllText(path, logMessage);

            yield return new WaitForSeconds(1.0f);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxPoints : MonoBehaviour
{
    public static Vector3 GetScale(Vector3 leftTip, Vector3 leftBase, Vector3 rightTip, Quaternion rotation)
    {
        Vector3 leftDepthVector = leftTip + rotation * new Vector3(0, -0.001f, 0);

        Vector3 projectedPointY = VectorFunctions.ProjectPointOntoLine(leftTip, leftBase, rightTip);
        float scaleY = VectorFunctions.CalculateDistance(projectedPointY, leftTip);
        if (VectorFunctions.CalculateDistance(projectedPointY, leftTip) <
        VectorFunctions.CalculateDistance(projectedPointY, leftDepthVector))
        {
            scaleY = 0;
        }

        Vector3 leftUpVector = leftTip + rotation * new Vector3(0, 0, 0.001f);


        Vector3 projectedPointZ = VectorFunctions.ProjectPointOntoLine(leftTip, leftUpVector, rightTip);
        float scaleZ = VectorFunctions.CalculateDistance(projectedPointZ, leftTip);
        if (VectorFunctions.CalculateDistance(projectedPointZ, leftTip) <
        VectorFunctions.CalculateDistance(projectedPointZ, leftUpVector))
        {
            scaleZ = 0;
        }

        Vector3 leftSideVector = leftTip + rotation * new Vector3(0.001f, 0, 0);

        Vector3 projectedPointX = VectorFunctions.ProjectPointOntoLine(leftTip, leftSideVector, rightTip);
        float scaleX = VectorFunctions.CalculateDistance(projectedPointX, leftTip);
        if (VectorFunctions.CalculateDistance(projectedPointX, leftTip) <
            VectorFunctions.CalculateDistance(projectedPointX, leftSideVector))
        {
            scaleX = 0;
        }

        return new Vector3(scaleX, scaleY, scaleZ);
    }

    public static Vector3[] GetBoxPoints(Vector3 leftTip, Vector3 leftBase, Vector3 rightTip, Quaternion rotation)
    {
        Vector3 scale = GetScale(leftTip, leftBase, rightTip, rotation);
        if (scale.x == 0 || scale.y == 0 || scale.z == 0) return null;

        Vector3[] points = new Vector3[16];

        points[0] = leftTip;
        points[1] = leftTip + rotation * new Vector3(0, 0, scale.z);
        points[2] = leftTip + rotation * new Vector3(scale.x, 0, scale.z);
        points[3] = leftTip + rotation * new Vector3(scale.x, 0, 0);
        points[4] = leftTip + rotation * new Vector3(0, 0, 0);
        points[5] = leftTip + rotation * new Vector3(0, -scale.y, 0);
        points[6] = leftTip + rotation * new Vector3(0, -scale.y, scale.z);
        points[7] = leftTip + rotation * new Vector3(0, 0, scale.z);
        points[8] = leftTip + rotation * new Vector3(scale.x, 0, scale.z);
        points[9] = leftTip + rotation * new Vector3(scale.x, -scale.y, scale.z);
        points[10] = leftTip + rotation * new Vector3(0, -scale.y, scale.z);
        points[11] = leftTip + rotation * new Vector3(0, -scale.y, 0);
        points[12] = leftTip + rotation * new Vector3(scale.x, -scale.y, 0);
        points[13] = leftTip + rotation * new Vector3(scale.x, -scale.y, scale.z);
        points[14] = leftTip + rotation * new Vector3(scale.x, -scale.y, 0);
        points[15] = leftTip + rotation * new Vector3(scale.x, 0, 0);

        return points;
    }
}

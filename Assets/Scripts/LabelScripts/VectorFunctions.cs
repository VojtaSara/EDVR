using UnityEngine;
using System;

public static class VectorFunctions
{
    public static Vector3 ProjectPointOntoLine(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
    {
        Vector3 lineDirection = Vector3.Normalize(linePoint2 - linePoint1);
        float projection = Vector3.Dot(point - linePoint1, lineDirection);
        Vector3 projectionVector = projection * lineDirection;
        Vector3 projectedPoint = linePoint1 + projectionVector;
        return projectedPoint;
    }

    public static float CalculateDistance(Vector3 point1, Vector3 point2)
    {
        Vector3 difference = point2 - point1;
        float distance = (float)Math.Sqrt(difference.x * difference.x + difference.y * difference.y + difference.z * difference.z);
        return distance;
    }
}

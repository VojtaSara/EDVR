using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem), typeof(PointCloudLoader))]
public class PointsToParticles : MonoBehaviour
{
    public int numParticles = 2000;
    public float radius = 1f;
    public float particleSize = 0.1f;

    private PointCloudLoader pointCloudLoader;

    void Start()
    {
        pointCloudLoader = GetComponent<PointCloudLoader>();
        ParticleSystem ps = GetComponent<ParticleSystem>();
        //numParticles = pointCloudLoader.numOfLinesOfCurrentFile();
        // access main of the particle system
        ParticleSystem.MainModule main = ps.main;
        main.startSize = particleSize;
        main.loop = false;
        main.maxParticles = numParticles;
        main.startLifetime = Mathf.Infinity;
        // Set particle system settings
        GetComponent<ParticleSystem>().Emit(numParticles);
    }

    public void TransformIntoPosition(Vector3[] points)
    {
        // Update particle positions
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[numParticles];
        GetComponent<ParticleSystem>().GetParticles(particles);

        for (int i = 0; i < Mathf.Min(particles.Length, points.Length); i++)
        {
            particles[i].position = points[i];
        }
        GetComponent<ParticleSystem>().SetParticles(particles, particles.Length);
    }

    public void ColorPoints(Vector3 center, float radius, Color color)
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[numParticles];
        int numParticlesInRange = ps.GetParticles(particles);

        float[] distances = new float[numParticlesInRange];

        for (int i = 0; i < numParticlesInRange; i++)
        {
            ParticleSystem.Particle particle = particles[i];
            Vector3 controllerPos = this.transform.InverseTransformPoint(center);
            float distance = Vector3.Distance(controllerPos, particle.position);

            if (distance < radius)
            {
                particle.startColor = color;
            }
            particles[i] = particle;
        }
        // print minimum and maximum distance
        ps.SetParticles(particles, numParticlesInRange);
    }

    public void ColorPointsInBox(Vector3[] boxCorners, Color color)
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[numParticles];
        int numParticlesInBox = ps.GetParticles(particles);
        Debug.Log(boxCorners);

        for (int i = 0; i < numParticlesInBox; i++)
        {
            ParticleSystem.Particle particle = particles[i];
            bool insideBox = IsPointInsideBox(particle.position, boxCorners);

            if (insideBox)
            {
                particle.startColor = color;
            }

            particles[i] = particle;
        }

        ps.SetParticles(particles, numParticlesInBox);
    }

    bool IsPointInsideBox(Vector3 point, Vector3[] boxCorners)
    {
        bool insideBox = true;
        Vector3[] cubePts = { boxCorners[0] , boxCorners[1], boxCorners[5], boxCorners[6], boxCorners[15], boxCorners[8], boxCorners[12], boxCorners[13] };

        // Calculate normals of box faces
        Vector3[] faceNormals = new Vector3[6];
        faceNormals[0] = Vector3.Cross(cubePts[1] - cubePts[0], cubePts[2] - cubePts[0]).normalized;
        faceNormals[1] = Vector3.Cross(cubePts[1] - cubePts[5], cubePts[4] - cubePts[5]).normalized;
        faceNormals[2] = Vector3.Cross(cubePts[4] - cubePts[0], cubePts[3] - cubePts[0]).normalized;
        faceNormals[3] = Vector3.Cross(cubePts[1] - cubePts[2], cubePts[6] - cubePts[2]).normalized;
        faceNormals[4] = Vector3.Cross(cubePts[4] - cubePts[5], cubePts[7] - cubePts[5]).normalized;
        faceNormals[5] = Vector3.Cross(cubePts[2] - cubePts[3], cubePts[7] - cubePts[3]).normalized;

        for (int j = 0; j < 1; j++)
        {
            Vector3 normal = faceNormals[j];
            Vector3 corner = cubePts[j];

            if (Vector3.Dot(normal, point - corner) > 0)
            {
                insideBox = false;
                break;
            }
        }

        return insideBox;
    }
}

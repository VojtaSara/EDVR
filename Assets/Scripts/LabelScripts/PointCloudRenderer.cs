using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.VFX;

public class PointCloudRenderer : MonoBehaviour
{
    private string SAVED_LABELS_PATH;

    Texture2D texColor;
    Texture2D texPosScale;
    VisualEffect vfx;
    uint resolution = 512;

    public float particleSize = 0.0002f;
    bool toUpdate = false;
    uint particleCount = 0;
    private string currSceneName = "menu";

    Vector3[] pcPoints;
    Vector3[] pcColors;

    private void Start()
    {
        SAVED_LABELS_PATH = Path.Combine(
            Application.persistentDataPath,
            "SavedLabelsSemantic"
        );
        vfx = GetComponent<VisualEffect>();
        // disable the visual effect
        vfx.enabled = false;

        Vector3[] positions = new Vector3[(int)resolution * (int)resolution];
        Color[] colors = new Color[(int)resolution * (int)resolution];

        StartCoroutine(
            SaveTexturePeriodically(10f, SAVED_LABELS_PATH)
        );
    }
    string GetFormattedDateTime()
    {
        DateTime dateTime = DateTime.Now;
        string year = dateTime.Year.ToString().Substring(2, 2);
        string month = dateTime.Month.ToString("00");
        string day = dateTime.Day.ToString("00");
        string hour = dateTime.Hour.ToString("00");
        string minute = dateTime.Minute.ToString("00");
        string second = dateTime.Second.ToString("00");
        return $"{year}_{month}_{day}_{hour}_{minute}_{second}";
    }


    IEnumerator SaveTexturePeriodically(float intervalSeconds, string filePath)
    {
        while (true)
        {
            yield return new WaitForSeconds(intervalSeconds);
            if (currSceneName != "menu")
            {
                // Create a new texture and read the current texColor into it
                Texture2D savedTexture = new Texture2D(texColor.width, texColor.height);
                savedTexture.SetPixels(texColor.GetPixels());
                savedTexture.Apply();

                // Convert the texture to a byte array
                byte[] bytes = savedTexture.EncodeToPNG();

                // Use a separate thread to write the byte array to a file in the background
                ThreadPool.QueueUserWorkItem((state) =>
                {
                    File.WriteAllBytes(filePath + "/" + currSceneName + ".png", bytes);
                    File.WriteAllBytes(filePath + "/" + currSceneName + "_" + GetFormattedDateTime() + ".png", bytes);
                });
            }
        }
    }
    
    private bool saveInProgress = false;
    private IEnumerator saveCoroutine;
    
    public void SaveTexture()
    {
        saveCoroutine = SaveTexture(SAVED_LABELS_PATH);
        StartCoroutine(saveCoroutine);
    }

    IEnumerator SaveTexture(string filePath)
    {
        if (currSceneName != "menu" && !saveInProgress)
        {
            saveInProgress = true;
            // Create a new texture and read the current texColor into it
            Texture2D savedTexture = new Texture2D(texColor.width, texColor.height);
            savedTexture.SetPixels(texColor.GetPixels());
            savedTexture.Apply();

            // Convert the texture to a byte array
            byte[] bytes = savedTexture.EncodeToPNG();

            // Use a separate thread to write the byte array to a file in the background
            ThreadPool.QueueUserWorkItem((state) =>
            {
                File.WriteAllBytes(filePath + "/" + currSceneName + ".png", bytes);
                saveInProgress = false;
            });
        }
        yield return null;
    }

    public void formPointCloud()
    {
        PointCloudLoader pointCloudLoader = GameObject.Find("Particle System").GetComponent<PointCloudLoader>();
        pcPoints = pointCloudLoader.GetLoadedPoints();
        pcColors = pointCloudLoader.GetLoadedColors();
        currSceneName = pointCloudLoader.CurrSceneName();

        Vector3[] positions = new Vector3[(int)resolution * (int)resolution];
        Color[] colors = new Color[(int)resolution * (int)resolution];


        for (int x = 0; x < (int)resolution * (int)resolution; x++)
        {
            if (x >= pcPoints.Length)
            {
                positions[x] = new Vector3(1, 1, 1);
                colors[x] = new Color(1, 1, 1, 1);
            }
            else
            {
                positions[x] = new Vector3(pcPoints[x].x, pcPoints[x].z , pcPoints[x].y);
                colors[x] = new Color(pcColors[x].x / 255, pcColors[x].y / 255, pcColors[x].z / 255, 1);
            }
        }

        SetParticles(positions, colors);
    }

    

    private void Update()
    {
        if (toUpdate)
        {
            vfx.enabled = true;
            toUpdate = false;

            vfx.Reinit();
            vfx.SetUInt(Shader.PropertyToID("particleCount"), particleCount);
            vfx.SetTexture(Shader.PropertyToID("TexColor"), texColor);
            vfx.SetTexture(Shader.PropertyToID("TexPosScale"), texPosScale);
            vfx.SetUInt(Shader.PropertyToID("resolution"), resolution);
        }
    }

    public void SetParticles(Vector3[] positions, Color[] colors)
    {
        texColor = new Texture2D((int)resolution, (int)resolution, TextureFormat.RGBAFloat, false);
        texPosScale = new Texture2D((int)resolution, (int)resolution, TextureFormat.RGBAFloat, false);

        string filePath = Path.Combine(Application.persistentDataPath, "SavedLabelsSemantic", currSceneName + ".png");
        bool loadFromFile = false;
        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            texColor.LoadImage(fileData);
            loadFromFile = true;
        }

        int texWidth = texColor.width;
        int texHeight = texColor.height;

        for (int y = 0; y < texHeight; y++)
        {
            for (int x = 0; x < texWidth; x++)
            {
                int index = x + y * texWidth;
                if (!loadFromFile)
                {
                    texColor.SetPixel(x, y, new Color(colors[index].r, colors[index].g, colors[index].b, 1));
                }
                var data = new Color(positions[index].x, positions[index].y, positions[index].z, particleSize);
                texPosScale.SetPixel(x, y, data);
            }
        }

        texColor.Apply();
        texPosScale.Apply();
        particleCount = (uint)positions.Length;
        toUpdate = true;
    }

    public void ChangeColor()
    {
        int texWidth = texColor.width;
        int texHeight = texColor.height;

        for (int y = 0; y < texHeight; y++)
        {
            for (int x = 0; x < texWidth; x++)
            {
                Color position = texPosScale.GetPixel(x, y);
                if (position.r > 0)
                {
                    texColor.SetPixel(x, y, new Color(1, 0, 0, 1));
                }
            }
        }

        texColor.Apply();
        toUpdate = true;
    }
    public void ColorPoints(Vector3 center, float radius, Color color)
    {
        int texWidth = texColor.width;
        int texHeight = texColor.height;
        Vector3 controllerPos = this.transform.InverseTransformPoint(center);
        // get the scale of the point cloud
        float scale = GetScaleRelativeToWorld();

        for (int y = 0; y < texHeight; y++)
        {
            for (int x = 0; x < texWidth; x++)
            {
                Color position = texPosScale.GetPixel(x, y);
                float distance = Vector3.Distance(controllerPos, new Vector3(position.r, position.g, position.b));
                if (distance < radius * (1 / scale))
                {
                    texColor.SetPixel(x, y, color);
                }
            }
        }

        texColor.Apply();
        vfx.SetTexture(Shader.PropertyToID("TexColor"), texColor);
    }

    float GetScaleRelativeToWorld()
    {
        Transform parentTransform = transform.parent;
        Vector3 originalScale = transform.localScale;
        // Calculate the scale relative to the world
        Vector3 worldScale = new Vector3(
                parentTransform.lossyScale.x * originalScale.x,
                parentTransform.lossyScale.y * originalScale.y,
                parentTransform.lossyScale.z * originalScale.z
            );

        // Return the magnitude of the world scale vector
        return worldScale.magnitude;
    }


}
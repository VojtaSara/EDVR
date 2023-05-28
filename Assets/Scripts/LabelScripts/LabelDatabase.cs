using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LabelDatabase : MonoBehaviour
{
    private List<GameObject>[] labels;
    private int currentScene = 0;
    public void InitializeLabels(int numOfScenes)
    {
        labels = new List<GameObject>[numOfScenes];
        for (int i = 0; i < numOfScenes; i++)
        {
            labels[i] = new List<GameObject>();
        }
    }

    // Update is called once per framev
    public void AddLabel(Vector3[] points, string type, Color labelColor)
    {
        // points are null if the controllers are currently misaligned
        if (points == null) return;

        GameObject myLine = new GameObject();
        myLine.transform.SetParent(this.transform);
        myLine.AddComponent<LineRenderer>();
        myLine.AddComponent<TextMesh>();
        TextMesh text = myLine.GetComponent<TextMesh>();
        text.text = type;
        text.transform.position = points[0];
        text.characterSize = 0.01f;

        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        //lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        Material mat = new Material(Shader.Find("Sprites/Default"));
        mat.color = labelColor; // Set color to orange
        lr.material = mat;

        lr.startWidth = 0.002f;
        lr.endWidth = 0.002f;
        lr.positionCount = 16;
        lr.useWorldSpace = false;

        for (int i = 0; i < 16; i++)
        {
            lr.SetPosition(i, myLine.transform.InverseTransformPoint(points[i]));
        }
        labels[currentScene].Add(myLine);

    }

    public void RemoveLastLabel()
    {
        if (labels[currentScene].Count > 0)
        {
            GameObject lastLabel = labels[currentScene][labels[currentScene].Count - 1];
            labels[currentScene].RemoveAt(labels[currentScene].Count - 1);
            Destroy(lastLabel);
        }
    }

    public void NextScene()
    {
        if (currentScene < labels.Length - 1)
        {
            currentScene++;
            RenderLabels(currentScene);
        }
    }

    public void PreviousScene()
    {
        if (currentScene > 0)
        {
            currentScene--;
            RenderLabels(currentScene);
        }
    }

    private void RenderLabels(int currentScene)
    {
        for (int i = 0; i < labels[currentScene].Count; i++)
        {
            GameObject label = labels[currentScene][i];
            label.SetActive(true);
        }
        for (int i = 0; i < labels.Length; i++)
        {
            if (i != currentScene)
            {
                for (int j = 0; j < labels[i].Count; j++)
                {
                    GameObject label = labels[i][j];
                    label.SetActive(false);
                }
            }
        }
    }

    private Color StringToRandomColor(string input)
    {
        int hash = 0;
        for (int i = 0; i < input.Length; i++)
        {
            hash += input[i];
        }
        System.Random random = new System.Random(hash);
        return new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
    }

    public void SaveAllLabels()
    {
        string path = Path.Combine(Application.persistentDataPath, "SavedLabels");
        Directory.CreateDirectory(path);
        for (int i = 0; i < labels.Length; i++)
        {
            string scenePath = Path.Combine(path, "Scene" + i);
            Directory.CreateDirectory(scenePath);
            for (int j = 0; j < labels[i].Count; j++)
            {
                GameObject label = labels[i][j];
                string labelPath = Path.Combine(scenePath, "Label" + j);
                Directory.CreateDirectory(labelPath);
                string labelType = label.GetComponent<TextMesh>().text;
                File.WriteAllText(Path.Combine(labelPath, "LabelType.txt"), labelType);
                LineRenderer lr = label.GetComponent<LineRenderer>();
                for (int k = 0; k < lr.positionCount; k++)
                {
                    Vector3 point = lr.GetPosition(k);
                    File.WriteAllText(Path.Combine(labelPath, "Point" + k + ".txt"), point.x + "," + point.y + "," + point.z);
                }
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Globalization;


public class PointCloudLoader : MonoBehaviour
{
    string[] filePaths;
    int currentFile = 0;
    Vector3[] loadedPoints;
    Vector3[] loadedColors;


    private void Start()
    {
        string path = Path.Combine(Application.persistentDataPath, "TrainingDataFolder");
        Directory.CreateDirectory(path);
        filePaths = Directory.GetFiles(path, "*.txt");
        Debug.Log(filePaths[0]);
        
        LabelDatabase labelDatabase = GameObject.Find("LabelDatabase").GetComponent<LabelDatabase>();
        labelDatabase.InitializeLabels(filePaths.Length);
    }


    public string[] linesOfCurrentFile()
    {
        return System.IO.File.ReadAllLines(filePaths[currentFile]);
    }

    public int numOfLinesOfCurrentFile()
    {
        string[] result = linesOfCurrentFile();
        return result.Length;
    }

    public (Vector3[], Vector3[]) ReadPointsFromTxtFile()
    {
        // Read points from txt file
        string[] lines = linesOfCurrentFile();
        Vector3[] points = new Vector3[lines.Length];
        Vector3[] colors = new Vector3[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            string[] line = lines[i].Split(' ');
            points[i] = new Vector3(
                float.Parse(line[0], CultureInfo.InvariantCulture),
                float.Parse(line[1], CultureInfo.InvariantCulture),
                float.Parse(line[2], CultureInfo.InvariantCulture));

            colors[i] = new Vector3(
                float.Parse(line[3], CultureInfo.InvariantCulture),
                float.Parse(line[4], CultureInfo.InvariantCulture),
                float.Parse(line[5], CultureInfo.InvariantCulture));
        }
        return (points, colors);
    }

    public void NextFile()
    {
        if (currentFile < filePaths.Length - 1)
        {
            currentFile++;
            TriggerPointRender();
        }
    }

    public void PreviousFile()
    {
        if (currentFile > 0)
        {
            currentFile--;
            TriggerPointRender();
        }
    }

    private void TriggerPointRender()
    {
        (loadedPoints, loadedColors) = ReadPointsFromTxtFile();
        //PointsToParticles pointsToParticles = GameObject.Find("Particle System").GetComponent<PointsToParticles>();
        //pointsToParticles.TransformIntoPosition(loadedPoints);
    } 

    public Vector3[] GetLoadedPoints()
    {
        return loadedPoints;
    }

    public Vector3[] GetLoadedColors()
    {
        return loadedColors;
    }

    public string CurrSceneName()
    {
        // get only the filename of filePaths[currentFile] without the extension
        string[] pathSplit = filePaths[currentFile].Split(Path.DirectorySeparatorChar);
        string[] nameSplit = pathSplit[pathSplit.Length - 1].Split('.');
        return nameSplit[0];
    }
}

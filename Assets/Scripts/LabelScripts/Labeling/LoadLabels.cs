using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class LoadLabels : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform buttonParent;
    public LabelBrush labelBrush;

    public List<LabelClass> labelList = new List<LabelClass>();

    private void Start()
    {
        LoadLabelData();
        CreateButtons();
    }

    private void LoadLabelData()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "Config", "labels.txt");
        StreamReader reader = new StreamReader(filePath);
        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine().Trim();
            if (!string.IsNullOrEmpty(line) && !line.StartsWith("#"))
            {
                string[] fields = line.Split(';');
                string name = fields[0].Trim('\'', ' ');
                int id = int.Parse(fields[1].Trim());

                string[] colorValues = fields[2].Trim('(', ')').Split(',');
                Color color = new Color(int.Parse(colorValues[0].Trim()) / 255f,
                                        int.Parse(colorValues[1].Trim()) / 255f,
                                        int.Parse(colorValues[2].Trim()) / 255f);
                LabelClass label = new LabelClass(name, id, color);
                labelList.Add(label);
            }
        }
        reader.Close();
    }

    private void CreateButtons()
    {
        foreach (LabelClass label in labelList)
        {
            GameObject button = Instantiate(buttonPrefab, buttonParent);
            button.GetComponentInChildren<Text>().text = label.name;
            button.GetComponent<Image>().color = label.color;
            button.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(label));
        }
    }

    private void OnButtonClick(LabelClass label)
    {
        labelBrush.selectedLabelClass = label;
    }
}

public class LabelClass
{
    public string name;
    public int id;
    public Color color;

    public LabelClass(string name, int id, Color color)
    {
        this.name = name;
        this.id = id;
        this.color = color;
    }
}
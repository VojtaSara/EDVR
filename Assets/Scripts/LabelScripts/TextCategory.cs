using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class TextCategory : MonoBehaviour
{
    public GameObject labelBrush;
    LabelBrush p;
    // Start is called before the first frame update
    void Start()
    {
        p = labelBrush.GetComponent<LabelBrush>();
    }

    // Update is called once per frame
    void Update()
    {
        // change the text of the textmesh
        if (p.selectedLabelClass != null)
        {
            GetComponent<TextMesh>().text = p.selectedLabelClass.name;
        }
    }
}

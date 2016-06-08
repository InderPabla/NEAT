using UnityEngine;
using System.Collections.Generic;

public class LineGraphDrawer : MonoBehaviour {

    public GameObject linePrefab;

    List<GameObject> lines;

    private float maxY, maxX, seperationX, seperationY, axisLineWidth, plotLineWidth;
    private Vector2 oldLocation;
    private Vector2 centerLocation;
    LineRenderer linePlot;
    int vertextCount = 2;
    void Start () {
        lines = new List<GameObject>();
        centerLocation = transform.position;
    }
	
	void Update () {
	
	}

    public void CalibrateGraph(float maxY, float maxX, float seperationX, float seperationY, float axisLineWidth, float plotLineWidth) {
        for (int i = 0; i < lines.Count; i++) {
            Destroy(lines[i]);
        }

        this.maxX = maxX;
        this.maxY = maxY;
        this.seperationX = seperationX;
        this.seperationY = seperationY;
        this.axisLineWidth = axisLineWidth;
        this.plotLineWidth = plotLineWidth;

        lines = new List<GameObject>();

        GameObject horizontal = (GameObject)Instantiate(linePrefab);
        horizontal.transform.parent = transform;
        LineRenderer lineHori = horizontal.GetComponent<LineRenderer>();
        lineHori.SetPosition(0, Vector2.zero + centerLocation);
        lineHori.SetPosition(1, new Vector2(seperationX*maxX,0f) + centerLocation);
        lineHori.SetWidth(axisLineWidth, axisLineWidth);
        lineHori.material = new Material(Shader.Find("Particles/Additive"));
        lineHori.SetColors(Color.white,Color.white);

        GameObject vertical = (GameObject)Instantiate(linePrefab);
        vertical.transform.parent = transform;
        LineRenderer lineVert = vertical.GetComponent<LineRenderer>();
        lineVert.SetPosition(0, Vector2.zero + centerLocation);
        lineVert.SetPosition(1, new Vector2(0f, seperationY * maxY) + centerLocation);
        lineVert.SetWidth(axisLineWidth, axisLineWidth);
        lineVert.material = new Material(Shader.Find("Particles/Additive"));
        lineVert.SetColors(Color.white, Color.white);

        GameObject plot = (GameObject)Instantiate(linePrefab);
        plot.transform.parent = transform;
        linePlot = plot.GetComponent<LineRenderer>();
        linePlot.SetPosition(0,centerLocation);
        linePlot.SetPosition(1, centerLocation);
        linePlot.SetWidth(plotLineWidth,plotLineWidth);
        linePlot.material = new Material(Shader.Find("Particles/Additive"));
        linePlot.SetColors(Color.red, Color.red);

        lines.Add(horizontal);
        lines.Add(vertical);
        lines.Add(plot);

        oldLocation = centerLocation;

    }

    public void PlotData(float dataPoint) {
        float y = (seperationY * dataPoint) + centerLocation.y;
        float x = seperationX + oldLocation.x;
        vertextCount++;
        linePlot.SetVertexCount(vertextCount);
        Vector2 newPosition = new Vector2(x,y);
        linePlot.SetPosition(vertextCount-1,newPosition);
        oldLocation = newPosition;

    }
}

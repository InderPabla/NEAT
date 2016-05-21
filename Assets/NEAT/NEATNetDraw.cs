using UnityEngine;
using System.Collections.Generic;

public class NEATNetDraw : MonoBehaviour {
    public GameObject linePrefab;
    public GameObject nodePrefab;

    private List<GameObject> lineList;

    private List<GameObject> nodeList;
    private Vector3[] locations;
    private Vector3 topLeft;

    // Use this for initialization
    void Start () {
        lineList = new List<GameObject>();
        nodeList = new List<GameObject>();
        topLeft = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void DrawNet(NEATNet net) {
        Clear();

        int numberOfInputs = net.GetNumberOfInputNodes();
        int numberOfOutputs = net.GetNumberOfOutputNodes();
        int numberOfNodes = net.GetNodeCount();
        int numberOfHiddens = numberOfNodes - (numberOfInputs + numberOfOutputs);
        int hiddenStartIndex = numberOfInputs + numberOfOutputs;
        locations = new Vector3[net.GetNodeCount()];
        int locationIndex = 0;

        float staryY = topLeft.y;
        for (int i = 0; i < numberOfInputs; i++) {
            Vector3 loc = new Vector3(topLeft.x, staryY, 0);
            GameObject node = (GameObject)Instantiate(nodePrefab, loc, nodePrefab.transform.rotation);
            node.GetComponent<Renderer>().material.color = Color.green;
            nodeList.Add(node);
            staryY--;

            locations[locationIndex] = loc;
            locationIndex++;
        }

        staryY = (topLeft.y - (numberOfInputs / 2f)) + (numberOfOutputs / 2f);
        for (int i = numberOfInputs; i < hiddenStartIndex; i++) {
            Vector3 loc = new Vector3(topLeft.x + 7f, staryY, 0);
            GameObject node = (GameObject)Instantiate(nodePrefab, loc, nodePrefab.transform.rotation);
            node.GetComponent<Renderer>().material.color = Color.red;
            nodeList.Add(node);
            staryY--;

            locations[locationIndex] = loc;
            locationIndex++;
        }

        float xn = 0;
        float yn = 0;
        float angle = 0;
        for (int i = hiddenStartIndex; i < numberOfNodes; i++) {
            xn = Mathf.Sin(Mathf.Deg2Rad * angle) * 2;
            yn = Mathf.Cos(Mathf.Deg2Rad * angle) * 2;

            Vector3 loc = new Vector3(xn + 3.5f + topLeft.x, (yn + topLeft.y) - numberOfInputs / 2f, 0);
            GameObject node = (GameObject)Instantiate(nodePrefab, loc, nodePrefab.transform.rotation);
            node.GetComponent<Renderer>().material.color = Color.magenta;
            nodeList.Add(node);
            angle += (360f / numberOfHiddens);

            locations[locationIndex] = loc;
            locationIndex++;
        }

        float[][] geneConnections = net.GetGeneDrawConnections();
        int colSize = geneConnections.GetLength(0);
        
        for (int i = 0; i < colSize; i++) {
            GameObject lineObj = (GameObject)Instantiate(linePrefab);
            lineList.Add(lineObj);
            LineRenderer lineRen = lineObj.GetComponent<LineRenderer>();
            lineRen.SetPosition(0, locations[(int)geneConnections[i][0]]);
            lineRen.SetPosition(1, locations[(int)geneConnections[i][1]]);
            lineRen.material = new Material(Shader.Find("Particles/Additive"));
            float size = 0.1f;
            float weight = geneConnections[i][2];
            float factor = Mathf.Abs(weight);
            Color color;

            if (weight > 0)
                color = Color.green;
            else if (weight < 0)
                color = Color.red;
            else
                color = Color.white;

            size = size * factor;
            if (size < 0.05f)
                size = 0.05f;
            if (size > 0.15f)
                size = 0.15f;

            lineRen.SetColors(color, color);
            lineRen.SetWidth(size,size);
        }
    }

    public void Clear() {
        for(int i = 0; i < lineList.Count; i++) {
            Destroy(lineList[i]);
        }
        
        for (int i = 0; i < nodeList.Count; i++) {
            Destroy(nodeList[i]);
        }

        lineList.Clear();
        nodeList.Clear();
    }
}

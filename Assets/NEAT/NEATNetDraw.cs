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
        int numberOfHiddens = numberOfNodes - (numberOfInputs+numberOfOutputs);
        int hiddenStartIndex = numberOfInputs + numberOfOutputs;

        locations = new Vector3[net.GetNodeCount()];

        float staryY = topLeft.y;
        for (int i = 0; i < numberOfInputs; i++) {
            Vector3 loc = new Vector3(topLeft.x, staryY, 0);
            GameObject node = (GameObject)Instantiate(nodePrefab, loc, nodePrefab.transform.rotation);
            nodeList.Add(node);
            staryY--;
        }

        staryY = (topLeft.y - (numberOfInputs / 2f)) + (numberOfOutputs / 2f) ;
        for (int i = numberOfInputs; i < hiddenStartIndex; i++) {
            Vector3 loc = new Vector3(topLeft.x+7f, staryY, 0);
            GameObject node = (GameObject)Instantiate(nodePrefab, loc, nodePrefab.transform.rotation);
            nodeList.Add(node);
            staryY--;
        }

        //int segmentNumber = 0;

        float xo = 0;
        float yo = 0;
        float xn = 0;
        float yn = 0;
        float angle = 0;
        for (int i = hiddenStartIndex; i < numberOfNodes; i++) {
            xn = Mathf.Sin(Mathf.Deg2Rad * angle) * 4;
            yn = Mathf.Cos(Mathf.Deg2Rad * angle) * 4;

            Vector3 loc = new Vector3(xn, yn, 0);
            GameObject node = (GameObject)Instantiate(nodePrefab, loc, nodePrefab.transform.rotation);
            nodeList.Add(node);
            angle += (360f / numberOfHiddens);
        }

        /*GameObject lineObj = (GameObject)Instantiate(linePrebaf);
        lineObjList.Add(lineObj);

        LineRenderer lineRen = lineObj.GetComponent<LineRenderer>();
        lineRen.SetColors(Color.red,Color.red);
        lineRen.SetPosition(0,Vector2.zero);
        lineRen.SetPosition(1, Vector2.up);*/

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

        //Debug.Log("CCCCC: "+lineList.Count);
    }
}

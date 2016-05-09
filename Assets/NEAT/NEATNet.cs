using System.Collections.Generic;

public class NEATNet{

    private List<NEATGene> geneList;
    private List<NEATNode> nodeList;

    private int numberOfInputs;
    private int numberOfOutputs;
    private int innovationNumber;

    public NEATNet(int innovationNumber, int numberOfInputs, int numberOfOutputs) {
        this.numberOfInputs = numberOfInputs;
        this.numberOfOutputs = numberOfOutputs;
        this.innovationNumber = innovationNumber;

        InitilizeNodes();
        InitilizeGenes();
    }

    public void InitilizeNodes() {
        nodeList = new List<NEATNode>();

        NEATNode node = null;
        for (int i = 0; i < numberOfInputs; i++) {

            if(i == (numberOfInputs - 1))
                node = new NEATNode(i,NEATNode.INPUT_BIAS_NODE);
            else
                node = new NEATNode(i, NEATNode.INPUT_NODE);

            nodeList.Add(node);
        }

        for (int i = numberOfInputs; i < numberOfInputs+numberOfOutputs; i++){
            node = new NEATNode(i, NEATNode.OUTPUT_NODE);
            nodeList.Add(node);
        }

        /*for (int i = 0; i < nodeList.Count; i++) {
            UnityEngine.Debug.Log(nodeList[i].GetNodeID()+" "+nodeList[i].GetNodeType());
        }
        UnityEngine.Debug.Log("---");*/

    }

    public void InitilizeGenes() {
        geneList = new List<NEATGene>();
        NEATGene gene = null;

        for (int i = 0; i < numberOfInputs; i++){
            for (int j = numberOfInputs; j < numberOfInputs+numberOfOutputs; j++){
                gene = new NEATGene(innovationNumber, i, j, 1f, true);
                geneList.Add(gene);
                innovationNumber++;
            }
        }
        
        /*for (int i = 0; i < geneList.Count; i++)
        {
            UnityEngine.Debug.Log(geneList[i].GetInnovation()+" "+geneList[i].GetInID() + " " + geneList[i].GetOutID());
        }*/
    }

    public void SetInputValues(float[] inputs) {
        for (int i = 0; i < numberOfInputs; i++) {
            if (nodeList[i].GetNodeType() == NEATNode.INPUT_NODE) {
                nodeList[i].SetValue(inputs[i]);
            }
            else {
                break;
            }
        }
    }

    public float[] GetAllNodeValues() {
        float[] values = new float[nodeList.Count];
        for (int i = 0; i < values.Length; i++){
            values[i] = nodeList[i].GetValue();
        }
        return values;
    }

    private float[] GetInputValues(){
        float[] values = new float[numberOfInputs];
        for (int i = 0; i < numberOfInputs; i++){
            values[i] = nodeList[i].GetValue();
        }
        return values;
    }

    private float[] GetOutputValues(){
        float[] values = new float[numberOfOutputs];
        for (int i = 0; i < numberOfOutputs; i++) {
            values[i] = nodeList[i + numberOfInputs].GetValue();
        }
        return values;
    }

    private float[] GetHiddenValues(){
        int numberOfHiddens = nodeList.Count - (numberOfInputs + numberOfOutputs);
        float[] values = new float[numberOfHiddens];
        for (int i = 0; i < numberOfHiddens; i++){
            values[i] = nodeList[i + numberOfInputs + numberOfOutputs].GetValue();
        }
        return values;
    }

    public float[] FireNet(float[] inputs){
        float[] output = new float[numberOfOutputs];

        //set input values to the input nodes
        SetInputValues(inputs);

        //feed forward reccurent net 
        float[] tempValues = GetAllNodeValues();
        int numberOfGenes = geneList.Count;

        for (int i = 0; i < numberOfGenes; i++) {
            NEATGene gene = geneList[i];
            bool on = gene.GetGeneState();
            if (on == true) {
                int inID = gene.GetInID();
                int outID = gene.GetOutID();
                float weight = gene.GetWeight();

                NEATNode outNode = nodeList[outID];

                float inNodeValue = tempValues[inID];
                float outNodeValue = tempValues[outID];

                float newOutNodeValue = outNodeValue + (inNodeValue*weight);
                outNode.SetValue(newOutNodeValue);
            }
        }

        //Activation
        for (int i = 0; i < nodeList.Count; i++) {
            nodeList[i].Activation();
        }

        // return output values 
        output = GetOutputValues();

        return output;
    }

    public void Mutate() {

    }


}
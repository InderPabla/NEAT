using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

public class NEATNet {

    private NEATConsultor consultor;

    private List<NEATGene> geneList;
    private List<NEATNode> nodeList;

    private int numberOfInputs;
    private int numberOfOutputs;
    private int[] netID = new int[2];

    private float time;
    private float netFitness;

    public NEATNet(NEATNet copy) {
        this.consultor = copy.consultor;
        this.numberOfInputs = copy.numberOfInputs;
        this.numberOfOutputs = copy.numberOfOutputs;

        CopyNodes(copy.nodeList);
        CopyGenes(copy.geneList);

        this.netID = new int[2];
        this.time = 0f;
        this.netFitness = 0f;
    }

    public NEATNet(NEATPacket packet, NEATConsultor consultor) {
        this.consultor = consultor;
        this.numberOfInputs = packet.node_inputs;
        this.numberOfOutputs = packet.node_outputs;
        
        InitilizeNodes();

        int numberOfNodes = packet.node_total;
        int numberOfgenes = packet.gene_total;
        int informationSize = NEATGene.GENE_INFORMATION_SIZE;

        NEATNode node = null;
        NEATGene gene = null;

        for (int i = numberOfInputs + numberOfOutputs; i < numberOfNodes; i++) {
            node = new NEATNode(i, NEATNode.HIDDEN_NODE);
            nodeList.Add(node);
        }

        float[] geneInformation = packet.genome.Split('_').Select(x => float.Parse(x)).ToArray();
        geneList = new List<NEATGene>();

        for (int i = 0; i < geneInformation.Length; i+=informationSize) {
            gene = new NEATGene(0, (int)geneInformation[i], (int)geneInformation[i + 1], geneInformation[i + 2], geneInformation[i + 3] == 1.0? true:false);
            geneList.Add(gene);
        }

        this.netID = new int[2];
        this.time = 0f;
        this.netFitness = 0f;
    }

    public NEATNet(NEATConsultor consultor, int[] netID, int numberOfInputs, int numberOfOutputs, float time) {
        this.consultor = consultor;
        this.netID = new int[] {netID[0], netID[1]};
        this.numberOfInputs = numberOfInputs;
        this.numberOfOutputs = numberOfOutputs;
        this.time = time;
        this.netFitness = 0f;

        InitilizeNodes();
        InitilizeGenes();
    }

    public NEATNet(NEATConsultor consultor, int numberOfInputs, int numberOfOutputs, List<NEATNode> copyNodes, List<NEATGene> copyGenes) {
        this.consultor = consultor;
        this.numberOfInputs = numberOfInputs;
        this.numberOfOutputs = numberOfOutputs;

        CopyNodes(copyNodes);
        CopyGenes(copyGenes);
        
        this.netID = new int[2];
        this.time = 0f;
        this.netFitness = 0f;
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
    }

    public void InitilizeGenes() {
        geneList = new List<NEATGene>();
        NEATGene gene = null;

        for (int i = 0; i < numberOfInputs; i++){
            for (int j = numberOfInputs; j < numberOfInputs+numberOfOutputs; j++){
                int inno = consultor.CheckGeneExistance(i,j);
                gene = new NEATGene(inno, i, j, 1f, true);
                InsertNewGene(gene);
            }
        }
    }

    public float GetNetFitness() {
        return netFitness;
    }

    public void SetNetID(int[] netID) {
        this.netID = new int[] {netID[0], netID[1]};
    }

    public void SetNetFitness(float netFitness) {
        this.netFitness = netFitness;
    }

    public void AddNetFitness(float netFitness) {
        this.netFitness += netFitness;
    }

    public int[] GetNetID() {
        return netID;
    }

    public float GetTestTime() {
        return time;
    }

    public int GetNodeCount() {
        return nodeList.Count;
    }

    public int GetGeneCount() {
        return geneList.Count;
    }

    public int GetNumberOfInputNodes() {
        return numberOfInputs;
    }

    public int GetNumberOfOutputNodes() {
        return numberOfOutputs;
    }

    public NEATConsultor GetConsultor() {
        return consultor;
    }

    public void SetTestTime(float time) {
        this.time = time;
    }

    public float[][] GetGeneDrawConnections() {
        float[][] connections = null;
        List<float[]> connectionList = new List<float[]>();
        int numberOfGenes = geneList.Count;

        for (int i = 0; i < numberOfGenes; i++) {
            NEATGene gene = geneList[i];
            if (gene.GetGeneState() == true) {
                float[] details = new float[3];
                details[0] = gene.GetInID();
                details[1] = gene.GetOutID();

                //if (gene.GetGeneState() == true)
                    details[2] = gene.GetWeight();
                //else
                    //details[2] = 0f;

                connectionList.Add(details);
            }

        }
        connections = connectionList.ToArray();
        return connections;
    }

    public string GetGenomeString() {
        string genome = "";
        int numberOfGenes = geneList.Count;

        for (int i = 0; i < numberOfGenes; i++) {
            NEATGene gene = geneList[i];
            genome += gene.GetGeneString();

            if (i < numberOfGenes - 1)  {
                genome += "_";
            }
        }
        return genome;
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

    public void CopyNodes(List<NEATNode> copyNodes) {
        nodeList = new List<NEATNode>();
        int numberOfNodes = copyNodes.Count;
        for (int i = 0; i < numberOfNodes; i++) {
            NEATNode node = new NEATNode(copyNodes[i]);
            nodeList.Add(node);
        }
    }


    public void CopyGenes(List<NEATGene> copyGenes) {
        geneList = new List<NEATGene>();
        int numberOfGenes = copyGenes.Count;
        for (int i = 0; i < numberOfGenes; i++) {
            NEATGene gene = new NEATGene(copyGenes[i]);
            geneList.Add(gene);
        }
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
        //int randomNumberT;
        //do
        //{
            //randomNumberT = Random.Range(1, 3);
            int randomNumber = Random.Range(1, 101);
            if (randomNumber <= 1)
            {
                AddConnection();
            }
            else if (randomNumber <= 2)
            {
                AddNode();
            }
        //}
        //while (randomNumberT == 1);

        //do
        //{
            //randomNumberT = Random.Range(1, 3);
            MutateWeight();
        //}
        //while (randomNumberT == 1);
    }

    public void AddConnection(){
        int randomNodeID1, randomNodeID2, inno;
        bool found = false;
        int totalAttemptsAllowed = (int)Mathf.Pow(nodeList.Count,2);

        while (totalAttemptsAllowed > 0 && found == false) {

            randomNodeID1 = Random.Range(0, nodeList.Count);
            randomNodeID2 = Random.Range(numberOfInputs, nodeList.Count);

            if (!ConnectionExists(randomNodeID1, randomNodeID2)) {
                inno = consultor.CheckGeneExistance(randomNodeID1, randomNodeID2);
                NEATGene gene = new NEATGene(inno, randomNodeID1, randomNodeID2, 1f, true);
                InsertNewGene(gene);

                found = true;
            }
            else if(nodeList[randomNodeID1].GetNodeType() > 1 && !ConnectionExists(randomNodeID2, randomNodeID1)) {
                inno = consultor.CheckGeneExistance(randomNodeID2, randomNodeID1);
                NEATGene gene = new NEATGene(inno, randomNodeID2, randomNodeID1, 1f, true);
                InsertNewGene(gene);

                found = true;
            }

            if(randomNodeID1 == randomNodeID2)
                totalAttemptsAllowed --;
            else
                totalAttemptsAllowed -= 2;

            
        }

        if (found == false) {
            AddNode();
        }
    }

    public void AddNode(){
        int firstID, secondID, thirdID, inno;
        float oldWeight;
        int randomGeneIndex = Random.Range(0, geneList.Count);
        NEATGene oldGene = geneList[randomGeneIndex];
        oldGene.SetGeneState(false);
        firstID = oldGene.GetInID();
        thirdID = oldGene.GetOutID();
        oldWeight = oldGene.GetWeight();

        NEATNode newNode = new NEATNode(nodeList.Count, NEATNode.HIDDEN_NODE);
        nodeList.Add(newNode);
        secondID = newNode.GetNodeID();

        inno = consultor.CheckGeneExistance(firstID, secondID);
        NEATGene newGene1 = new NEATGene(inno, firstID, secondID, 1f, true);

        inno = consultor.CheckGeneExistance(secondID, thirdID);
        NEATGene newGene2 = new NEATGene(inno, secondID, thirdID, oldWeight, true);

        InsertNewGene(newGene1);
        InsertNewGene(newGene2);
    }

    //-----UNUSED
    public void DeleteConnection() {
        int randomGeneIndex = Random.Range(0, geneList.Count);
        geneList.RemoveAt(randomGeneIndex);
    }

    //-----UNUSED
    public void DeleteNode() {
        int randomNodeIndex = Random.Range(numberOfInputs+numberOfOutputs, nodeList.Count);
        
        for (int i = 0; i < geneList.Count; i++) {
            NEATGene gene = geneList[i];
            int inID = gene.GetInID();
            int outID = gene.GetOutID();

            if (inID == randomNodeIndex || outID == randomNodeIndex) {
                geneList.RemoveAt(i);
            }
        }
        nodeList.RemoveAt(randomNodeIndex);
    }

    public void MutateWeight(){
        int numberOfGenes = geneList.Count;
        float weight;
        for (int i = 0; i < numberOfGenes; i++){
            NEATGene gene = geneList[i];

            int randomNumber = Random.Range(1, 101);

            if (randomNumber <= 1) {
                weight = gene.GetWeight();
                weight *= -1f;
                gene.SetWeight(weight);
            }
            else if (randomNumber <= 2) {
                weight = Random.Range(-1f,1f);
                gene.SetWeight(weight);
            }
            else if (randomNumber <= 3) {
                float factor = Random.Range(0f,1f) + 1f;
                weight = gene.GetWeight() * factor;
                gene.SetWeight(weight);
            }
            else if (randomNumber <= 4) {
                float factor = Random.Range(0f, 1f);
                weight = gene.GetWeight() * factor;
                gene.SetWeight(weight);
            }
            else if (randomNumber <= 5)
            {
                gene.SetGeneState(!gene.GetGeneState());
            }
        }

    }

    public bool ConnectionExists(int inID, int outID) {
        int numberOfGenes = geneList.Count;

        for (int i = 0; i < numberOfGenes; i++) {
            int nodeInID = geneList[i].GetInID();
            int nodeOutID = geneList[i].GetOutID();

            if (nodeInID == inID && nodeOutID == outID)
                return true;
        }

        return false;
    }

    public void ClearNodeValues() {
        int numberOfNodes = nodeList.Count;
        for (int i = 0; i < numberOfNodes; i++)
        {
            nodeList[i].SetValue(0f);
        }
    }

    public void InsertNewGene(NEATGene gene) {
        //geneList.Add(gene);
        int inno = gene.GetInnovation();
        int insertIndex = FindInnovationInsertIndex(inno);

        if (insertIndex == geneList.Count)
            geneList.Add(gene);
        else 
            geneList.Insert(insertIndex,gene);
    }

    public int FindInnovationInsertIndex(int inno) {
        int numberOfGenes = geneList.Count;
        int startIndex = 0;
        int endIndex = numberOfGenes - 1;
        if (numberOfGenes == 0)
            return 0;
        else if (numberOfGenes == 1) {
            if (inno > geneList[0].GetInnovation()) {
                return 1;
            }
            else {
                return 0;
            }
        }

        while (true) {
            int middleIndex = (endIndex + startIndex)/2;
            int middleInno = geneList[middleIndex].GetInnovation();
            if(endIndex-startIndex == 1) {
                int endInno = geneList[endIndex].GetInnovation();
                int startInno = geneList[startIndex].GetInnovation();
                if (inno < startInno)
                    return startIndex;
                else if (inno > endInno)
                    return endIndex + 1;
                else
                    return endIndex;
            }
            else if (inno > middleInno) {
                startIndex = middleIndex;
            }
            else {
                endIndex = middleIndex;
            }
        }
    }

    internal static NEATNet CreateMutateCopy(NEATNet net) {
        NEATNet copy = null;

        copy = new NEATNet(net);
        copy.Mutate();

        return copy;
    }

    internal static NEATNet Corssover (NEATNet parent1, NEATNet parent2)
    {
        NEATNet child = null;

        Hashtable geneHash = new Hashtable();

        List<NEATGene> childGeneList = new List<NEATGene>();
        List<NEATNode> childNodeList = null;

        List<NEATGene> geneList1 = parent1.geneList;
        List<NEATGene> geneList2 = parent2.geneList;

        NEATConsultor consultor = parent1.GetConsultor();

        int numberOfGenes1 = geneList1.Count;
        int numberOfGenes2 = geneList2.Count;
        int numberOfInputs = parent1.GetNumberOfInputNodes();
        int numberOfOutputs = parent1.GetNumberOfOutputNodes();

        if (parent1.GetNodeCount() > parent2.GetNodeCount()) {
            childNodeList = parent1.nodeList;
        }
        else
        {
            childNodeList = parent2.nodeList;
        }

        for (int i = 0; i < numberOfGenes1; i++) {

            geneHash.Add(geneList1[i].GetInnovation(),new NEATGene[] { geneList1[i], null});
        }

        for (int i = 0; i < numberOfGenes2; i++) {
            int innovationNumber = geneList2[i].GetInnovation();
            if (geneHash.ContainsKey(innovationNumber) == true) {
                NEATGene[] geneValue = (NEATGene[])geneHash[innovationNumber];
                geneValue[1] = geneList2[i];
                geneHash.Remove(innovationNumber);
                geneHash.Add(innovationNumber, geneValue);
            }
            else {
                geneHash.Add(innovationNumber, new NEATGene[] { null , geneList2[i] });
            }
        }

        NEATGene gene;
        int randomIndex;
        ICollection keysCol = geneHash.Keys;
        int[] keys = new int[keysCol.Count];
        keysCol.CopyTo(keys,0);
        keys = keys.OrderBy(i => i).ToArray();
        
        for (int i = 0; i < keys.Length; i++) {
            NEATGene[] geneValue = (NEATGene[])geneHash[keys[i]];

            if (geneValue[0] != null && geneValue[1] != null)  {
                randomIndex = Random.Range(0, 2);

                gene = CrossoverCopyGene(geneValue[randomIndex]);
                childGeneList.Add(gene);
            }
            else if (parent1.GetNetFitness() > parent2.GetNetFitness()) {
                if (geneValue[0] != null) {
                    gene = CrossoverCopyGene(geneValue[0]);
                    childGeneList.Add(gene);
                }
            }
            else if (parent1.GetNetFitness() < parent2.GetNetFitness()) {
                if (geneValue[1] != null) {
                    gene = CrossoverCopyGene(geneValue[1]);
                    childGeneList.Add(gene);
                }
            }
            else if (geneValue[0] != null) {
                gene = CrossoverCopyGene(geneValue[0]);
                childGeneList.Add(gene);
            }
            else if (geneValue[1] != null) {
                gene = CrossoverCopyGene(geneValue[1]);
                childGeneList.Add(gene);
            }
        }

        child = new NEATNet(consultor, numberOfInputs, numberOfOutputs, childNodeList, childGeneList);
        return child;
    }

    public static NEATGene CrossoverCopyGene(NEATGene copyGene) {
        NEATGene gene = new NEATGene(copyGene);
        int randomNumber = Random.Range(0, 4);
        if (gene.GetGeneState() == false && randomNumber == 0) {
            gene.SetGeneState(true);
        }
        return gene;
    }

    internal static bool SameSpecies(NEATNet net1, NEATNet net2) {
        NEATConsultor consultor = net1.consultor;
        List<NEATGene> geneList1 = net1.geneList;
        List<NEATGene> geneList2 = net2.geneList;

        bool done = false;

        int numberOfGenes1 = geneList1.Count;
        int numberOfGenes2 = geneList2.Count;
        int largeGenomeSize = numberOfGenes1 > numberOfGenes2 ? numberOfGenes1 : numberOfGenes2;
        int smallerGenomeSize = numberOfGenes1 > numberOfGenes2 ? numberOfGenes2 : numberOfGenes1;
        int excessGenes = 0;
        int disjointGenes = 0;
        int equalGenes = 0;
        int biggerIndex = 0;
        int smallerIndex = 0;
        int excessIndex, nonExcessIndex, excessInnovation, nonExcessInnovation;

        float disjointCoefficient = consultor.GetDisjointCoefficient();
        float excessCoefficient = consultor.GetExcessCoefficient();
        float averageWeightDifferenceCoefficient = consultor.GetAverageWeightDifferenceCoefficient();
        float deltaThreshold = consultor.GetDeltaThreshold();
        float similarity = 0;
        float averageWeightDifference = 0;

        for (smallerIndex = 0; smallerIndex < smallerGenomeSize; smallerIndex++) {
            done = false;
            while (!done) {
                NEATGene gene1, gene2;
                if (smallerGenomeSize == numberOfGenes1) {
                    gene1 = geneList1[smallerIndex];
                    gene2 = geneList2[biggerIndex];
                }
                else {
                    gene1 = geneList2[smallerIndex];
                    gene2 = geneList1[biggerIndex];
                }

                if (gene1.GetInnovation() == gene2.GetInnovation()) {
                    averageWeightDifference += Mathf.Abs(gene1.GetWeight() - gene2.GetWeight());
                    equalGenes++;
                    biggerIndex++;
                    done = true;
                }
                else if (gene1.GetInnovation() > gene2.GetInnovation()) {
                    disjointGenes++;
                    biggerIndex++;
                }
                else if (gene1.GetInnovation() < gene2.GetInnovation()) {
                    disjointGenes++;
                    done = true;
                }

                if (biggerIndex == largeGenomeSize)
                    break;
            }

            if (biggerIndex == largeGenomeSize)
                break;
        }
        
        done = false;

        if (geneList1[numberOfGenes1 - 1].GetInnovation() > geneList2[numberOfGenes2 - 1].GetInnovation()) {
            excessIndex = numberOfGenes1 - 1;
            nonExcessIndex = numberOfGenes2 - 1;
            nonExcessInnovation = geneList2[nonExcessIndex].GetInnovation();

            while (!done) {
                excessInnovation = geneList1[excessIndex].GetInnovation();
                if (excessInnovation > nonExcessInnovation) {
                    excessGenes++;
                    excessIndex--;
                }
                else {
                    done = true;
                }
            }
        }
        else if (geneList1[numberOfGenes1 - 1].GetInnovation() < geneList2[numberOfGenes2 - 1].GetInnovation()) {
            excessIndex = numberOfGenes2 - 1;
            nonExcessIndex = numberOfGenes1 - 1;
            nonExcessInnovation = geneList1[nonExcessIndex].GetInnovation();
           
            while (!done) {
                excessInnovation = geneList2[excessIndex].GetInnovation();
                if (excessInnovation > nonExcessInnovation) {
                    excessGenes++;
                    excessIndex--;
                }
                else {
                    done = true;
                }
            }
        }

        averageWeightDifference = averageWeightDifference / (float)equalGenes;
        similarity = (averageWeightDifference * averageWeightDifferenceCoefficient) + (((float)disjointGenes / (float)largeGenomeSize) * disjointCoefficient) + (((float)excessGenes / (float)largeGenomeSize) * excessCoefficient);

        /*Debug.Log("()()()()()()()()()()()()()()()()()()()()");
        Debug.Log("()()()()()()()()()()()()()()()()()()()()");
        Debug.Log("Details: " + equalGenes + " " + averageWeightDifference + " " + disjointGenes + " " + excessGenes + " " + largeGenomeSize+" "+similarity);
        int totalNormalNodes = net1.numberOfOutputs * net1.numberOfInputs;
        if (numberOfGenes1 > totalNormalNodes)
        {
            Debug.Log("--gene 1--");
            for (int i = totalNormalNodes; i < numberOfGenes1; i++)
            {
                NEATGene gene = geneList1[i];
                Debug.Log(i + " " + gene.GetInnovation() + " " + gene.GetInID() + " " + gene.GetOutID());
            }
        }

        if (numberOfGenes2 > totalNormalNodes)
        {
            Debug.Log("--gene 2--");
            for (int i = totalNormalNodes; i < numberOfGenes2; i++)
            {
                NEATGene gene = geneList2[i];
                Debug.Log(i + " " + gene.GetInnovation() + " " + gene.GetInID() + " " + gene.GetOutID());
            }
        }
        Debug.Log("~~~~~~~~");
        Debug.Log("()()()()()()()()()()()()()()()()()()()()");
        Debug.Log("()()()()()()()()()()()()()()()()()()()()");*/

        return similarity < deltaThreshold;
    }

    public void PrintDetails() {
        Debug.Log("-----------------");
        int numberOfNodes = nodeList.Count;
        for (int i = 0; i < numberOfNodes; i++) {
            NEATNode node = nodeList[i];
            Debug.Log("ID:" + node.GetNodeID() + ", Type:" + node.GetNodeType());
        }
        Debug.Log("-----------------");
        int numberOfGenes = geneList.Count;
        for (int i = 0; i < numberOfGenes; i++) {
            NEATGene gene = geneList[i];
            Debug.LogWarning("Inno " + gene.GetInnovation() + ", In:" + gene.GetInID() + ", Out:" + gene.GetOutID() + ", On:" + gene.GetGeneState() + ", Wi:" + gene.GetWeight());
        }
        Debug.Log("-----------------");
    }

}
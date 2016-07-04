using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

/// <summary>
/// Handels mutation, crossover, specification, feedforward activation and creation of neural network genome. 
/// </summary>
public class NEATNet {

    private NEATConsultor consultor; //Handles consultor genome sequence

    private List<NEATGene> geneList; //list of the genome sequence for this neural network
    private List<NEATNode> nodeList; //list of nodes for this neural network

    private int numberOfInputs; //Number of input perceptrons of neural network (including bias)
    private int numberOfOutputs; //Number of output perceptrons 
    private int[] netID = new int[2]; //ID of this neural network

    private float time; //time to run test on this neural network
    private float timeLived; //time the neural network actually lived in the test enviroment
    private float netFitness; //fitness of this neural network

    /// <summary>
    /// Creating neural network structure from deep copying another network
    /// </summary>
    /// <param name="copy">Neural network to deep copy</param>
    public NEATNet(NEATNet copy) {
        this.consultor = copy.consultor; //shallow copy consultor
        this.numberOfInputs = copy.numberOfInputs; //copy number of inputs
        this.numberOfOutputs = copy.numberOfOutputs; //copy number of outputs

        CopyNodes(copy.nodeList); //deep copy node list
        CopyGenes(copy.geneList); //deep copy gene list

        this.netID = new int[2]; //reset ID
        this.time = 0f; //reset time
        this.netFitness = 0f; //reset fitness
        this.timeLived = 0f; //reset time lived
    }

    /// <summary>
    /// Creating neural network structure using neat packet from database
    /// </summary>
    /// <param name="packet">Neat packet received from database</param>
    /// <param name="consultor">Consultor with master genome and specification information</param>
    public NEATNet(NEATPacket packet, NEATConsultor consultor) {
        this.consultor = consultor; //shallow copy consultor
        this.numberOfInputs = packet.node_inputs; //copy number of inputs
        this.numberOfOutputs = packet.node_outputs; //copy number of outputs

        int numberOfNodes = packet.node_total; //number of nodes in the network from database
        int numberOfgenes = packet.gene_total; //number of genes in the network from database
        int informationSize = NEATGene.GENE_INFORMATION_SIZE; //size of genome information

        geneList = new List<NEATGene>(); //create an empty gene list

        InitilizeNodes(); //initialize initial nodes

        for (int i = numberOfInputs + numberOfOutputs; i < numberOfNodes; i++) { //run through the left over nodes, since (numberOfInputs + numberOfOutputs) where created by initilize node method
            NEATNode node = new NEATNode(i, NEATNode.HIDDEN_NODE); //create node with index i as id and will be hidden node
            nodeList.Add(node); //add node to node list
        }

        float[] geneInformation = packet.genome.Split('_').Select(x => float.Parse(x)).ToArray(); //using Linq libary and delimiters, parse and spilt string genome from neat packet into float array

        for (int i = 0; i < geneInformation.Length; i+=informationSize) { //run through all gene information, 4 information make up 1 gene, thus increment by 4
            int inno = this.consultor.CheckGeneExistance((int)geneInformation[i], (int)geneInformation[i + 1]); //check if this gene exists in the consultor
            NEATGene gene = new NEATGene(inno, (int)geneInformation[i], (int)geneInformation[i + 1], geneInformation[i + 2], geneInformation[i + 3] == 1.0? true:false); //create gene
            geneList.Add(gene); //add gene to the gene list
        }

        this.netID = new int[2]; //reset ID
        this.time = 0f; //reset time
        this.netFitness = 0f; //reset fitness
        this.timeLived = 0f; //reset time lived
    }

    /// <summary>
    /// Create fresh network structure (every input connect to every output) from provided parameters
    /// </summary>
    /// <param name="consultor">Consultor with master genome and specification information</param>
    /// <param name="netID">ID of the network</param>
    /// <param name="numberOfInputs">Number of input perceptrons</param>
    /// <param name="numberOfOutputs">Number of output perceptrons</param>
    /// <param name="time">Time to test the network</param>
    public NEATNet(NEATConsultor consultor, int[] netID, int numberOfInputs, int numberOfOutputs, float time) {
        this.consultor = consultor; //shallow copy consultor
        this.netID = new int[] {netID[0], netID[1]}; //copy ID
        this.numberOfInputs = numberOfInputs; //copy number of inputs
        this.numberOfOutputs = numberOfOutputs; //copy number of outputs
        this.time = time; //copy time to test

        this.netFitness = 0f; //reset net fitness
        this.timeLived = 0f; //reset time lived

        InitilizeNodes(); //initialize initial nodes
        InitilizeGenes(); //initialize initial gene sequence
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="consultor"></param>
    /// <param name="numberOfInputs"></param>
    /// <param name="numberOfOutputs"></param>
    /// <param name="copyNodes"></param>
    /// <param name="copyGenes"></param>
    public NEATNet(NEATConsultor consultor, int numberOfInputs, int numberOfOutputs, List<NEATNode> copyNodes, List<NEATGene> copyGenes) {
        this.consultor = consultor;
        this.numberOfInputs = numberOfInputs;
        this.numberOfOutputs = numberOfOutputs;

        CopyNodes(copyNodes);
        CopyGenes(copyGenes);
        
        this.netID = new int[2];
        this.time = 0f;
        this.netFitness = 0f;
        this.timeLived = 0f;
    }

    /// <summary>
    /// 
    /// </summary>
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

    /// <summary>
    /// 
    /// </summary>
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

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public float GetNetFitness() {
        return netFitness;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public float GetTimeLived() {
        return timeLived;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="netID"></param>
    public void SetNetID(int[] netID) {
        this.netID = new int[] {netID[0], netID[1]};
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="netFitness"></param>
    public void SetNetFitness(float netFitness) {
        this.netFitness = netFitness;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="netFitness"></param>
    public void AddNetFitness(float netFitness) {
        this.netFitness += netFitness;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="timeLived"></param>
    public void SetTimeLived(float timeLived) {
        this.timeLived = timeLived;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="timeLived"></param>
    public void AddTimeLived(float timeLived) {
        this.timeLived += timeLived;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int[] GetNetID() {
        return netID;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public float GetTestTime() {
        return time;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetNodeCount() {
        return nodeList.Count;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetGeneCount() {
        return geneList.Count;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetNumberOfInputNodes() {
        return numberOfInputs;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetNumberOfOutputNodes() {
        return numberOfOutputs;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public NEATConsultor GetConsultor() {
        return consultor;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    public void SetTestTime(float time) {
        this.time = time;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public float[][] GetGeneDrawConnections() {
        float[][] connections = null;
        List<float[]> connectionList = new List<float[]>();
        int numberOfGenes = geneList.Count;

        for (int i = 0; i < numberOfGenes; i++) {
            NEATGene gene = geneList[i];
            //if (gene.GetGeneState() == true) {
                float[] details = new float[3];
                details[0] = gene.GetInID();
                details[1] = gene.GetOutID();

                if (gene.GetGeneState() == true)
                    details[2] = gene.GetWeight();
                else
                    details[2] = 0f;

                connectionList.Add(details);
            //}

        }
        connections = connectionList.ToArray();
        return connections;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputs"></param>
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

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public float[] GetAllNodeValues() {
        float[] values = new float[nodeList.Count];

        for (int i = 0; i < values.Length; i++){
            values[i] = nodeList[i].GetValue();
        }
        return values;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private float[] GetInputValues(){
        float[] values = new float[numberOfInputs];

        for (int i = 0; i < numberOfInputs; i++){
            values[i] = nodeList[i].GetValue();
        }
        return values;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private float[] GetOutputValues(){
        float[] values = new float[numberOfOutputs];

        for (int i = 0; i < numberOfOutputs; i++) {
            values[i] = nodeList[i + numberOfInputs].GetValue();
        }
        return values;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private float[] GetHiddenValues(){
        int numberOfHiddens = nodeList.Count - (numberOfInputs + numberOfOutputs);
        float[] values = new float[numberOfHiddens];

        for (int i = 0; i < numberOfHiddens; i++){
            values[i] = nodeList[i + numberOfInputs + numberOfOutputs].GetValue();
        }
        return values;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="copyNodes"></param>
    public void CopyNodes(List<NEATNode> copyNodes) {
        nodeList = new List<NEATNode>();
        int numberOfNodes = copyNodes.Count;

        for (int i = 0; i < numberOfNodes; i++) {
            NEATNode node = new NEATNode(copyNodes[i]);
            nodeList.Add(node);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="copyGenes"></param>
    public void CopyGenes(List<NEATGene> copyGenes) {
        geneList = new List<NEATGene>();
        int numberOfGenes = copyGenes.Count;

        for (int i = 0; i < numberOfGenes; i++) {
            NEATGene gene = new NEATGene(copyGenes[i]);
            geneList.Add(gene);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputs"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 
    /// </summary>
    public void Mutate() {
        int randomNumber = Random.Range(1, 101);
        int chance = 25;

        if (randomNumber <= chance) {
            AddConnection();
        }
        else if (randomNumber <= (chance+chance)) {
            AddNode();
        }
        MutateWeight();
    }

    /// <summary>
    /// 
    /// </summary>
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

    /// <summary>
    /// 
    /// </summary>
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
    /// <summary>
    /// 
    /// </summary>
    public void DeleteConnection() {
        int randomGeneIndex = Random.Range(0, geneList.Count);
        geneList.RemoveAt(randomGeneIndex);
    }

    //-----UNUSED
    /// <summary>
    /// 
    /// </summary>
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

    /// <summary>
    /// 
    /// </summary>
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
            else if (randomNumber <= 5) {
                gene.SetGeneState(!gene.GetGeneState());
            }
        }

    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="inID"></param>
    /// <param name="outID"></param>
    /// <returns></returns>
    public bool ConnectionExists(int inID, int outID) {
        int numberOfGenes = geneList.Count;

        for (int i = 0; i < numberOfGenes; i++) {
            int nodeInID = geneList[i].GetInID();
            int nodeOutID = geneList[i].GetOutID();

            if (nodeInID == inID && nodeOutID == outID) {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    public void ClearNodeValues() {
        int numberOfNodes = nodeList.Count;

        for (int i = 0; i < numberOfNodes; i++) {
            nodeList[i].SetValue(0f);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gene"></param>
    public void InsertNewGene(NEATGene gene) {
        int inno = gene.GetInnovation();
        int insertIndex = FindInnovationInsertIndex(inno);

        if (insertIndex == geneList.Count) {
            geneList.Add(gene);
        }
        else {
            geneList.Insert(insertIndex, gene);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inno"></param>
    /// <returns></returns>
    public int FindInnovationInsertIndex(int inno) {
        int numberOfGenes = geneList.Count;
        int startIndex = 0;
        int endIndex = numberOfGenes - 1;

        if (numberOfGenes == 0) {
            return 0;
        }
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="net"></param>
    /// <returns></returns>
    internal static NEATNet CreateMutateCopy(NEATNet net) {
        NEATNet copy = null;

        copy = new NEATNet(net);
        copy.Mutate();

        return copy;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parent1"></param>
    /// <param name="parent2"></param>
    /// <returns></returns>
    internal static NEATNet Corssover (NEATNet parent1, NEATNet parent2) {
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
            int state = -1;
            if (geneValue[0] != null && geneValue[1] != null)  {
                randomIndex = Random.Range(0, 2);

                if (geneValue[0].GetGeneState() == true && geneValue[1].GetGeneState() == true) {
                    state = 0;
                }
                else if (geneValue[0].GetGeneState() == false && geneValue[1].GetGeneState() == false) {
                    state = 1;
                }
                else {
                    state = 2;
                }
                gene = CrossoverCopyGene(geneValue[randomIndex],state);
                childGeneList.Add(gene);
            }
            else if (parent1.GetNetFitness() > parent2.GetNetFitness()) {
                if (geneValue[0] != null) {
                    if (geneValue[0].GetGeneState() == true) {
                        state = 3;
                    }
                    else {
                        state = 4;
                    }
                    gene = CrossoverCopyGene(geneValue[0],state);
                    childGeneList.Add(gene);
                }
            }
            else if (parent1.GetNetFitness() < parent2.GetNetFitness()) {
                if (geneValue[1] != null) {
                    if (geneValue[1].GetGeneState() == true) {
                        state = 3;
                    }
                    else {
                        state = 4;
                    }
                    gene = CrossoverCopyGene(geneValue[1],state);
                    childGeneList.Add(gene);
                }
            }
            else if (geneValue[0] != null) {
                if (geneValue[0].GetGeneState() == true){
                    state = 3;
                }
                else {
                    state = 4;
                }
                gene = CrossoverCopyGene(geneValue[0], state);
                childGeneList.Add(gene);
            }
            else if (geneValue[1] != null) {
                if (geneValue[1].GetGeneState() == true) {
                    state = 3;
                }
                else {
                    state = 4;
                }
                gene = CrossoverCopyGene(geneValue[1],state);
                childGeneList.Add(gene);
            }
        }

        child = new NEATNet(consultor, numberOfInputs, numberOfOutputs, childNodeList, childGeneList);
        return child;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="copyGene"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public static NEATGene CrossoverCopyGene(NEATGene copyGene, int state) {
        NEATGene gene = new NEATGene(copyGene);


       /* int randomNrandomNumber = Random.Range(0, 5);
        if (gene.GetGeneState() == false && 5 == 0) {
            gene.SetGeneState(true);
        }*/

        if (state == 1) {
            int randomNumber = Random.Range(0, 11);
            if (randomNumber == 0) {
                gene.SetGeneState(false);
            }
        }
        else if (state == 2) {
            int randomNumber = Random.Range(0, 11);
            if (randomNumber == 0) {
                gene.SetGeneState(true);
            }
        }
        else {
            int randomNumber = Random.Range(0, 5);
            if (randomNumber == 0) {
                gene.SetGeneState(!gene.GetGeneState());
            }
        }

        return gene;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="net1"></param>
    /// <param name="net2"></param>
    /// <returns></returns>
    internal static bool SameSpeciesV2(NEATNet net1, NEATNet net2) {
        Hashtable geneHash = new Hashtable();
        NEATConsultor consultor = net1.consultor;
        NEATGene[] geneValue;

        List<NEATGene> geneList1 = net1.geneList;
        List<NEATGene> geneList2 = net2.geneList;

        ICollection keysCol;
        int[] keys;

        int numberOfGenes1 = geneList1.Count;
        int numberOfGenes2 = geneList2.Count;
        int largerGenomeSize = numberOfGenes1 > numberOfGenes2 ? numberOfGenes1 : numberOfGenes2;
        int excessGenes = 0;
        int disjointGenes = 0;
        int equalGenes = 0;

        float disjointCoefficient = consultor.GetDisjointCoefficient();
        float excessCoefficient = consultor.GetExcessCoefficient();
        float averageWeightDifferenceCoefficient = consultor.GetAverageWeightDifferenceCoefficient();
        float deltaThreshold = consultor.GetDeltaThreshold();
        float similarity = 0;
        float averageWeightDifference = 0;

        bool foundAllExcess = false;
        bool isFirstGeneExcess = false;

        for (int i = 0; i < geneList1.Count; i++) {
            int innovation = geneList1[i].GetInnovation();
            geneValue = new NEATGene[] {geneList1[i], null};
            geneHash.Add(innovation, geneValue);
        }

        for (int i = 0; i < geneList2.Count; i++) {
            int innovation = geneList2[i].GetInnovation();

            if (!geneHash.ContainsKey(innovation)) {
                geneValue = new NEATGene[] {null, geneList2[i]};
                geneHash.Add(innovation, geneValue);
            }
            else {
                geneValue = (NEATGene[]) geneHash[innovation];
                geneValue[1] = geneList2[i];
            }
        }

        keysCol = geneHash.Keys;
        keys = new int[keysCol.Count];
        keysCol.CopyTo(keys, 0);
        keys = keys.OrderBy(i => i).ToArray();

        for (int i = keys.Length-1; i >= 0; i--) {
            geneValue = (NEATGene[])geneHash[keys[i]];
            if (foundAllExcess == false) {
                if (i == keys.Length - 1 && geneValue[1] == null) {
                    isFirstGeneExcess = true;
                }

                if (isFirstGeneExcess == true && geneValue[1] == null) {
                    excessGenes++;
                }
                else if (isFirstGeneExcess == false && geneValue[0] == null) {
                    excessGenes++;
                }
                else {
                    foundAllExcess = true;
                }

            }

            if(foundAllExcess == true){ 
                if (geneValue[0] != null && geneValue[1] != null) {
                    equalGenes++;
                    averageWeightDifference += Mathf.Abs(geneValue[0].GetWeight() - geneValue[1].GetWeight());
                }
                else {
                    disjointGenes++;
                }
            }
        }

        averageWeightDifference = averageWeightDifference / (float)equalGenes;
        similarity = (averageWeightDifference * averageWeightDifferenceCoefficient) +
                     (((float)disjointGenes * disjointCoefficient) / (float)largerGenomeSize) +
                     (((float)excessGenes * excessCoefficient) / (float)largerGenomeSize);

        return similarity<=deltaThreshold;
    }

    /// <summary>
    /// 
    /// </summary>
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
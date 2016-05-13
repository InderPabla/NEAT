﻿using System.Collections.Generic;
using UnityEngine;

public class NEATNet{

    private List<NEATGene> geneList;
    private List<NEATNode> nodeList;

    private int numberOfInputs;
    private int numberOfOutputs;
    private int innovationNumber;
    private int netID;

    private float time;
    private float netFitness;

    public NEATNet(NEATNet copy) {
        this.netID = copy.netID;
        this.numberOfInputs = copy.numberOfInputs;
        this.numberOfOutputs = copy.numberOfOutputs;
        this.innovationNumber = copy.innovationNumber;
        this.time = copy.time;
        this.netFitness = 0f;

        nodeList = new List<NEATNode>();
        geneList = new List<NEATGene>();

        int numberOfNodes = copy.nodeList.Count;
        for (int i = 0; i < numberOfNodes; i++) {
            NEATNode node = new NEATNode(copy.nodeList[i]);
            nodeList.Add(node);
        }

        int numberOfGenes = copy.geneList.Count;
        for (int i = 0; i < numberOfGenes; i++) {
            NEATGene gene = new NEATGene(copy.geneList[i]);
            geneList.Add(gene);
        }
    }

    public NEATNet(int netID, int innovationNumber, int numberOfInputs, int numberOfOutputs, float time) {
        this.netID = netID;
        this.numberOfInputs = numberOfInputs;
        this.numberOfOutputs = numberOfOutputs;
        this.innovationNumber = innovationNumber;
        this.time = time;
        this.netFitness = 0f;

        InitilizeNodes();
        InitilizeGenes();

        Mutate();
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
                gene = new NEATGene(innovationNumber, i, j, 1f, true);
                geneList.Add(gene);
                innovationNumber++;
            }
        }
    }

    public float GetNetFitness() {
        return netFitness;
    }

    public void SetNetID(int netID) {
        this.netID = netID;
    }

    public void SetNetFitness(float netFitness) {
        this.netFitness = netFitness;
    }

    public void AddNetFitness(float netFitness) {
        this.netFitness += netFitness;
    }

    public int GetNetID() {
        return netID;
    }

    public float GetTestTime() {
        return time;
    }

    public int GetNodeCount()
    {
        return nodeList.Count;
    }

    public int GetGeneCount()
    {
        return geneList.Count;
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
        int randomNumberT;
        do
        {
            randomNumberT = Random.Range(1, 3);
            int randomNumber = Random.Range(1, 101);
            if (randomNumber <= 5)
            {
                AddConnection();
            }
            else if (randomNumber <= 10)
            {
                AddNode();
            }
        }
        while (randomNumberT == 1);

        do
        {
            randomNumberT = Random.Range(1, 3);
            MutateWeight();
        }
        while (randomNumberT == 1);
    }

    public void AddConnection(){
        int randomNodeID1, randomNodeID2;
        bool found = false;
        int totalAttemptsAllowed = (int)Mathf.Pow(nodeList.Count,2);

        while (totalAttemptsAllowed > 0 && found == false) {
           
            randomNodeID1 = Random.Range(numberOfInputs, nodeList.Count);
            randomNodeID2 = Random.Range(numberOfInputs, nodeList.Count);

            if (!ConnectionExists(randomNodeID1, randomNodeID2)) {
                NEATGene gene = new NEATGene(innovationNumber, randomNodeID1, randomNodeID2, 1f, true);
                geneList.Add(gene);
                innovationNumber++;
                found = true;
            }
            else if(!ConnectionExists(randomNodeID2, randomNodeID1)) {
                NEATGene gene = new NEATGene(innovationNumber, randomNodeID2, randomNodeID1, 1f, true);
                geneList.Add(gene);
                innovationNumber++;
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
        int firstID, secondID, thirdID;
        float oldWeight;
        int randomGeneIndex = Random.Range(0, geneList.Count);

        NEATGene oldGene = geneList[randomGeneIndex];
        oldGene.SetGeneState(false);
        firstID = oldGene.GetInID();
        thirdID = oldGene.GetOutID();
        oldWeight = oldGene.GetWeight(); 
        
        NEATNode newNode = new NEATNode(nodeList.Count,NEATNode.HIDDEN_NODE);
        nodeList.Add(newNode);
        secondID = newNode.GetNodeID();
        
        NEATGene newGene1 = new NEATGene(innovationNumber,firstID,secondID,1f,true);
        innovationNumber++;

        NEATGene newGene2 = new NEATGene(innovationNumber, secondID, thirdID, oldWeight, true);
        innovationNumber++;

        geneList.Add(newGene1);
        geneList.Add(newGene2);
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

    internal static NEATNet CreateMutateCopy(NEATNet net)
    {
        NEATNet copy = null;

        copy = new NEATNet(net);
        copy.Mutate();

        return copy;
    }

}
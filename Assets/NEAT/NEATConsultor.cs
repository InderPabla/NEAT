﻿using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Consultor is the master genome of avaliable to all NEATNet's. 
/// The main job of conultor is to keep track of new genes as they are discovered and increment global innovation number when a new gene is found.
/// Consultor is extremely important as it helps identify the history of genome based on innovation number. 
/// Consultor also keeps coefficient information to calculate disparity between 2 networks.
/// </summary>
public class NEATConsultor  {

    private float deltaThreshold; //threshold value that determines whether 2 neural networks belong to the same species
    private float disjointCoefficient; //coefficient effect of disjoint genes  
    private float excessCoefficient; //coefficient effect of  excess genes
    private float averageWeightDifferenceCoefficient; //coefficient effect of average weight difference between equal genes

    private int innovationNumber = 0; //initial innovation number of 0

    private int numberOfInputs; //number of inputs in the neural network
    private int numberOfOutputs; //number of outputs in the neural network

    private List<NEATGene> geneList; //consultor gene list

    /// <summary>
    /// Creating consultor structure NEAT packet retrived from database and coefficient information from UI
    /// </summary>
    /// <param name="packet">NEATPacket retrieved from database</param>
    /// <param name="deltaThreshold">Delta threshold to set</param>
    /// <param name="disjointCoefficient">Disjoint coefficient to set</param>
    /// <param name="excessCoefficient">Excess coefficient to set</param>
    /// <param name="averageWeightDifferenceCoefficient">Averange weight difference coefficient to set</param>
    public NEATConsultor(NEATPacket packet, float deltaThreshold, float disjointCoefficient, float excessCoefficient, float averageWeightDifferenceCoefficient) {

        this.numberOfInputs = packet.node_inputs; //get number of inputs from packet
        this.numberOfOutputs = packet.node_outputs; //get number of outputs from packet

        //copy thresholdes and coefficients
        this.deltaThreshold = deltaThreshold;  
        this.disjointCoefficient = disjointCoefficient;
        this.excessCoefficient = excessCoefficient;
        this.averageWeightDifferenceCoefficient = averageWeightDifferenceCoefficient;

        int informationSize = NEATGene.GENE_INFORMATION_SIZE; //get number of gene information size 
        float[] geneInformation = packet.consultor_genome.Split('_').Select(x => float.Parse(x)).ToArray(); //using Linq libary and delimiters, parse and spilt string genome from neat packet into float array
        geneList = new List<NEATGene>();

        
        for (int i = 0; i < geneInformation.Length; i += informationSize) {
            NEATGene gene = new NEATGene(innovationNumber, (int)geneInformation[i], (int)geneInformation[i + 1], 1f, true);
            geneList.Add(gene);
            innovationNumber++;
        }
    }

    /// <summary>
    /// Creating consultor structure from scratch with details given from UI
    /// </summary>
    /// <param name="numberOfInputs">Number of input nodes in the neural network</param>
    /// <param name="numberOfOutputs">Number of outputs nodes in the neural network</param>
    /// <param name="deltaThreshold">Delta threshold to set</param>
    /// <param name="disjointCoefficient">Disjoint coefficient to set</param>
    /// <param name="excessCoefficient">Excess coefficient to set</param>
    /// <param name="averageWeightDifferenceCoefficient">Averange weight difference coefficient to set</param>
    public NEATConsultor(int numberOfInputs, int numberOfOutputs, float deltaThreshold, float disjointCoefficient, float excessCoefficient, float averageWeightDifferenceCoefficient) {
        this.numberOfInputs = numberOfInputs;
        this.numberOfOutputs = numberOfOutputs;

        this.deltaThreshold = deltaThreshold;
        this.disjointCoefficient = disjointCoefficient;
        this.excessCoefficient = excessCoefficient;
        this.averageWeightDifferenceCoefficient = averageWeightDifferenceCoefficient;

        geneList = new List<NEATGene>();

        InitilizeGenome();
    }

    /// <summary>
    /// 
    /// </summary>
    private void InitilizeGenome() {
        geneList = new List<NEATGene>();

        for (int i = 0; i < numberOfInputs; i++) {
            for (int j = numberOfInputs; j < numberOfInputs + numberOfOutputs; j++) {
                AddNewGene(innovationNumber, i, j);
                innovationNumber++;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inNodeID"></param>
    /// <param name="outNodeID"></param>
    /// <returns></returns>
    public int CheckGeneExistance(int inNodeID, int outNodeID) {
        NEATGene gene = null;
        int oldInnovationNumber = innovationNumber;
        int numberOfGenes = geneList.Count;

        for (int i = 0; i < numberOfGenes; i++) {
            gene = geneList[i];
            int inID = gene.GetInID();
            int outID = gene.GetOutID();
            if (inID == inNodeID && outID == outNodeID) {
                return gene.GetInnovation();
            }
        }

        AddNewGene(innovationNumber, inNodeID, outNodeID);
        innovationNumber++;

        return oldInnovationNumber;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inno"></param>
    /// <param name="inNodeID"></param>
    /// <param name="outNodeID"></param>
    public void AddNewGene(int inno, int inNodeID, int outNodeID) {
        NEATGene gene = new NEATGene(inno, inNodeID, outNodeID, 1f, true);
        geneList.Add(gene);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="deltaThreshold"></param>
    public void SetDeltaThreshold(float deltaThreshold) {
        this.deltaThreshold = deltaThreshold;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public float GetDisjointCoefficient() {
        return disjointCoefficient;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public float GetExcessCoefficient() {
        return excessCoefficient;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public float GetAverageWeightDifferenceCoefficient() {
        return averageWeightDifferenceCoefficient;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public float GetDeltaThreshold() {
        return deltaThreshold;
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
    public string GetGenomeString() {
        string genome = ""; //
        int numberOfGenes = geneList.Count;

        for (int i = 0; i < numberOfGenes; i++) {
            NEATGene gene = geneList[i];
            genome += gene.GetGeneString();

            if (i < numberOfGenes - 1) {
                genome += "_";
            }
        }
        return genome;
    }

}
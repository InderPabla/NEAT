using System.Collections.Generic;

public class NEATConsultor  {
    private float disjointCoefficient;
    private float excessCoefficient;
    private float averageWeightDifferenceCoefficient;

    private int innovationNumber = 0;

    private int numberOfInputs;
    private int numberOfOutputs;

    private List<NEATGene> geneList;

    public NEATConsultor(int numberOfInputs, int numberOfOutputs, float disjointCoefficient, float excessCoefficient, float averageWeightDifferenceCoefficient) {
        this.numberOfInputs = numberOfInputs;
        this.numberOfOutputs = numberOfOutputs;

        this.disjointCoefficient = disjointCoefficient;
        this.excessCoefficient = excessCoefficient;
        this.averageWeightDifferenceCoefficient = averageWeightDifferenceCoefficient;

        geneList = new List<NEATGene>();

        InitilizeGenome();
    }

    private void InitilizeGenome() {
        geneList = new List<NEATGene>();

        for (int i = 0; i < numberOfInputs; i++) {
            for (int j = numberOfInputs; j < numberOfInputs + numberOfOutputs; j++) {
                AddNewGene(innovationNumber, i, j);
                innovationNumber++;
            }
        }
    }

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

    public void AddNewGene(int inno, int inNodeID, int outNodeID) {
        NEATGene gene = new NEATGene(inno, inNodeID, outNodeID, 1f, true);
        geneList.Add(gene);
    }

    public float GetDisjointCoefficient() {
        return disjointCoefficient;
    }

    public float GetExcessCoefficient() {
        return excessCoefficient;
    }

    public float GetAverageWeightDifferenceCoefficient() {
        return averageWeightDifferenceCoefficient;
    }

    public int GetGeneCount() {
        return geneList.Count;
    }

    public string GetGenomeString()
    {
        string genome = "";
        int numberOfGenes = geneList.Count;

        for (int i = 0; i < numberOfGenes; i++)
        {
            NEATGene gene = geneList[i];
            genome += gene.GetGeneString();

            if (i < numberOfGenes - 1)
            {
                genome += "_";
            }
        }
        return genome;
    }

}

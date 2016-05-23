using System.Collections.Generic;

public class NEATConsultor  {
    public int innovationNumber = 0;

    private int numberOfInputs;
    private int numberOfOutputs;

    private List<NEATGene> geneList;

    public NEATConsultor(int numberOfInputs, int numberOfOutputs) {
        this.numberOfInputs = numberOfInputs;
        this.numberOfOutputs = numberOfOutputs;

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


	
}

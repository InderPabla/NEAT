using System.Collections.Generic;

public class NEATNet{

    private List<NEATGene> geneList;
    private List<NEATNode> nodeList;

    private int numberOfInputs;
    private int numberOfOutputs;

    public NEATNet(int numberOfInputs, int numberOfOutputs) {
        this.numberOfInputs = numberOfInputs;
        this.numberOfOutputs = numberOfOutputs;
    }

    public void InitilizeNodes() {
        NEATNode node = null;
        for (int i = 0; i < numberOfInputs; i++) {
            if(i == numberOfInputs - 1)
            node = new NEATNode(i,NEATNode.INPUT_NODE);
            nodeList.Add(node);
        }

    }
}



using System.Collections.Generic;

public class NEATConsultor  {
    public int innovationNumber = 0;

    private int numberOfInputs;
    private int numberOfOutputs;

    private List<NEATGene> genome;

    public NEATConsultor(int numberOfInputs, int numberOfOutputs) {
        this.numberOfInputs = numberOfInputs;
        this.numberOfOutputs = numberOfOutputs;

        genome = new List<NEATGene>();

        InitilizeGenome();
    }


	
}

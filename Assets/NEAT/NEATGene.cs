
public class NEATGene {

    private int inID;
    private int outID;

    private float weight;

    private bool state;

    public NEATGene(int inID, int outID, float weight, bool state) {
        this.inID = inID;
        this.outID = outID;
        this.weight = weight;
        this.state = state;
    }

}

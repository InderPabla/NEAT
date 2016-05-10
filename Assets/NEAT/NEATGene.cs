
public class NEATGene {
    private int inno;

    private int inID;
    private int outID;

    private float weight;

    private bool on;

    public NEATGene(int inno, int inID, int outID, float weight, bool on) {
        this.inno = inno;
        this.inID = inID;
        this.outID = outID;
        this.weight = weight;
        this.on = on;
    }

    public int GetInID() {
        return inID;
    }

    public int GetOutID() {
        return outID;
    }

    public int GetInnovation(){
        return inno;
    }

    public float GetWeight(){
        return weight;
    }

    public bool GetGeneState() {
        return on;
    }

    public void SetGeneState(bool on) {
        this.on = on;
    }

    public void SetWeight(float weight) {
        this.weight = weight;
    }
}

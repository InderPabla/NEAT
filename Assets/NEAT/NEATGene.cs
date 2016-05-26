using System;

public class NEATGene : IEquatable<NEATGene>{

    public const int GENE_INFORMATION_SIZE = 4;

    private int inno;

    private int inID;
    private int outID;

    private float weight;

    private bool on;

    public NEATGene(NEATGene copy)
    {
        this.inno = copy.inno;
        this.inID = copy.inID;
        this.outID = copy.outID;
        this.weight = copy.weight;
        this.on = copy.on;
    }
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

    public string GetGeneString() {
        string gene = inID + "_" + outID + "_" + weight + "_";

        if (on == true) {
            gene += 1;
        }
        else {
            gene += 0;
        }

        return gene;
    }

    public bool Equals(NEATGene other) {
        if (other == null) {
            return false;
        }

        if (inID == other.inID && outID == other.outID) {
            return true;
        }

        return false;
    }
}

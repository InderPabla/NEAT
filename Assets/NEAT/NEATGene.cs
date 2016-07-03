using System;

/// <summary>
/// 
/// </summary>
public class NEATGene : IEquatable<NEATGene>{

    public const int GENE_INFORMATION_SIZE = 4;

    private int inno;

    private int inID;
    private int outID;

    private float weight;

    private bool on;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="copy"></param>
    public NEATGene(NEATGene copy)
    {
        this.inno = copy.inno;
        this.inID = copy.inID;
        this.outID = copy.outID;
        this.weight = copy.weight;
        this.on = copy.on;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inno"></param>
    /// <param name="inID"></param>
    /// <param name="outID"></param>
    /// <param name="weight"></param>
    /// <param name="on"></param>
    public NEATGene(int inno, int inID, int outID, float weight, bool on) {
        this.inno = inno;
        this.inID = inID;
        this.outID = outID;
        this.weight = weight;
        this.on = on;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetInID() {
        return inID;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetOutID() {
        return outID;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetInnovation(){
        return inno;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public float GetWeight(){
        return weight;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool GetGeneState() {
        return on;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="on"></param>
    public void SetGeneState(bool on) {
        this.on = on;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="weight"></param>
    public void SetWeight(float weight) {
        this.weight = weight;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
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

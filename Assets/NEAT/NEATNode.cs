using System;

/// <summary>
/// Acts like the individual neuron of a network.
/// </summary>
public class NEATNode {

    //constant node types
    public const int INPUT_NODE = 0;
    public const int INPUT_BIAS_NODE = 1;
    public const int HIDDEN_NODE = 2;
    public const int OUTPUT_NODE = 3;

    private int ID;
    private int type;

    private float value;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="copy"></param>
    public NEATNode(NEATNode copy) {
        this.ID = copy.ID;
        this.type = copy.type;

        if (this.type == INPUT_BIAS_NODE) {
            this.value = 1f;
        }
        else {
            this.value = 0f;
        }

        //this.value = copy.value; << MAJOR BUG FIXED!
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="type"></param>
    public NEATNode(int ID, int type) {
        this.ID = ID;
        this.type = type;      

        if (this.type == INPUT_BIAS_NODE) {
            this.value = 1f;
        }
        else {
            this.value = 0f;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetNodeID() {
        return ID;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetNodeType(){
        return type;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public float GetValue() {
        return value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void SetValue(float value) {
        if(type != INPUT_BIAS_NODE)
            this.value = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Activation() {
        value =  (float)Math.Tanh(value); 
        //value= 1.0f / (1.0f + (float)Math.Exp(-value));
    }
}

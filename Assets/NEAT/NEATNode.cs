using System;

public class NEATNode {

    //constant node types
    public const int INPUT_NODE = 0;
    public const int INPUT_BIAS_NODE = 1;
    public const int HIDDEN_NODE = 2;
    public const int OUTPUT_NODE = 3;

    private int ID;
    private int type;

    private float value;

    public NEATNode(NEATNode copy) {
        this.ID = copy.ID;
        this.type = copy.type;
        this.value = copy.value;
    }

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

    public int GetNodeID() {
        return ID;
    }

    public int GetNodeType(){
        return type;
    }

    public float GetValue()
    {
        return value;
    }

    public void SetValue(float value)
    {
        if(type != INPUT_BIAS_NODE)
            this.value = value;
    }

    public void Activation() {
        value =  (float)Math.Tanh(value);
        //value= 1.0f / (1.0f + (float)Math.Exp(-value));
    }
}

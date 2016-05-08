
public class NEATNode {

    //constant node types
    public const int INPUT_NODE = 0;
    public const int INPUT_BIAS_NODE = 1;
    public const int HIDDEN_NODE = 2;
    public const int OUTPUT_NODE = 3;

    private int ID;
    private int type;

    private float value;

    public NEATNode(int id, int type) {
        this.ID = id;
        this.type = type;      

        if (this.type == INPUT_BIAS_NODE) {
            this.value = 1f;
        }
        else {
            this.value = 0f;
        }
    }
}

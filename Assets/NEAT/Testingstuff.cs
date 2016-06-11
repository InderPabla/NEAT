using UnityEngine;
using System.Collections;

public class Testingstuff : MonoBehaviour {

    public HingeJoint2D j;
    

    void Start()
    {
 
    }

    // Update is called once per frame
    void FixedUpdate () {
        

        Debug.Log(j.jointAngle);
    }
}

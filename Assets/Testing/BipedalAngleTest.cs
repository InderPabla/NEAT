using UnityEngine;
using System.Collections;

public class BipedalAngleTest : MonoBehaviour {
    public Rigidbody2D[] legs = new Rigidbody2D[2];
    public Rigidbody2D body;

    // Use this for initialization
    void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {

        /*string output = "";

        for (int i = 0; i < wheels.Length; i++)
        {
            float currentAngle = 0;
            currentAngle = wheels[i].transform.eulerAngles.z - body.transform.eulerAngles.z;
            output += (int)currentAngle + " ";
            if (currentAngle < 0)
                currentAngle = 360f + currentAngle;
            if (currentAngle < 0)
            {
                JointMotor2D m =  wheels[i].motor;
                m.motorSpeed = -100f;
                wheels[i].motor = m;
            }
            else {
                JointMotor2D m = wheels[i].motor;
                m.motorSpeed = 100f;
                wheels[i].motor = m;
            }
        }*/
        
    }
}

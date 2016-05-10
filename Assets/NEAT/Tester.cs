using UnityEngine;
using System;
using System.Collections.Generic;

public class Tester : MonoBehaviour
{

    public List<Rigidbody2D> bodies;

    private NEATNet net;
    private bool isActive = false;
    private const string ON_FINISHED = "OnFinished";

    public delegate void TestFinishedEventHandler(object source, EventArgs args);
    public event TestFinishedEventHandler TestFinished;

    void Start()
    {
        //isActive = true;
        //net = new NEATNet(0,6,1);
    }

    void FixedUpdate()
    {
        if (isActive == true)
        {
            UpdateNet(); //update neural net

            if (FailCheck() == true)
            {
                OnFinished();
            }
        }
    }

    public void Activate(NEATNet net)
    {
        this.net = net;
        Invoke(ON_FINISHED, net.GetTestTime());
        isActive = true;
    }

    //action based on neural net faling the test
    protected virtual void OnFinished()
    {
        if (TestFinished != null)
        {
            TestFinished(net.GetNetID(), EventArgs.Empty);
            Destroy(gameObject);
        }
    }

    //--Add your own neural net update code here--//
    //Updates nerual net with new inputs from the agent
    private void UpdateNet()
    {

        float boardVelocity = bodies[0].velocity.x; //get current velocity of the board
        //both poles angles in radians
        float pole1AngleRadian = Mathf.Deg2Rad * bodies[1].transform.eulerAngles.z;
        float pole2AngleRadian = Mathf.Deg2Rad * bodies[2].transform.eulerAngles.z;

        //both poles angular velocities 
        float pole1AngularVelocity = bodies[1].angularVelocity;
        float pole2AngularVelocity = bodies[2].angularVelocity;

        float[] inputValues = { boardVelocity, pole1AngleRadian, pole2AngleRadian, pole1AngularVelocity, pole2AngularVelocity }; //gather pole and track data into an array 
        float[] output = net.FireNet(inputValues); //caluclate new neural net output with given input values

        //Debug.Log(output[0]);
        bodies[0].velocity += new Vector2(output[0], 0); //update track velocity with neural net output


    }

    //--Add your own neural net fail code here--//
    //restrictions on the test to fail bad neural networks faster
    private bool FailCheck()
    {
        return false;
    }


}

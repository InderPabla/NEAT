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
        
    }

    void FixedUpdate()
    {
        if (isActive == true)
        {
            UpdateNet(); //update neural net
            CalculateFitness(); //calculate fitness

            if (FailCheck() == true)
            {
                OnFinished();
            }
        }
    }

    public void Activate(NEATNet net)
    {
        this.net = net;
        this.net.SetNetFitness(Mathf.Pow((1f/(float)net.GetGeneCount()),2));
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

        float boardLocation = bodies[0].transform.position.x/4f;

        float[] inputValues = { boardVelocity, boardLocation, pole1AngleRadian, pole2AngleRadian, pole1AngularVelocity, pole2AngularVelocity }; //gather pole and track data into an array 
        float[] output = net.FireNet(inputValues); //caluclate new neural net output with given input values

        //Debug.Log(output[0]);
        bodies[0].velocity += new Vector2(output[0], 0); //update track velocity with neural net output
    }

    //--Add your own neural net fail code here--//
    //restrictions on the test to fail bad neural networks faster
    private bool FailCheck()
    {
        float failDegree = 45;

        float pole1AngleDegree = bodies[1].transform.eulerAngles.z;
        float pole2AngleDegree = bodies[2].transform.eulerAngles.z;

        bool inControl = false;
        bool onTrack = false;

        //if both poles are within 45 degrees on eaither side then fail check is false
        if (((pole1AngleDegree <= failDegree && pole1AngleDegree >= 0) || (pole1AngleDegree <= 360 && pole1AngleDegree >= (360 - failDegree))) &&
            ((pole2AngleDegree <= failDegree && pole2AngleDegree >= 0) || (pole2AngleDegree <= 360 && pole2AngleDegree >= (360 - failDegree))))
        {
            inControl = true;
        }
        else
            return true;
        //if both poles are above 0 y then fail check is false
        if (bodies[1].transform.localPosition.y > 0 && bodies[2].transform.localPosition.y > 0)
        {
            onTrack = true;
        }
        else
            return true;

        /*float boardLocation = Mathf.Abs(bodies[0].transform.position.x) / 4f;
        if (boardLocation > 1f)
            return true;*/

        return false;



        /*if (inControl == true && onTrack == true)
            return false;
        else
            //one or both the pole(s) have fallen below 45 degrees or have fallen below 0 y, thus fail is true
            return true;*/
    }

    //--Add your own neural net fail code here--//
    //Fitness calculation
    private void CalculateFitness() {
        float factor = 1f;

        float pole1Factor = bodies[1].transform.eulerAngles.z;
        float pole2Factor = bodies[2].transform.eulerAngles.z;

        if (pole1Factor < 90f)
        {
            pole1Factor = ((90f - pole1Factor) / 90f);
        }
        else if (pole1Factor > 270f)
        {
            pole1Factor = ((pole1Factor - 270f) / 90f);
        }

        if (pole2Factor < 90f)
        {
            pole2Factor = ((90f - pole2Factor) / 90f);
        }
        else if (pole2Factor > 270f)
        {
            pole2Factor = ((pole2Factor - 270f) / 90f);
        }

        float speedFactor = Mathf.Abs(bodies[0].velocity.x);
        if (speedFactor < 1f)
            speedFactor = 1f;
        speedFactor = 1f / speedFactor;
       

        float boardFactor = Mathf.Abs(bodies[0].transform.position.x);
        if (boardFactor < 1f)
            boardFactor = 1f;
        boardFactor = 1f / boardFactor;

        //factor = factor * pole1Factor * pole2Factor * speedFactor;
        // factor = factor * pole1Factor * pole2Factor;

        factor = factor * pole1Factor * pole2Factor * boardFactor * speedFactor;
        float fit = Time.deltaTime + factor*Time.deltaTime;
        net.AddNetFitness(fit);
        
    }
}

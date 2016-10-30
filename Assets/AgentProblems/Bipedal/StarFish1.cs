using UnityEngine;
using System.Collections;
using System;


public class StarFish1 : MonoBehaviour, IAgentTester
{

    private NEATGeneticControllerV2 controller;
    private NEATNet net;
    private bool isActive = false;
    private bool isLoaded = false;
    private const string ACTION_ON_FINISHED = "OnFinished";
    public delegate void TestFinishedEventHandler(object source, EventArgs args);
    public event TestFinishedEventHandler TestFinished;

    private float damage = 100f;
    private bool finished = false;

    public WheelJoint2D[] wheels1 = new WheelJoint2D[5];
    public Rigidbody2D body;
    public TouchDetector bodyTouch;
    public TouchDetector[] detector1 = new TouchDetector[5];
    private float[] initialDeg = new float[5];

    //12 (11+1 bias) inputs, 5 outputs 
    public void UpdateNet()
    {
        float bodyDeg = body.transform.eulerAngles.z;

        if (bodyDeg < 0f)
            bodyDeg = 360f + bodyDeg;

        float bodyRad = bodyDeg;
        if (bodyRad > 180f)
            bodyRad = bodyRad - 360f;
        bodyRad *= Mathf.Deg2Rad;

        float[] wheels1Rad = new float[5];
        float[] wheels1Deg = new float[5];

        for (int i = 0; i < wheels1Deg.Length; i++)
        {
            wheels1Deg[i] = wheels1[i].transform.eulerAngles.z - initialDeg[i];
            if (wheels1Deg[i] < 0f)
                wheels1Deg[i] = 360f + wheels1Deg[i];


            wheels1Rad[i] = wheels1Deg[i];
            if (wheels1Rad[i] > 180f)
                wheels1Rad[i] = 180f - wheels1Rad[i];
            wheels1Rad[i] = wheels1Rad[i] * Mathf.Deg2Rad;
        }


        float[] output = net.FireNet(new float[] { bodyRad,
            wheels1Rad[0], wheels1Rad[1], wheels1Rad[2], wheels1Rad[3],wheels1Rad[4],
            detector1[0].touch, detector1[1].touch, detector1[2].touch, detector1[3].touch,detector1[4].touch});


        for (int i = 0; i < wheels1Deg.Length; i++)
        {
           
            JointMotor2D jointMotor = wheels1[i].motor;
            float speed = 0f;
            float value = output[i];

            if (value < 0 && wheels1Deg[i] > 10f && wheels1Deg[i] < 180f)
            {
                speed = 0;
            }
            else if (value > 0 && wheels1Deg[i] < 10f && wheels1Deg[i] > 180f)
            {
                speed = 0;
            }
            else
            {
                speed = value * 150f;
            }

            jointMotor.motorSpeed = speed;
            wheels1[i].motor = jointMotor;
        }
    }

    public bool FailCheck()
    {
        if (damage <= 0)
        {
            return true;
        }

        /*if (bodyTouch.touch == 1)
            return true;*/
        return false;
    }

    public void CalculateFitnessOnUpdate()
    {
        net.AddTimeLived(Time.deltaTime);
    }

    //--Add your own neural net fail code here--//
    //Final fitness calculations
    public void CalculateFitnessOnFinish()
    {
        float fit = body.transform.position.x;
        if (fit < 0)
            fit = UnityEngine.Random.Range(0f, 0.001f);
        fit = fit * fit;
        net.SetNetFitness(fit);
    }



    //---
    void FixedUpdate()
    {
        if (isActive == true)
        {
            UpdateNet(); //update neural net
            CalculateFitnessOnUpdate(); //calculate fitness

            if (FailCheck() == true)
            {
                OnFinished();
            }
        }
    }

    //action based on neural net faling the test
    //protected virtual
    public virtual void OnFinished()
    {
        if (TestFinished != null)
        {
            if (!finished)
            {
                finished = true;
                CalculateFitnessOnFinish();
                TestFinished(net.GetNetID(), EventArgs.Empty);
                TestFinished -= controller.OnFinished; //unsubscrive from the event notification
                Destroy(gameObject); //destroy this gameobject
            }
        }
    }

    public void Activate(NEATNet net)
    {
        this.net = net;
        Invoke(ACTION_ON_FINISHED, net.GetTestTime());
        isActive = true;
    }

    public NEATNet GetNet()
    {
        return net;
    }

    public void SubscriveToEvent(NEATGeneticControllerV2 controller)
    {
        this.controller = controller;
        TestFinished += controller.OnFinished; //subscrive to an event notification
    }

    void Start()
    {
        for (int i = 0; i < initialDeg.Length; i++)
        {
            initialDeg[i] = wheels1[i].transform.eulerAngles.z;
            if (initialDeg[i] < 0)
            {
                initialDeg[i] = 360f + initialDeg[i];
            }
        }
    }
}

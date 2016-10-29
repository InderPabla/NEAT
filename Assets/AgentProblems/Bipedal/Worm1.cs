using UnityEngine;
using System.Collections;
using System;


public class Worm1 : MonoBehaviour, IAgentTester
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

    public WheelJoint2D[] wheels1 = new WheelJoint2D[4];
    public TouchDetector[] detector1 = new TouchDetector[5];
    public Rigidbody2D front;

    public void UpdateNet()
    {
        float frontDeg = front.transform.eulerAngles.z;
        if (frontDeg < 0f)
            frontDeg = 360f + frontDeg;

        float frontRad = frontDeg;
        if (frontRad > 180f)
            frontRad = frontRad - 360f;
        frontRad *= Mathf.Deg2Rad;

        float[] wheels1Rad = new float[4];
        float[] wheels1Deg = new float[4];

        for (int i = 0; i < wheels1Deg.Length; i++)
        {
            wheels1Deg[i] = wheels1[i].transform.eulerAngles.z;
            if (wheels1Deg[i] < 0f)
                wheels1Deg[i] = 360f + wheels1Deg[i];


            if (wheels1Deg[i] < 0f)
                wheels1Deg[i] = 360f + wheels1Deg[i];

            wheels1Rad[i] = wheels1Deg[i];
            if (wheels1Rad[i] > 180f)
                wheels1Rad[i] = 180f - wheels1Rad[i];
            wheels1Rad[i] = wheels1Rad[i] * Mathf.Deg2Rad;
        }


        float[] output = net.FireNet(new float[] {frontDeg,
            wheels1Rad[0]/(Mathf.PI/6f), wheels1Rad[1]/(Mathf.PI/6f), wheels1Rad[2]/(Mathf.PI/6f), wheels1Rad[3]/(Mathf.PI/6f),
            detector1[0].touch, detector1[1].touch, detector1[2].touch, detector1[3].touch, detector1[4].touch});


        for (int i = 0; i < wheels1Deg.Length; i++)
        {

            JointMotor2D jointMotor = wheels1[i].motor;
            float speed = 0f;
            float value = output[i];

            if (value < 0 && wheels1Deg[i] > 50f && wheels1Deg[i] < 180f)
            {
                speed = 0;
            }
            else if (value > 0 && wheels1Deg[i] < 310f && wheels1Deg[i] > 180f)
            {
                speed = 0;
            }
            else
            {
                speed = value * 200f;
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
        float fit = wheels1[0].transform.position.x;
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
}

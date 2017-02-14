using UnityEngine;
using System.Collections;
using System;


public class CarWalker : MonoBehaviour, IAgentTester
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

    
    public Rigidbody2D body;
    public WheelJoint2D dragger1;
    public WheelJoint2D dragger2;
    public TouchDetector draggerTouch;
    //10 (9+1 bias) inputs, 4 outputs 
    int deg1State = 0;
    int deg2State = 0;
    public void UpdateNet()
    {
        float bodyDeg = body.transform.eulerAngles.z;

        if (bodyDeg < 0f)
            bodyDeg = 360f + bodyDeg;

        float bodyRad = bodyDeg;
        if (bodyRad > 180f)
            bodyRad = bodyRad - 360f;
        bodyRad *= Mathf.Deg2Rad;

        float dragger1Deg = dragger1.transform.eulerAngles.z;
        if (dragger1Deg < 0f)
            dragger1Deg = 360f + dragger1Deg;
        dragger1Deg = dragger1Deg - bodyDeg;
        if (dragger1Deg < 0f)
            dragger1Deg = 360f + dragger1Deg;

        float dragger2Deg = dragger2.transform.eulerAngles.z;
        if (dragger2Deg < 0f)
            dragger2Deg = 360f + dragger2Deg;
        dragger2Deg = dragger2Deg - dragger1Deg;
        if (dragger2Deg < 0f)
            dragger2Deg = 360f + dragger2Deg;

        float dragger1Rad = dragger1Deg;
        if (dragger1Rad > 180f)
            dragger1Rad = 180f - dragger1Rad;
        dragger1Rad = dragger1Rad * Mathf.Deg2Rad;

        float dragger2Rad = dragger2Deg;
        if (dragger2Rad > 180f)
            dragger2Rad = 180f - dragger2Rad;
        dragger2Rad = dragger2Rad * Mathf.Deg2Rad;

       

        float[] output = net.FireNet(new float[] { bodyRad, dragger1Rad, dragger2Rad, draggerTouch.touch, deg1State, deg2State});

        float dragger1Value = output[0];
        float dragger2Value = output[1];
        float dragger1Speed = 0f;
        float dragger2Speed = 0f;


        if (dragger1Value < 0 && dragger1Deg > 2 && dragger1Deg < 180f)
        {
            dragger1Speed = 0;
            deg1State = -1;

        }
        else if (dragger1Value > 0 && dragger1Deg < 315f && dragger1Deg > 180f)
        {
            dragger1Speed = 0;
            deg1State = 1;
        }
        else
        {
            dragger1Speed = dragger1Value * 150f;
            deg1State = 0;
        }

        if (dragger2Value < 0 && dragger2Deg > 110f && dragger2Deg < 180f)
        {
            dragger2Speed = 0;
            deg2State = -1;

        }
        else if (dragger2Value > 0 && dragger2Deg < 250f && dragger2Deg > 180f)
        {
            dragger2Speed = 0;
            deg2State = 1;
        }
        else
        {
            dragger2Speed = dragger2Value * 150f;
            deg2State = 0;
        }

        JointMotor2D dragger1Motor = dragger1.motor;
        dragger1Motor.motorSpeed = dragger1Speed;
        dragger1.motor = dragger1Motor;

        JointMotor2D dragger2Motor = dragger2.motor;
        dragger2Motor.motorSpeed = dragger2Speed;
        dragger2.motor = dragger2Motor;

        net.SetNetFitness(body.transform.position.x);

        float diff1 = Mathf.Abs(dragger1Deg - pervDragger1Deg);
        //float diff2 = Mathf.Abs(dragger2Deg - pervDragger2Deg);

        if (diff1 < 0.0001)
            diff1 = 0.0001f;
        diff1 = Mathf.Pow((1f / diff1), 0.1f) * 0.05f;


        /*if (diff2 < 0.0001)
            diff2 = 0.0001f;
        diff2 = Mathf.Pow((1f / diff2), 0.1f) * 0.05f;*/


        damage -= diff1;;
        //damage -= diff2;

        pervDragger1Deg = dragger1Deg;
        //pervDragger2Deg = dragger2Deg;

    }
    float pervDragger1Deg, pervDragger2Deg;
    public bool FailCheck()
    {
        if (damage <= 0)
        {
            return true;
        }

        if ((body.transform.eulerAngles.z > 20f && body.transform.eulerAngles.z < 340f))
        {
            net.SetNetFitness(net.GetNetFitness()/3f);
            return true;
        }

        return false;
    }

    public void CalculateFitnessOnUpdate()
    {
        
    }

    //--Add your own neural net fail code here--//
    //Final fitness calculations
    public void CalculateFitnessOnFinish()
    {

        float fit = net.GetNetFitness();

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

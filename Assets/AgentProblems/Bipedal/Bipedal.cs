using UnityEngine;
using System.Collections;
using System;


public class Bipedal : MonoBehaviour, IAgentTester
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
    public Rigidbody2D body;
    public TouchDetector[] detector1 = new TouchDetector[5];
    public bool[] wheels1Broken = new bool[4];
    float[] wheels1DegPrev = new float[4];

    //10 (9+1 bias) inputs, 4 outputs 
    public void UpdateNet()
    {
        float bodyDeg = body.transform.eulerAngles.z;
        
        if (bodyDeg < 0f)
            bodyDeg = 360f + bodyDeg;

        float bodyRad = bodyDeg;
        if (bodyRad > 180f)
            bodyRad = bodyRad - 360f;
        bodyRad *= Mathf.Deg2Rad;

        float[] wheels1Rad = new float[4];
        float[] wheels1Deg = new float[4];

        for (int i = 0; i < wheels1Deg.Length; i++)
        {
            wheels1Deg[i] = wheels1[i].transform.eulerAngles.z;
            if (wheels1Deg[i] < 0f)
                wheels1Deg[i] = 360f + wheels1Deg[i];
            
            if (i < 2)
            {
                wheels1Deg[i] = wheels1Deg[i] - bodyDeg;
            }
            else if (i == 2)
            {
                wheels1Deg[i] = wheels1Deg[i] - wheels1Deg[0];
            }
            else if (i == 3)
            {
                wheels1Deg[i] = wheels1Deg[i] - wheels1Deg[1];
            }

            if (wheels1Deg[i] < 0f)
                wheels1Deg[i] = 360f + wheels1Deg[i];

            wheels1Rad[i] = wheels1Deg[i];
            if (wheels1Rad[i] > 180f)
                wheels1Rad[i] = 180f - wheels1Rad[i];
            wheels1Rad[i] = wheels1Rad[i] * Mathf.Deg2Rad;
        }


        float[] output = net.FireNet(new float[] { bodyRad,
            wheels1Rad[0]/(Mathf.PI/6f), wheels1Rad[1]/(Mathf.PI/6f), wheels1Rad[2]/(Mathf.PI/6f), wheels1Rad[3]/(Mathf.PI/6f),
            detector1[0].touch, detector1[1].touch, detector1[2].touch, detector1[3].touch});


        for (int i = 0; i < wheels1Deg.Length; i++)
        {
            if (wheels1Broken[i] == false)
            {
                JointMotor2D jointMotor = wheels1[i].motor;
                float speed = 0f;
                float value = output[i];

                if (value < 0 && wheels1Deg[i] > 50f && wheels1Deg[i] < 180f)
                {
                    speed =0 ;
                    if (wheels1Deg[i] > 70f)
                        wheels1Broken[i] = true;
                }
                else if (value > 0 && wheels1Deg[i] < 310f && wheels1Deg[i] > 180f)
                {
                    speed = 0;
                    if (wheels1Deg[i] < 290f)
                        wheels1Broken[i] = true;
                }
                else
                {
                    speed = value * 200f;
                }

                jointMotor.motorSpeed = speed;
                wheels1[i].motor = jointMotor;
            }
            else
            {
                wheels1[i].useMotor = false;
            }
        }

        if (detector1[0].touch == 1 || detector1[1].touch == 1 || detector1[4].touch == 1)
            damage -= 5f;


        for (int i = 0; i < wheels1DegPrev.Length; i++)
        {
            float diff = Mathf.Abs(wheels1Deg[i] - wheels1DegPrev[i]);

            if (diff < 0.0001)
                diff = 0.0001f;
            diff = Mathf.Pow((1f / diff), 0.1f) *0.05f;
            //diff = (diff/360f) *1f;


            damage -= diff;


        }

        for (int i = 0; i < wheels1DegPrev.Length; i++)
        {
            wheels1DegPrev[i] = wheels1Deg[i];
        }

    }

    public bool FailCheck()
    {
        if (damage <= 0)
        {
            return true;
        }

        if ((body.transform.eulerAngles.z > 50f && body.transform.eulerAngles.z < 310f)/* || body.transform.position.y > 1.9f*/)
        {
            float fit = net.GetNetFitness();
            fit /= 2.5f;
            net.SetNetFitness(fit);
            return true;
        }

        for (int i = 0; i < wheels1Broken.Length; i++)
        {
            if (wheels1Broken[i] == true)
            {
                float fit = net.GetNetFitness();
                fit /= 2.5f;
                net.SetNetFitness(fit);
                return true;
            }

        }

        if (detector1[0].touch == 1 || detector1[1].touch == 1 || detector1[4].touch == 1)
        {
            //damage -= 5f;
            float fit = net.GetNetFitness();
            fit /= 2.5f;
            net.SetNetFitness(fit);
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
        float fit = (body.transform.position.x + 
            wheels1[0].transform.position.x + 
            wheels1[1].transform.position.x+
            wheels1[2].transform.position.x+
            wheels1[3].transform.position.x)/5f;

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

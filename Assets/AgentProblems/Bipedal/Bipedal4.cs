using UnityEngine;
using System.Collections;
using System;

public class Bipedal4 : MonoBehaviour, IAgentTester
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
    public WheelJoint2D[] wheels2 = new WheelJoint2D[4];
    public Rigidbody2D body;
    public TouchDetector[] detector1 = new TouchDetector[4];
    public TouchDetector[] detector2 = new TouchDetector[4];
    public bool[] wheels1Broken = new bool[4];
    public bool[] wheels2Broken = new bool[4];

    public void UpdateNet()
    {
        float bodyDeg = body.transform.eulerAngles.z;
        if (bodyDeg < 0f)
            bodyDeg = 360f + bodyDeg;

        float bodyRad = bodyDeg;
        if (bodyRad > 180f)
            bodyRad = bodyRad - 360f;
        bodyRad *= Mathf.Deg2Rad;

        float[] wheels1Deg = new float[4];
        float[] wheels1Rad = new float[4];
        float[] wheels2Deg = new float[4];
        float[] wheels2Rad = new float[4];

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

        for (int i = 0; i < wheels2Deg.Length; i++)
        {
            wheels2Deg[i] = wheels2[i].transform.eulerAngles.z;
            if (wheels2Deg[i] < 0f)
                wheels2Deg[i] = 360f + wheels2Deg[i];

            if (i < 2)
            {
                wheels2Deg[i] = wheels2Deg[i] - bodyDeg;
            }
            else if (i == 2)
            {
                wheels2Deg[i] = wheels2Deg[i] - wheels2Deg[0];
            }
            else if (i == 3)
            {
                wheels2Deg[i] = wheels2Deg[i] - wheels2Deg[1];
            }

            if (wheels2Deg[i] < 0f)
                wheels2Deg[i] = 360f + wheels2Deg[i];

            wheels2Rad[i] = wheels2Deg[i];
            if (wheels2Rad[i] > 180f)
                wheels2Rad[i] = 180f - wheels2Rad[i];
            wheels2Rad[i] = wheels2Rad[i] * Mathf.Deg2Rad;
        }

        float[] output = net.FireNet(new float[] { bodyRad,
            wheels1Rad[0]/(Mathf.PI/6f), wheels1Rad[1]/(Mathf.PI/6f), wheels1Rad[2]/(Mathf.PI/6f), wheels1Rad[3]/(Mathf.PI/6f),
            wheels2Rad[0]/(Mathf.PI/6f), wheels2Rad[1]/(Mathf.PI/6f), wheels2Rad[2]/(Mathf.PI/6f), wheels2Rad[3]/(Mathf.PI/6f),
            detector1[0].touch, detector1[1].touch, detector1[2].touch, detector1[3].touch,
            detector2[0].touch, detector2[1].touch, detector2[2].touch, detector2[3].touch});


        for (int i = 0; i < wheels1Deg.Length; i++)
        {
            if (wheels1Broken[i] == false)
            {
                JointMotor2D jointMotor = wheels1[i].motor;
                float speed = 0f;
                float value = output[i];

                if (value < 0 && wheels1Deg[i] > 30f && wheels1Deg[i] < 180f)
                {
                    speed = 150f;
                    if (wheels1Deg[i] > 60f)
                        wheels1Broken[i] = true;
                }
                else if (value > 0 && wheels1Deg[i] < 330f && wheels1Deg[i] > 180f)
                {
                    speed = -150f;
                    if (wheels1Deg[i] < 300f)
                        wheels1Broken[i] = true;
                }
                else
                {
                    speed = value * 150f;
                }

                jointMotor.motorSpeed = speed;
                wheels1[i].motor = jointMotor;
            }
            else
            {
                wheels1[i].useMotor = false;
            }
        }

        for (int i = 0; i < wheels2Deg.Length; i++)
        {
            if (wheels2Broken[i] == false)
            {
                JointMotor2D jointMotor = wheels2[i].motor;
                float speed = 0f;
                float value = output[i + 4];

                if (value < 0 && wheels2Deg[i] > 30f && wheels2Deg[i] < 180f)
                {
                    speed = 150f;
                    if (wheels2Deg[i] > 60f)
                        wheels2Broken[i] = true;
                }
                else if (value > 0 && wheels2Deg[i] < 330f && wheels2Deg[i] > 180f)
                {
                    speed = -150f;
                    if (wheels2Deg[i] < 300f)
                        wheels2Broken[i] = true;
                }
                else
                {
                    speed = value * 150f;
                }

                jointMotor.motorSpeed = speed;
                wheels2[i].motor = jointMotor;
            }
            else
            {
                wheels2[i].useMotor = false;
            }
        }


    }

    public bool FailCheck()
    {
        if (damage <= 0)
        {
            return true;
        }

        if ((body.transform.eulerAngles.z > 60f && body.transform.eulerAngles.z < 300f) || body.transform.position.y > 1.9f)
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

        for (int i = 0; i < wheels2Broken.Length; i++)
        {
            if (wheels2Broken[i] == true)
            {
                float fit = net.GetNetFitness();
                fit /= 2.5f;
                net.SetNetFitness(fit);
                return true;
            }

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
        float fit = body.transform.position.x;
        if (fit < 0)
            fit = UnityEngine.Random.Range(0f, 0.001f);

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

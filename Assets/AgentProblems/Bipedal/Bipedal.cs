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

    public WheelJoint2D[] wheels = new WheelJoint2D[4];
    public Rigidbody2D body;
    public TouchDetector[] detector = new TouchDetector[4];
    // Use this for initialization
    void Start()
    {
        
    }

    public void UpdateNet()
    {
        float[] angles = new float[4];
        float bodyAngle = body.transform.eulerAngles.z;
        if (bodyAngle > 180f)
            bodyAngle = bodyAngle - 360f;
        bodyAngle *= Mathf.Deg2Rad;

        for (int i = 0; i < angles.Length; i++)
        {
            if (i < 2)
                angles[i] = wheels[i].transform.eulerAngles.z - body.transform.eulerAngles.z;
            else if (i == 2)
                angles[i] = wheels[i].transform.eulerAngles.z - wheels[0].transform.eulerAngles.z;
            else if (i == 3)
                angles[i] = wheels[i].transform.eulerAngles.z - wheels[1].transform.eulerAngles.z;
            //angles[i] = getJointRotation(wheels[i].GetComponent<ConfigurableJoint>()).eulerAngles.z;
            if (angles[i] > 180f)
                angles[i] = angles[i] - 360f;
            angles[i] *= Mathf.Deg2Rad;


        }

        float[] output = net.FireNet(new float[] {bodyAngle,angles[0],angles[1],angles[2],angles[3],detector[0].touch,detector[1].touch,detector[2].touch,detector[3].touch});

        for (int i = 0; i < wheels.Length; i++)
        {
            JointMotor2D jointMotor = wheels[i].motor;
            jointMotor.motorSpeed = output[i] * 100f;

            float currentAngle = 0;
            if (i < 2)
                currentAngle = wheels[i].transform.eulerAngles.z - body.transform.eulerAngles.z;
            else if (i == 2)
            {
                float beforeAngle = wheels[0].transform.eulerAngles.z - body.transform.eulerAngles.z;
                if (beforeAngle < 0f)
                    beforeAngle = 360f + beforeAngle;
                currentAngle = wheels[i].transform.eulerAngles.z - beforeAngle;
            }
            else if (i == 3)
            {
                float beforeAngle = wheels[1].transform.eulerAngles.z - body.transform.eulerAngles.z;
                if (beforeAngle < 0f)
                    beforeAngle = 360f + beforeAngle;
                currentAngle = wheels[i].transform.eulerAngles.z - beforeAngle; 
            }

            if (currentAngle < 0f)
                currentAngle = 360f + currentAngle;
           
            if (i < 2 && output[i] < 0f && currentAngle > 30f && currentAngle < 180f)
            {
                jointMotor.motorSpeed = -output[i]*0f;
            }
            else if (i < 2 && output[i] > 0f && currentAngle < 330f && currentAngle > 180f)
            {
                jointMotor.motorSpeed = -output[i] * 0f;
            }
            else if (i>=2 && output[i] < 0f && currentAngle > 45f && currentAngle < 180f)
            {
                jointMotor.motorSpeed = -output[i] * 0f;
            }
            else if (i >= 2 && output[i] > 0f && currentAngle < 315f && currentAngle > 180f)
            {
                jointMotor.motorSpeed = -output[i] * 0f;
            }

            /*if (i == 2)
                wheels[i].useMotor = false;
            else*/
                wheels[i].motor = jointMotor;
        }


    }

    public Quaternion getJointRotation(ConfigurableJoint joint)
    {
        return (Quaternion.FromToRotation(joint.axis, joint.connectedBody.transform.rotation.eulerAngles));
    }

    public bool FailCheck()
    {
        if (damage <= 0)
        {
            return true;
        }

        if (body.transform.eulerAngles.z > 80f && body.transform.eulerAngles.z < 280f)
        {
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
        float fit = body.transform.position.x;
        if (fit < 0)
            fit = UnityEngine.Random.Range(0f,0.001f);

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

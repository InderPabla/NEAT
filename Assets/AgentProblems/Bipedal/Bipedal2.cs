using UnityEngine;
using System.Collections;
using System;

public class Bipedal2 : MonoBehaviour, IAgentTester
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

    public void UpdateNet()
    {
        
        float bodyAngle = body.transform.eulerAngles.z;
        if (bodyAngle > 180f)
            bodyAngle = bodyAngle - 360f;
        bodyAngle *= Mathf.Deg2Rad;

        float[] angles1 = new float[4];
        for (int i = 0; i < angles1.Length; i++)
        {
            if (i < 2)
                angles1[i] = wheels1[i].transform.eulerAngles.z - body.transform.eulerAngles.z;
            else if (i == 2)
                angles1[i] = wheels1[i].transform.eulerAngles.z - wheels1[0].transform.eulerAngles.z;
            else if (i == 3)
                angles1[i] = wheels1[i].transform.eulerAngles.z - wheels1[1].transform.eulerAngles.z;

            if (angles1[i] > 180f)
                angles1[i] = angles1[i] - 360f;
            angles1[i] *= Mathf.Deg2Rad;
        }

        float[] angles2 = new float[4];
        for (int i = 0; i < angles2.Length; i++)
        {
            if (i < 2)
                angles2[i] = wheels2[i].transform.eulerAngles.z - body.transform.eulerAngles.z;
            else if (i == 2)
                angles2[i] = wheels2[i].transform.eulerAngles.z - wheels2[0].transform.eulerAngles.z;
            else if (i == 3)
                angles2[i] = wheels2[i].transform.eulerAngles.z - wheels2[1].transform.eulerAngles.z;

            if (angles2[i] > 180f)
                angles2[i] = angles2[i] - 360f;
            angles2[i] *= Mathf.Deg2Rad;
        }

        float[] output = net.FireNet(new float[] { bodyAngle,
            angles1[0], angles1[1], angles1[2], angles1[3],
            angles2[0], angles2[1], angles2[2], angles2[3],
            detector1[0].touch, detector1[1].touch, detector1[2].touch, detector1[3].touch,
            detector2[0].touch, detector2[1].touch, detector2[2].touch, detector2[3].touch});

        for (int i = 0; i < wheels1.Length; i++)
        {
            JointMotor2D jointMotor = wheels1[i].motor;
            jointMotor.motorSpeed = Mathf.Sign(output[i]) * 250f;

            float currentAngle = 0;
            if (i < 2)
            {
                currentAngle = wheels1[i].transform.eulerAngles.z - body.transform.eulerAngles.z;
            }
            else if (i == 2)
            {
                float beforeAngle = wheels1[0].transform.eulerAngles.z - body.transform.eulerAngles.z;
                if (beforeAngle < 0f)
                    beforeAngle = 360f + beforeAngle;
                currentAngle = wheels1[i].transform.eulerAngles.z - beforeAngle;
            }
            else if (i == 3)
            {
                float beforeAngle = wheels1[1].transform.eulerAngles.z - body.transform.eulerAngles.z;
                if (beforeAngle < 0f)
                    beforeAngle = 360f + beforeAngle;
                currentAngle = wheels1[i].transform.eulerAngles.z - beforeAngle;
            }

            if (currentAngle < 0f)
                currentAngle = 360f + currentAngle;

            if (i < 2 && output[i] < 0f && currentAngle > 30f && currentAngle < 180f)
            {
                jointMotor.motorSpeed = -Mathf.Sign(output[i]) * 250f;
            }
            else if (i < 2 && output[i] > 0f && currentAngle < 330f && currentAngle > 180f)
            {
                jointMotor.motorSpeed = -Mathf.Sign(output[i]) * 250f;
            }
            else if (i >= 2 && output[i] < 0f && currentAngle > 45f && currentAngle < 180f)
            {
                jointMotor.motorSpeed = -Mathf.Sign(output[i]) * 250f;
            }
            else if (i >= 2 && output[i] > 0f && currentAngle < 315f && currentAngle > 180f)
            {
                jointMotor.motorSpeed = -Mathf.Sign(output[i]) * 250f;
            }

            wheels1[i].motor = jointMotor;
        }

        for (int i = 0; i < wheels2.Length; i++)
        {
            JointMotor2D jointMotor = wheels2[i].motor;
            jointMotor.motorSpeed = Mathf.Sign(output[i + 4]) * 250f;

            float currentAngle = 0;
            if (i < 2) 
                currentAngle = wheels2[i].transform.eulerAngles.z - body.transform.eulerAngles.z;
            else if (i == 2)
            {
                float beforeAngle = wheels2[0].transform.eulerAngles.z - body.transform.eulerAngles.z;
                
                if (beforeAngle < 0f)
                    beforeAngle = 360f + beforeAngle;
                currentAngle = wheels2[i].transform.eulerAngles.z - beforeAngle;
            }
            else if (i == 3)
            {
                float beforeAngle = wheels2[1].transform.eulerAngles.z - body.transform.eulerAngles.z;
                if (beforeAngle < 0f)
                    beforeAngle = 360f + beforeAngle;
                currentAngle = wheels2[i].transform.eulerAngles.z - beforeAngle;
            }

            if (currentAngle < 0f)
                currentAngle = 360f + currentAngle;

            if (i < 2 && output[i + 4] < 0f && currentAngle > 30f && currentAngle < 180f)
            {
                jointMotor.motorSpeed = -Mathf.Sign(output[i+4]) * 250f;
            }
            else if (i < 2 && output[i + 4] > 0f && currentAngle < 330f && currentAngle > 180f)
            {
                jointMotor.motorSpeed = -Mathf.Sign(output[i + 4]) * 250f;
            }
            else if (i >= 2 && output[i + 4] < 0f && currentAngle > 45f && currentAngle < 180f)
            {
                jointMotor.motorSpeed = -Mathf.Sign(output[i + 4]) * 250f;
            }
            else if (i >= 2 && output[i + 4] > 0f && currentAngle < 315f && currentAngle > 180f)
            {
                jointMotor.motorSpeed = -Mathf.Sign(output[i + 4]) * 250f;
            }

            wheels2[i].motor = jointMotor;
        }


        /*for (int i = 0; i < 2; i++) {
            if (detector1[i].touch == 1) {
                damage -= 10f;
            }
        }

        for (int i = 0; i < 2; i++)
        {
            if (detector2[i].touch == 1)
            {
                damage -= 10f;
            }
        }*/
    }

    public bool FailCheck()
    {
        if (damage <= 0)
        {
            return true;
        }

        if (body.transform.eulerAngles.z > 60f && body.transform.eulerAngles.z < 300f)
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

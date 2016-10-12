using UnityEngine;
using System.Collections;
using System;

public class PoleBalancer : MonoBehaviour {

    private NEATGeneticControllerV2 controller;
    private NEATNet net;
    private bool isActive = false;
    private bool isLoaded = false;
    private const string ACTION_ON_FINISHED = "OnFinished";
    public delegate void TestFinishedEventHandler(object source, EventArgs args);
    public event TestFinishedEventHandler TestFinished;

    private float damage = 100f;
    private bool finished = false;

    public Rigidbody2D rPole;
    public Rigidbody2D rBoard;

    // Use this for initialization
    void Start () {
	
	}

    public void UpdateNet()
    {
        float poleAngularVelo = rPole.angularVelocity;
        float boardVeloX = rBoard.velocity.x;
        float poleAngle = rPole.transform.eulerAngles.z;

        if (poleAngle < 45f)
            poleAngle = poleAngle / 45f;
        else
            poleAngle = (315f - poleAngle) / 45f;

        float[] input = new float[] { poleAngularVelo,boardVeloX,poleAngle, (float)Math.Tan(rBoard.transform.position.x)};
        float[] output = net.FireNet(input);

        rBoard.velocity += new Vector2(Time.deltaTime*output[0]*7f,0f);
    }

    public bool FailCheck()
    {
        if (damage <= 0)
        {
            return true;
        }

        float poleAngle = rPole.transform.eulerAngles.z;
        if (poleAngle >= 45f && poleAngle <= 270f)
            return true;
        

        return false;
    }

    public float Sigmoid(float x)
    {
        return 2f / (1f + (float)Math.Exp(-2f * x)) - 1f;
    }

    public void CalculateFitnessOnUpdate()
    {
        float score = Time.deltaTime;
        float poleAngle = rPole.transform.eulerAngles.z;
        if (poleAngle < 90f)
            poleAngle = poleAngle / 90f;
        else
            poleAngle = (270f - poleAngle) / 90f;

        poleAngle = Mathf.Abs(poleAngle);
        float poleAngleScore = 1f - poleAngle;

        float positionScore = Sigmoid(Mathf.Abs(rBoard.transform.position.x));
        float veloScore = 1f-Mathf.Abs(rBoard.velocity.x);
        net.AddNetFitness(score* veloScore);

    }

    //--Add your own neural net fail code here--//
    //Final fitness calculations
    public void CalculateFitnessOnFinish()
    {
        
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

﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

public class Tester : MonoBehaviour
{

    public List<Rigidbody2D> bodies;

    private NEATNet net;
    private bool isActive = false;
    private bool isLoaded = false;
    private const string ON_FINISHED = "OnFinished";
    private Semaphore mutex; 
    public delegate void TestFinishedEventHandler(object source, EventArgs args);
    public event TestFinishedEventHandler TestFinished;

    List<Vector2> points = new List<Vector2>();
    float damage = 100f;

    bool finished = false;

    public GameObject linePrefab;
    private GameObject[] lineObjects = new GameObject[18];
    private LineRenderer[] lines = new LineRenderer[18];
    private float[] sightHit = new float[18];

    void Start()
    {
        mutex = new Semaphore(1, 1);

        /*bodies[0].transform.eulerAngles = new Vector3(0f, 0f, UnityEngine.Random.Range(0f,360f));

        for (int i = 0; i < 18; i++) {
            lineObjects[i] = (GameObject)Instantiate(linePrefab);
            lineObjects[i].transform.parent = transform;
            lines[i] = lineObjects[i].GetComponent<LineRenderer>();
            lines[i].SetWidth(0.1f,0.1f);
            lines[i].material = new Material(Shader.Find("Particles/Additive"));
            lines[i].SetColors(Color.red,Color.red);
        }*/


    }
    float avgAngle = 0;
    void TakePoint() {
        //points.Add(bodies[0].transform.position);
        //points.Add(bodies[0].velocity);

        Invoke("TakePoint", 0.025f);
    }
    bool start = false;
    void FixedUpdate() {
        if (isActive == true) {
            UpdateNet(); //update neural net
            CalculateFitness(); //calculate fitness

            if (FailCheck() == true) {
                OnFinished();
            }
        }
    }

    public void Activate(NEATNet net) {
        this.net = net;
        Invoke(ON_FINISHED, net.GetTestTime());
        isActive = true;
    }

    //action based on neural net faling the test
    protected virtual void OnFinished() {
        if (TestFinished != null)
        {
            if (!finished)
            {
                finished = true;
                CalculateFitnessOnFinish();
                TestFinished(net.GetNetID(), EventArgs.Empty);
                Destroy(gameObject);
            }
        }
    }

    //--Add your own neural net update code here--//
    //Updates nerual net with new inputs from the agent
    private void UpdateNet() {
        /*float boardVelocity = bodies[0].velocity.x; //get current velocity of the board
        //both poles angles in radians
        float pole1AngleRadian = Mathf.Deg2Rad * bodies[1].transform.eulerAngles.z;
        //float pole2AngleRadian = Mathf.Deg2Rad * bodies[2].transform.eulerAngles.z;

        //both poles angular velocities 
        float pole1AngularVelocity = bodies[1].angularVelocity;
        //float pole2AngularVelocity = bodies[2].angularVelocity;

        float boardLocation = bodies[0].transform.localPosition.x;

        float[] inputValues = { boardVelocity, pole1AngleRadian, pole2AngleRadian, pole1AngularVelocity, pole2AngularVelocity }; //gather pole and track data into an array 
        float[] output = net.FireNet(inputValues); //caluclate new neural net output with given input values
        Vector2 velo = bodies[0].velocity;
        velo += new Vector2(output[0], 0);
        bodies[0].velocity = velo;*/

        /*Vector2 dir = bodies[0].transform.up;
        Vector2 deltaVector = pos.position - bodies[0].transform.position;
        deltaVector = deltaVector.normalized;
        float rad2 = Mathf.Atan2(deltaVector.y, deltaVector.x);

        rad2 *= Mathf.Rad2Deg;
        rad2 = 90f - rad2;

        if (rad2 < 0f)
        {
            rad2 += 360f;
        }
        rad2 = 360 - rad2;

        rad2 -= bodies[0].transform.eulerAngles.z;
        if (rad2 < 0)
            rad2 = 360 + rad2;


        if (rad2 >= 180f)
        {
            rad2 = 360 - rad2;
            rad2 *= -1f;
        }

        rad2 *= Mathf.Deg2Rad;*/

        //if(start == false)
            UpdateOverTime();
        //start = true;
        

        /*float angle = -100f;
        float angleAdd = (22.22f) / 2f;
        float distance = 10f;
        float outDistance = 0.35f;
        int ignoreFoodLayer = ~(1 << 8);

        Vector3[] direction = new Vector3[18];
        Vector3[] relativePosition = new Vector3[18];
        RaycastHit2D[] rayHit = new RaycastHit2D[18];

        float redness = 1f-(damage / 100f);
        Color lineColor = new Color(1f, redness, redness);
        

        for (int i = 0; i < 18; i++) {
            direction[i] = Quaternion.AngleAxis(angle, Vector3.forward) * bodies[0].transform.up;
            relativePosition[i] = bodies[0].transform.position + (outDistance * direction[i]);
            rayHit[i] = Physics2D.Raycast(relativePosition[i], direction[i], distance, ignoreFoodLayer);
            lines[i].SetPosition(0, relativePosition[i]);
            sightHit[i] = -1f;

            if (rayHit[i].collider != null) {
                sightHit[i] = Vector2.Distance(rayHit[i].point, bodies[0].transform.position) / distance;
                lines[i].SetPosition(1, rayHit[i].point);
            }
            else {
                lines[i].SetPosition(1, relativePosition[i]);
            }

            lines[i].SetColors(lineColor, lineColor);

            angle += angleAdd;
        }

        damage -= Time.deltaTime * 30;*/
    }

    public void UpdateOverTime() {
        /* if (output[0] > threshold)
           motorSpeed1 = motorPower;
       else if (output[0] < -threshold)
           motorSpeed1 = -motorPower;
       else 
           motorSpeed1 = 0f;

       JointMotor2D motor1 = bodies[1].GetComponent<HingeJoint2D>().motor;
       motor1.motorSpeed = motorSpeed1;
       if (motorSpeed1 == 0)
           bodies[1].GetComponent<HingeJoint2D>().useMotor = false;
       else
           bodies[1].GetComponent<HingeJoint2D>().useMotor = true;
       bodies[1].GetComponent<HingeJoint2D>().motor = motor1;*/


        /*Vector2 dir = bodies[0].transform.up;

        float[] inputValues = 
            {
                sightHit[0], sightHit[1], sightHit[2],
                sightHit[3], sightHit[4], sightHit[5],
                sightHit[6], sightHit[7], sightHit[8],
                sightHit[9], sightHit[10], sightHit[11],
                sightHit[12], sightHit[13], sightHit[14],
                sightHit[15], sightHit[16], sightHit[17],
                (damage / 100f), bodies[0].transform.eulerAngles.z * Mathf.Deg2Rad
            };
        float[] output = net.FireNet(inputValues);

        bodies[0].angularVelocity = output[0] * 250f;
        if (output[1] >= 0)
            bodies[0].velocity = dir * output[1] * 5f;
        else
            bodies[0].velocity /= 2f;

        Invoke("UpdateOverTime",0.08f);*/

        float deg = bodies[0].transform.eulerAngles.z;
        if (deg > 180f)
            deg = (deg - 180f) * -1f;
        deg = deg * Mathf.Deg2Rad;
        float isIn = -1f;
        if (Mathf.Abs(bodies[0].transform.position.y) < 0.25f)
        {
            isIn = 1f;
        }

        float[] inputValues = 
            {
                bodies[0].transform.position.y,
                isIn,
                deg
            };
        float[] output = net.FireNet(inputValues);

        //bodies[0].velocity = new Vector2(output[0]*5f,output[1]*5f);

        Vector2 dir = bodies[0].transform.right;
        bodies[0].velocity = dir * output[1] * 5f;
        bodies[0].angularVelocity = output[0] * 250f;
    }
    
    //--Add your own neural net fail code here--//
    //restrictions on the test to fail bad neural networks faster
    private bool FailCheck() {
        /*float failDegree = 45f;
        float pole1AngleDegree = bodies[1].transform.eulerAngles.z;
        float pole2AngleDegree = bodies[2].transform.eulerAngles.z;
        //if both poles are within 45 degrees on eaither side then fail check is false
        if (!(((pole1AngleDegree <= failDegree && pole1AngleDegree >= 0) || (pole1AngleDegree <= 360 && pole1AngleDegree >= (360 - failDegree))) &&
            ((pole2AngleDegree <= failDegree && pole2AngleDegree >= 0) || (pole2AngleDegree <= 360 && pole2AngleDegree >= (360 - failDegree))))) {
                return true; 
        }
        //if both poles are above 0 y then fail check is false
        if (!(bodies[1].transform.localPosition.y > 0 && bodies[2].transform.localPosition.y > 0)) {
                return true;
        }*/

        /*if (Mathf.Abs(bodies[0].transform.localPosition.x) > 5f) {
            return true;
        }*/

        /*float failDegree = 10f;
        float pole1AngleDegree = bodies[1].transform.eulerAngles.z;
        if (!(((pole1AngleDegree <= failDegree && pole1AngleDegree >= 0) || (pole1AngleDegree <= 360 && pole1AngleDegree >= (360 - failDegree))))) {
            if(isUp == true)
                return true;
        }*/

        /*if (bodies[0].transform.localPosition.y > 2f || bodies[0].transform.localPosition.y < -2f) 
            return true; 
        
        if (bodies[0].transform.eulerAngles.z > 35f && bodies[0].transform.eulerAngles.z<215)
            return true;*/

        if (damage <= 0) 
            return true;
        
        return false;
    }

    bool isUp = false;

    //--Add your own neural net fail code here--//
    //Fitness calculation
    private void CalculateFitness() {
        /*float factor = 1f;

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
        if (speedFactor < 0.1f)
            speedFactor = 0.1f;
        speedFactor = 1f / speedFactor;
       

        float boardFactor = Mathf.Abs(bodies[0].transform.localPosition.x);
        if (boardFactor < 0.1f)
            boardFactor = 0.1f;
        boardFactor =  1f/speedFactor;

        factor = factor * pole1Factor * pole2Factor * boardFactor * speedFactor;
        float fit = factor*Time.deltaTime;

        net.AddNetFitness(fit);*/

        /*float pole1Factor = bodies[1].transform.eulerAngles.z;

        if (pole1Factor < 90f) {
            if (pole1Factor<70f)
                isUp = true;
            pole1Factor = ((90f - pole1Factor) / 90f);
            
        }
        else if (pole1Factor > 270f) {
            if (pole1Factor >290f)
                isUp = true;
            pole1Factor = ((pole1Factor - 270f) / 90f); 
        }
        else {
            pole1Factor = 0f;
        }

        net.AddNetFitness(Mathf.Pow((Time.deltaTime*pole1Factor),1.1f*/

        /*if (bodies[0].angularVelocity == 0)
            fit = fit * 2f;*/
        /*if (bodies[0].velocity.magnitude < 0.5f)
            fit = fit / 2f;*/

        /*if (Mathf.Abs(output[0]) > 0.05f)
            this.net.AddNetFitness(-1f);
        if (output[1] < 0f )
            this.net.AddNetFitness(-1f);*/
        float y = Mathf.Abs(bodies[0].transform.position.y);
        if (y<= 1f)
            net.AddNetFitness(Time.deltaTime*(1f+(1f-y)));
    }


    //--Add your own neural net fail code here--//
    //Final fitness calculations
    private void CalculateFitnessOnFinish() {
        
        /*float fit = net.GetNetFitness();
        float angle = bodies[0].transform.eulerAngles.z;
        if (angle > 180)
            angle = 360 - angle;
        angle = angle * Mathf.Deg2Rad;

        fit = Mathf.Pow(fit,angle);
        this.net.SetNetFitness(fit);*/

        //this.net.SetNetFitness(1f/bodies[0].velocity.magnitude);
        //float avg = bodies[0].transform.position.x + bodies[1].transform.position.x + bodies[2].transform.position.x + bodies[3].transform.position.x + bodies[4].transform.position.x;
        //avg = avg / 5f;
        /*float velo = 0;
        if (points.Count > 0)
        {
            

            for (int i = 0; i < points.Count; i++)
            {
                velo += points[i].x;

            }
            velo = velo / points.Count;
        }

        float avg = bodies[0].transform.localPosition.x * velo;
        if (bodies[0].transform.localPosition.x < 0 || avg<0)
            avg = 0;


        this.net.SetNetFitness(avg);*/

        /*float life = this.net.GetTimeLived();
        life = (life / net.GetTestTime()) * 2f;

        float fit = this.net.GetNetFitness();
        fit = Mathf.Pow(fit,life);

        this.net.SetNetFitness(fit);*/

        //this.net.SetNetFitness(Mathf.Pow((1f / Vector2.Distance(bodies[0].transform.position, pos.position)), life));

        /*float life = this.net.GetTimeLived();
        float factor = (life / net.GetTestTime()) *2f;

        float totalDistanceFit = 0;
        if (points.Count > 0)
        {
            for (int i = 1; i < points.Count; i++)
            {
                float dis = Mathf.Pow(Vector2.Distance(points[i], points[i - 1]), 2);
                totalDistanceFit += dis;
            }
            totalDistanceFit /= 50f;
        }*/

    }

    public void OtherActivity(int type) {
        mutex.WaitOne();

        if (type == 0)
        {
            OnFinished();
        }
        /*if (type == 0) {
            net.AddNetFitness(1f);
            damage = 100f;
        }

        if (type == 1) {
            net.SetNetFitness(net.GetNetFitness()*0.5f);
            OnFinished();
        }*/

        mutex.Release();
    }

    public NEATNet GetNet() {
        return net;
    }

}

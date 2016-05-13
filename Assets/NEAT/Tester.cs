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

    List<Vector2> points = new List<Vector2>();
    float damage = 10f;
    void Start()
    {
        TakePoint();
        bodies[0].transform.eulerAngles = new Vector3(0f, 0f, UnityEngine.Random.Range(0f,360f));
    }

    void TakePoint() {
        points.Add(bodies[0].transform.position);
        Invoke("TakePoint",0.5f);
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
        Invoke(ON_FINISHED, net.GetTestTime());
        isActive = true;
    }

    //action based on neural net faling the test
    protected virtual void OnFinished()
    {
        if (TestFinished != null)
        {
            CalculateFitnessOnFinish();
            TestFinished(net.GetNetID(), EventArgs.Empty);
            Destroy(gameObject);
        }
    }

    float h1 = -1f, h2 = -1f, h3 = -1f, h4 = -1f, h5 = -1f, h6 = -1f, h7 = -1f, h8 = -1f, h9 = -1f, h10 = -1f;
    //--Add your own neural net update code here--//
    //Updates nerual net with new inputs from the agent
    private void UpdateNet()
    {

        /*float boardVelocity = bodies[0].velocity.x; //get current velocity of the board
        //both poles angles in radians
        float pole1AngleRadian = Mathf.Deg2Rad * bodies[1].transform.eulerAngles.z;
        float pole2AngleRadian = Mathf.Deg2Rad * bodies[2].transform.eulerAngles.z;

        //both poles angular velocities 
        float pole1AngularVelocity = bodies[1].angularVelocity;
        float pole2AngularVelocity = bodies[2].angularVelocity;

        float boardLocation = bodies[0].transform.position.x/4f;

        float[] inputValues = { boardVelocity, boardLocation, pole1AngleRadian, pole2AngleRadian, pole1AngularVelocity, pole2AngularVelocity }; //gather pole and track data into an array 
        float[] output = net.FireNet(inputValues); //caluclate new neural net output with given input values
        bodies[0].velocity += new Vector2(output[0], 0); //update track velocity with neural net output*/



        Vector2 dir1 = Quaternion.AngleAxis(22.5f, Vector3.forward) * bodies[0].transform.up;
        Vector2 dir2 = Quaternion.AngleAxis(45f, Vector3.forward) * bodies[0].transform.up;
        Vector2 dir3 = Quaternion.AngleAxis(67.5f, Vector3.forward ) * bodies[0].transform.up;
        Vector2 dir4 = Quaternion.AngleAxis(90f, Vector3.forward) * bodies[0].transform.up;
        Vector2 dir5 = Quaternion.AngleAxis(-22.5f, Vector3.forward) * bodies[0].transform.up;
        Vector2 dir6 = Quaternion.AngleAxis(-45f, Vector3.forward) * bodies[0].transform.up;
        Vector2 dir7 = Quaternion.AngleAxis(-67.5f, Vector3.forward) * bodies[0].transform.up;
        Vector2 dir8 = Quaternion.AngleAxis(-90f, Vector3.forward) * bodies[0].transform.up;
        Vector2 dir9 = Quaternion.AngleAxis(180f, Vector3.forward) * bodies[0].transform.up;

        Vector2 position = bodies[0].transform.position + 0.5f* bodies[0].transform.up;
        RaycastHit2D hit1 = Physics2D.Raycast(position, bodies[0].transform.up, 2.5f);
        RaycastHit2D hit2 = Physics2D.Raycast(position, dir1, 2.5f);
        RaycastHit2D hit3 = Physics2D.Raycast(position, dir2, 2.5f);
        RaycastHit2D hit4 = Physics2D.Raycast(position, dir3, 2.5f);
        RaycastHit2D hit5 = Physics2D.Raycast(position, dir4, 2.5f);
        RaycastHit2D hit6 = Physics2D.Raycast(position, dir5, 2.5f);
        RaycastHit2D hit7 = Physics2D.Raycast(position, dir6, 2.5f);
        RaycastHit2D hit8 = Physics2D.Raycast(position, dir7, 2.5f);
        RaycastHit2D hit9 = Physics2D.Raycast(position, dir8, 2.5f);

        position = bodies[0].transform.position - 0.5f * bodies[0].transform.up;
        RaycastHit2D hit10 = Physics2D.Raycast(position, dir9, 4f);

        h1 = -1f; h2 = -1f; h3 = -1f; h4 = -1f; h5 = -1f; h6 = -1f; h7 = -1f; h8 = -1f; h9 = -1f; h10 = -1f;
        if (hit1.collider!=null)
        {
            /*UnityEngine.Debug.DrawLine(bodies[0].transform.position, hit1.point, Color.red, 0.01f);
            float dis = Vector2.Distance(hit1.point, bodies[0].transform.position);
            UnityEngine.Debug.Log(dis);*/
            h1 = Vector2.Distance(hit1.point, bodies[0].transform.position);
        }

        if (hit2.collider != null)
        {
            h2 = Vector2.Distance(hit2.point, bodies[0].transform.position);
        }

        if (hit3.collider != null)
        {
            h3 = Vector2.Distance(hit3.point, bodies[0].transform.position); 
        }

        if (hit4.collider != null)
        {
            h4 = Vector2.Distance(hit4.point, bodies[0].transform.position);   
        }

        if (hit5.collider != null)
        {
            h5 = Vector2.Distance(hit5.point, bodies[0].transform.position);  
        }

        if (hit6.collider != null)
        {
            h6 = Vector2.Distance(hit6.point, bodies[0].transform.position); 
        }

        if (hit7.collider != null)
        {
            h7 = Vector2.Distance(hit7.point, bodies[0].transform.position);
        }
        if (hit8.collider != null)
        {
            h8 = Vector2.Distance(hit8.point, bodies[0].transform.position);
        }
        if (hit9.collider != null)
        {
            h9 = Vector2.Distance(hit9.point, bodies[0].transform.position);
        }
        if (hit10.collider != null)
        {
            h10 = Vector2.Distance(hit10.point, bodies[0].transform.position);
        }

        float boardLocation = bodies[0].transform.position.x / 4f;

        float[] inputValues = { h1,h2,h3,h4,h5,h6,h7,h8,h9,h10, damage/10f}; //gather pole and track data into an array 

        float[] output = net.FireNet(inputValues);

        Vector2 dir = bodies[0].transform.up;
        if (output[1] > 0)
            bodies[0].velocity = 2f * dir  * output[1];


        if (output[0]<0f)
            bodies[0].angularVelocity = 500f;
        else
            bodies[0].angularVelocity = -500f;

        //bodies[0].angularVelocity = output[0] * -750f;

        float doDamage = 2f;
        if (bodies[0].velocity.magnitude <= 0.25f)
            doDamage = 0f;

        if (h1 != -1)
        {
            h1 = Mathf.Abs(h1);
            if (h1 <= 1f)
                damage -= doDamage;
        }

        if (h2 != -1)
        {
            h2 = Mathf.Abs(h2);
            if (h2 <= 1f)
                damage -= doDamage;
        }

        if (h3 != -1)
        {
            h3 = Mathf.Abs(h3);
            if (h3 <= 1f)
                damage -= doDamage;
        }

        if (h4 != -1)
        {
            h4 = Mathf.Abs(h4);
            if (h4 <= 1f)
                damage -= doDamage;
        }

        if (h5 != -1)
        {
            h5 = Mathf.Abs(h5);
            if (h5 <= 1f)
                damage -= doDamage;
        }

        if (h6 != -1)
        {
            h6 = Mathf.Abs(h6);
            if (h6 <= 1f)
                damage -= doDamage;
        }

        if (h7 != -1)
        {
            h7 = Mathf.Abs(h7);
            if (h7 <= 1f)
                damage -= doDamage;
        }

        if (h8 != -1)
        {
            h8 = Mathf.Abs(h8);
            if (h8 <= 1f)
                damage -= doDamage;
        }

        if (h9 != -1)
        {
            h9 = Mathf.Abs(h9);
            if (h9 <= 1f)
                damage -= doDamage;
        }

        if (h10 != -1)
        {
            h10 = Mathf.Abs(h10);
            if (h10 <= 1f)
                damage -= doDamage;
        }

    }

    //--Add your own neural net fail code here--//
    //restrictions on the test to fail bad neural networks faster
    private bool FailCheck()
    {
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

        /*h1 = Mathf.Abs(h1);
        h2 = Mathf.Abs(h2);
        h3 = Mathf.Abs(h3);
        h4 = Mathf.Abs(h4);
        h5 = Mathf.Abs(h5);
        h6 = Mathf.Abs(h6);
        if (h1 <= 0.85f || h2 <= 0.85f || h3 <= 0.85f || h4 <= 0.85f || h5 <= 0.85f || h6 <= 0.85f)
        {
            damage -= 0.5f;
        }*/

        if (damage<=0)
            return true;

        return false;
    }

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
        if (speedFactor < 1f)
            speedFactor = 1f;
        speedFactor = 1f / speedFactor;
       

        float boardFactor = Mathf.Abs(bodies[0].transform.position.x);
        if (boardFactor < 1f)
            boardFactor = 1f;
        boardFactor = 1f / boardFactor;

        //factor = factor * pole1Factor * pole2Factor * boardFactor * speedFactor;
        float fit = Time.deltaTime + factor*Time.deltaTime;
        net.AddNetFitness(fit);*/


    }

    //--Add your own neural net fail code here--//
    //Final fitness calculations
    private void CalculateFitnessOnFinish() {
        this.net.AddNetFitness(Mathf.Pow((1f / (float)net.GetGeneCount()), 2));

        for (int i = 1; i < points.Count; i++) {
            float dis = Mathf.Pow(Vector2.Distance(points[i],points[i-1]),4f);
            this.net.AddNetFitness(dis);
        }
    }

    public void OtherActivity(int type) {
        if (type == 0)
            OnFinished();
        else
            damage--;
    }

}

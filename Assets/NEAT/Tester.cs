using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

public class Tester : MonoBehaviour
{

    public List<Rigidbody2D> bodies;

    private NEATNet net;
    private bool isActive = false;
    private const string ON_FINISHED = "OnFinished";
    private Semaphore mutex; 
    public delegate void TestFinishedEventHandler(object source, EventArgs args);
    public event TestFinishedEventHandler TestFinished;

    List<Vector2> points = new List<Vector2>();
    float damage = 10f;
    
    void Start()
    {
        mutex = new Semaphore(1, 1);

        TakePoint();
        bodies[0].transform.eulerAngles = new Vector3(0f, 0f, UnityEngine.Random.Range(0f,360f));
    }

    void TakePoint() {
        points.Add(bodies[0].transform.position);
        Invoke("TakePoint",1f);
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
    float doDamage = 5f;
    float[] output;
    float state = 0f;
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

        float boardLocation = bodies[0].transform.localPosition.x;

        float[] inputValues = { boardVelocity, boardLocation, pole1AngleRadian, pole2AngleRadian, pole1AngularVelocity, pole2AngularVelocity }; //gather pole and track data into an array 
        float[] output = net.FireNet(inputValues); //caluclate new neural net output with given input values
        bodies[0].velocity += new Vector2(output[0], 0); //update track velocity with neural net output*/

        float angle = -100f;
        float angleAdd = 22.22f;

        Vector2 dir1 = Quaternion.AngleAxis(angle, Vector3.forward) * bodies[0].transform.up;
        angle += angleAdd;
        Vector2 dir2 = Quaternion.AngleAxis(angle, Vector3.forward) * bodies[0].transform.up;
        angle += angleAdd;
        Vector2 dir3 = Quaternion.AngleAxis(angle, Vector3.forward) * bodies[0].transform.up;
        angle += angleAdd;
        Vector2 dir4 = Quaternion.AngleAxis(angle, Vector3.forward) * bodies[0].transform.up;
        angle += angleAdd;
        Vector2 dir5 = Quaternion.AngleAxis(angle, Vector3.forward) * bodies[0].transform.up;
        angle += angleAdd;
        Vector2 dir6 = Quaternion.AngleAxis(angle, Vector3.forward) * bodies[0].transform.up;
        angle += angleAdd;
        Vector2 dir7 = Quaternion.AngleAxis(angle, Vector3.forward) * bodies[0].transform.up;
        angle += angleAdd;
        Vector2 dir8 = Quaternion.AngleAxis(angle, Vector3.forward) * bodies[0].transform.up;
        angle += angleAdd;
        Vector2 dir9 = Quaternion.AngleAxis(angle, Vector3.forward) * bodies[0].transform.up;


        Vector2 position1 = bodies[0].transform.position + (bodies[0].transform.up*0.5f);
        RaycastHit2D hit1 = Physics2D.Raycast(position1, dir1, 2f);
        RaycastHit2D hit2 = Physics2D.Raycast(position1, dir2, 2f);
        RaycastHit2D hit3 = Physics2D.Raycast(position1, dir3, 2f);
        RaycastHit2D hit4 = Physics2D.Raycast(position1, dir4, 2f);
        RaycastHit2D hit5 = Physics2D.Raycast(position1, dir5, 2f);
        RaycastHit2D hit6 = Physics2D.Raycast(position1, dir6, 2f);
        RaycastHit2D hit7 = Physics2D.Raycast(position1, dir7, 2f);
        RaycastHit2D hit8 = Physics2D.Raycast(position1, dir8, 2f);
        RaycastHit2D hit9 = Physics2D.Raycast(position1, dir9, 2f);

        Vector3 dir10 = Quaternion.AngleAxis(180f, Vector3.forward) * bodies[0].transform.up;
        Vector2 position10 = bodies[0].transform.position + (0.5f * dir10);
        RaycastHit2D hit10 = Physics2D.Raycast(position10, dir10, 2f);

        h1 = -1f; h2 = -1f; h3 = -1f; h4 = -1f; h5 = -1f; h6 = -1f; h7 = -1f; h8 = -1f; h9 = -1f; h10 = -1f;
        float hitCreatureAdd = 0f;
        string otherCreatureName = "B";
        //h1 = 0f; h2 = 0f; h3 = 0f; h4 = 0f; h5 = 0f; h6 = 0f; h7 = 0f; h8 = 0f; h9 = 0f; h10 = 0f;
        if (hit1.collider!=null)
        {
            h1 = Vector2.Distance(hit1.point, bodies[0].transform.position);
            if (hit1.collider.name.Contains(otherCreatureName))
            {
                h1 += hitCreatureAdd;
            }
            Debug.DrawLine(position1, hit1.point, Color.red, 0.002f);
        }

        if (hit2.collider != null)
        {
            h2 = Vector2.Distance(hit2.point, bodies[0].transform.position);
            if (hit2.collider.name.Contains(otherCreatureName))
                h2 += hitCreatureAdd;
            Debug.DrawLine(position1, hit2.point, Color.red, 0.002f);
        }

        if (hit3.collider != null)
        {
            h3 = Vector2.Distance(hit3.point, bodies[0].transform.position);
            if (hit3.collider.name.Contains(otherCreatureName))
                h3 += hitCreatureAdd;
            Debug.DrawLine(position1, hit3.point, Color.red, 0.002f);
        }

        if (hit4.collider != null)
        {
            h4 = Vector2.Distance(hit4.point, bodies[0].transform.position);
            if (hit4.collider.name.Contains(otherCreatureName))
                h4 += hitCreatureAdd;
            Debug.DrawLine(position1, hit4.point, Color.red, 0.002f);
        }

        if (hit5.collider != null)
        {
            h5 = Vector2.Distance(hit5.point, bodies[0].transform.position);
            if (hit5.collider.name.Contains(otherCreatureName))
                h5 += hitCreatureAdd;
            Debug.DrawLine(position1, hit5.point, Color.red, 0.002f);
        }

        if (hit6.collider != null)
        {
            h6 = Vector2.Distance(hit6.point, bodies[0].transform.position);
            if (hit6.collider.name.Contains(otherCreatureName))
                h6 += hitCreatureAdd;
            Debug.DrawLine(position1, hit6.point, Color.red, 0.002f);
        }

        if (hit7.collider != null)
        {
            h7 = Vector2.Distance(hit7.point, bodies[0].transform.position);
            if (hit7.collider.name.Contains(otherCreatureName))
                h7 += hitCreatureAdd;
            Debug.DrawLine(position1, hit7.point, Color.red, 0.002f);
        }
        if (hit8.collider != null)
        {
            h8 = Vector2.Distance(hit8.point, bodies[0].transform.position);
            if (hit8.collider.name.Contains(otherCreatureName))
                h8 += hitCreatureAdd;
            Debug.DrawLine(position1, hit8.point, Color.red, 0.002f);
        }
        if (hit9.collider != null)
        {
            h9 = Vector2.Distance(hit9.point, bodies[0].transform.position);
            if (hit9.collider.name.Contains(otherCreatureName))
                h9 += hitCreatureAdd;
            Debug.DrawLine(position1, hit9.point, Color.red, 0.002f);
        }
        if (hit10.collider != null)
        {
            h10 = Vector2.Distance(hit10.point, bodies[0].transform.position);
            if (hit10.collider.name.Contains(otherCreatureName))
                h10 += hitCreatureAdd;
            Debug.DrawLine(position10, hit10.point, Color.red, 0.002f);
        }

        float[] inputValues = {h1, h2,h3,h4,h5,h6,h7,h8,h9,h10}; //gather pole and track data into an array 

        output = net.FireNet(inputValues);

        Vector2 dir = bodies[0].transform.up;

        /*if (output[1] > 0.75f)
            bodies[0].velocity = 2f * dir;
        else if (output[1] < -0.75f)
            bodies[0].velocity = -2f * dir;
        else
        {
            bodies[0].velocity = Vector2.zero;
        }

        if (output[0] > 0.75f)
            bodies[0].angularVelocity = 250f;
        else if (output[0] < -0.75f)
            bodies[0].angularVelocity = -250f;
        else
            bodies[0].angularVelocity = 0f;*/


        if (output[0] > 0)
            bodies[0].angularVelocity = -250f;
        else
            bodies[0].angularVelocity = 250f;

        if (output[1] > 0)
            bodies[0].velocity = -2f * dir;
        else
            bodies[0].velocity = 2f * dir;
        

        doDamage = 5f;

        if (bodies[0].velocity.magnitude == 0f)
            doDamage = 0f;
        /*else if (output[1] < -0.75f)
        {
            doDamage = 100f;
            
        }

        if (h1 != -1)
        {
            h1 = Mathf.Abs(h1);
            if (h1 <= 0.5f)
                damage -= doDamage;
        }

        if (h2 != -1)
        {
            h2 = Mathf.Abs(h2);
            if (h2 <= 0.5f)
                damage -= doDamage;
        }

        if (h3 != -1)
        {
            h3 = Mathf.Abs(h3);
            if (h3 <= 0.5f)
                damage -= doDamage;
        }

        if (h4 != -1)
        {
            h4 = Mathf.Abs(h4);
            if (h4 <= 0.5f)
                damage -= doDamage;
        }

        if (h5 != -1)
        {
            h5 = Mathf.Abs(h5);
            if (h5 <= 0.5f)
                damage -= doDamage;
        }

        if (h6 != -1)
        {
            h6 = Mathf.Abs(h6);
            if (h6 <= 0.5f)
                damage -= doDamage;
        }

        if (h7 != -1)
        {
            h7 = Mathf.Abs(h7);
            if (h7 <= 0.5f)
                damage -= doDamage;
        }

        if (h8 != -1)
        {
            h8 = Mathf.Abs(h8);
            if (h8 <= 0.5f)
                damage -= doDamage;
        }

        if (h9 != -1)
        {
            h9 = Mathf.Abs(h9);
            if (h9 <= 0.5f)
                damage -= doDamage;
        }

        if (h10 != -1)
        {
            h10 = Mathf.Abs(h10);
            if (h10 <= 0.5f)
                damage -= doDamage;
        }*/

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
        }

        if (Mathf.Abs(bodies[1].transform.localPosition.x) >5f)
        {
            return true;
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
        if (speedFactor < 0.1f)
            speedFactor = 0.1f;
        speedFactor = 1f / speedFactor;
       

        float boardFactor = Mathf.Abs(bodies[0].transform.localPosition.x);
        if (boardFactor < 0.1f)
            boardFactor = 0.1f;
        boardFactor = 1f / boardFactor;

        //factor = factor * pole1Factor * pole2Factor * boardFactor * speedFactor;
        factor = factor * pole1Factor * pole2Factor * boardFactor;
        //factor = factor * factor;
        float fit = Time.deltaTime + factor*Time.deltaTime;

        net.AddNetFitness(fit);*/


        /*if (bodies[0].angularVelocity == 0)
            fit = fit * 2f;*/
        /*if (bodies[0].velocity.magnitude < 0.5f)
            fit = fit / 2f;*/

        /*if (Mathf.Abs(output[0]) > 0.05f)
            this.net.AddNetFitness(-1f);
        if (output[1] < 0f )
            this.net.AddNetFitness(-1f);*/

        this.net.AddNetFitness(Time.deltaTime);
    }

    
    //--Add your own neural net fail code here--//
    //Final fitness calculations
    private void CalculateFitnessOnFinish() {
        //this.net.AddNetFitness(Mathf.Pow((1f / (float)net.GetGeneCount()), 4f));
        float totalDistanceFit = 0;
       for (int i = 1; i < points.Count; i++) {
            float dis = Mathf.Pow(Vector2.Distance(points[i],points[i-1]),2f);

            /*this.net.AddNetFitness(dis);*/
            totalDistanceFit += dis;
        }
        float life = this.net.GetNetFitness();
        float ratio = totalDistanceFit/life;
        float newFit = ratio * totalDistanceFit;
        this.net.SetNetFitness(newFit);

    }

    public void OtherActivity(int type) {
        mutex.WaitOne();
        if (type == 0)
            OnFinished();
        else if (type == 1)
            OnFinished();//state++;
        else if (type == 2)
            state--;
        mutex.Release();
    }

    public NEATNet GetNet() {
        return net;
    }

}

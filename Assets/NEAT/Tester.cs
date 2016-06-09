using UnityEngine;
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

    public Transform pos;

    void Start()
    {
        mutex = new Semaphore(1, 1); 
        pos = GameObject.Find("Pos").transform;
        //TakePoint();
        //bodies[0].transform.eulerAngles = new Vector3(0f, 0f, UnityEngine.Random.Range(0f,360f));
    }

    void TakePoint() {
        points.Add(bodies[0].transform.position);
        Invoke("TakePoint",1f);
    }

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
            CalculateFitnessOnFinish();
            TestFinished(net.GetNetID(), EventArgs.Empty);
            Destroy(gameObject);
        }
    }

    float h1 = -1f, h2 = -1f, h3 = -1f, h4 = -1f, h5 = -1f, h6 = -1f, h7 = -1f, h8 = -1f, h9 = -1f, h10 = -1f;
    float[] output;
    float state = 0f;
    float hurt = 0f;
    //--Add your own neural net update code here--//
    //Updates nerual net with new inputs from the agent
    private void UpdateNet() {
        /*float boardVelocity = bodies[0].velocity.x; //get current velocity of the board
        //both poles angles in radians
        float pole1AngleRadian = Mathf.Deg2Rad * bodies[1].transform.eulerAngles.z;
        float pole2AngleRadian = Mathf.Deg2Rad * bodies[2].transform.eulerAngles.z;

        //both poles angular velocities 
        float pole1AngularVelocity = bodies[1].angularVelocity;
        float pole2AngularVelocity = bodies[2].angularVelocity;

        float boardLocation = bodies[0].transform.localPosition.x;

        float[] inputValues = { boardVelocity, pole1AngleRadian, pole2AngleRadian, pole1AngularVelocity, pole2AngularVelocity }; //gather pole and track data into an array 
        float[] output = net.FireNet(inputValues); //caluclate new neural net output with given input values
        Vector2 velo = bodies[0].velocity;
        velo += new Vector2(output[0], 0);
        bodies[0].velocity = velo;*/

        /*float boardVelocity = bodies[0].velocity.x; //get current velocity of the board
        //both poles angles in radians
        float pole1AngleRadian = Mathf.Deg2Rad * bodies[1].transform.eulerAngles.z;

        //both poles angular velocities 
        float pole1AngularVelocity = bodies[1].angularVelocity;


        float boardLocation = bodies[0].transform.localPosition.x;

        float[] inputValues = { boardVelocity, pole1AngleRadian, pole1AngularVelocity}; //gather pole and track data into an array 
        float[] output = net.FireNet(inputValues); //caluclate new neural net output with given input values
        Vector2 velo = bodies[0].velocity;
        velo += new Vector2(output[0], 0);
        bodies[0].velocity = velo;*/

        //bodies[0].velocity += new Vector2(output[0], 0); //update track velocity with neural net output

        float angle = -100f;
        float angleAdd = 22.22f;

        Vector3 dir1 = Quaternion.AngleAxis(angle, Vector3.forward) * bodies[0].transform.up;
        angle += angleAdd;
        Vector3 dir2 = Quaternion.AngleAxis(angle, Vector3.forward) * bodies[0].transform.up;
        angle += angleAdd;
        Vector3 dir3 = Quaternion.AngleAxis(angle, Vector3.forward) * bodies[0].transform.up;
        angle += angleAdd;
        Vector3 dir4 = Quaternion.AngleAxis(angle, Vector3.forward) * bodies[0].transform.up;
        angle += angleAdd;
        Vector3 dir5 = Quaternion.AngleAxis(angle, Vector3.forward) * bodies[0].transform.up;
        angle += angleAdd;
        Vector3 dir6 = Quaternion.AngleAxis(angle, Vector3.forward) * bodies[0].transform.up;
        angle += angleAdd;
        Vector3 dir7 = Quaternion.AngleAxis(angle, Vector3.forward) * bodies[0].transform.up;
        angle += angleAdd;
        Vector3 dir8 = Quaternion.AngleAxis(angle, Vector3.forward) * bodies[0].transform.up;
        angle += angleAdd;
        Vector3 dir9 = Quaternion.AngleAxis(angle, Vector3.forward) * bodies[0].transform.up;



        int ignoreLayer = ~(1 << 8);
        Vector2 position1 = bodies[0].transform.position + (0.35f * dir1);
        RaycastHit2D hit1 = Physics2D.Raycast(position1, dir1, 2f, ignoreLayer);

        Vector2 position2 = bodies[0].transform.position + (0.35f * dir2);
        RaycastHit2D hit2 = Physics2D.Raycast(position2, dir2, 2f, ignoreLayer);

        Vector2 position3 = bodies[0].transform.position + (0.35f * dir3);
        RaycastHit2D hit3 = Physics2D.Raycast(position3, dir3, 2f, ignoreLayer);

        Vector2 position4 = bodies[0].transform.position + (0.35f * dir4);
        RaycastHit2D hit4 = Physics2D.Raycast(position4, dir4, 2f, ignoreLayer);

        Vector2 position5 = bodies[0].transform.position + (0.35f * dir5);
        RaycastHit2D hit5 = Physics2D.Raycast(position5, dir5, 2f, ignoreLayer);

        Vector2 position6 = bodies[0].transform.position + (0.35f * dir6);
        RaycastHit2D hit6 = Physics2D.Raycast(position6, dir6, 2f, ignoreLayer);

        Vector2 position7 = bodies[0].transform.position + (0.35f * dir7);
        RaycastHit2D hit7 = Physics2D.Raycast(position7, dir7, 2f, ignoreLayer);

        Vector2 position8 = bodies[0].transform.position + (0.35f * dir8);
        RaycastHit2D hit8 = Physics2D.Raycast(position8, dir8, 2f, ignoreLayer);

        Vector2 position9 = bodies[0].transform.position + (0.35f * dir9);
        RaycastHit2D hit9 = Physics2D.Raycast(position9, dir9, 2f, ignoreLayer);

        Vector3 dir10 = Quaternion.AngleAxis(180f, Vector3.forward) * bodies[0].transform.up;
        Vector2 position10 = bodies[0].transform.position + (0.3f * dir10);
        RaycastHit2D hit10 = Physics2D.Raycast(position10, dir10, 2f, ignoreLayer);

        h1 = -1f; h2 = -1f; h3 = -1f; h4 = -1f; h5 = -1f; h6 = -1f; h7 = -1f; h8 = -1f; h9 = -1f; h10 = -1f;
        float hitCreatureAdd = 0f;
        string otherCreatureName = "B";
        if (hit1.collider != null) {
            h1 = Vector2.Distance(hit1.point, bodies[0].transform.position) / 2f;
            if (h1 != -1)
                Debug.DrawLine(position1, hit1.point, Color.red, 0.002f);
        }

        if (hit2.collider != null)
        {
            h2 = Vector2.Distance(hit2.point, bodies[0].transform.position) / 2f;
            if (h2 != -1)
                Debug.DrawLine(position2, hit2.point, Color.red, 0.002f);
        }

        if (hit3.collider != null)
        {
            h3 = Vector2.Distance(hit3.point, bodies[0].transform.position) / 2f;
            if (h3 != -1)
                Debug.DrawLine(position3, hit3.point, Color.red, 0.002f);
        }

        if (hit4.collider != null)
        {
            h4 = Vector2.Distance(hit4.point, bodies[0].transform.position) / 2f;
            if (h4 != -1)
                Debug.DrawLine(position4, hit4.point, Color.red, 0.002f);
        }

        if (hit5.collider != null)
        {
            h5 = Vector2.Distance(hit5.point, bodies[0].transform.position) / 2f;
            if (h5 != -1)
                Debug.DrawLine(position5, hit5.point, Color.red, 0.002f);
        }

        if (hit6.collider != null)
        {
            h6 = Vector2.Distance(hit6.point, bodies[0].transform.position) / 2f;
            if (h6 != -1)
                Debug.DrawLine(position6, hit6.point, Color.red, 0.002f);
        }

        if (hit7.collider != null)
        {
            h7 = Vector2.Distance(hit7.point, bodies[0].transform.position) / 2f;
            if (h7 != -1)
                Debug.DrawLine(position7, hit7.point, Color.red, 0.002f);
        }
        if (hit8.collider != null)
        {
            h8 = Vector2.Distance(hit8.point, bodies[0].transform.position) / 2f;
            if (h8 != -1)
                Debug.DrawLine(position8, hit8.point, Color.red, 0.002f);
        }
        if (hit9.collider != null)
        {
            h9 = Vector2.Distance(hit9.point, bodies[0].transform.position) / 2f;
            if (h9 != -1)
                Debug.DrawLine(position9, hit9.point, Color.red, 0.002f);
        }
        /*if (hit10.collider != null)
        {
            h10 = Vector2.Distance(hit10.point, bodies[0].transform.position) / 2f;
            if (h10 != -1)
                Debug.DrawLine(position10, hit10.point, Color.red, 0.002f);
        }*/


        Vector2 dir = bodies[0].transform.up;
        //Vector2 dirV = bodies[0].velocity;


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

        rad2 *= Mathf.Deg2Rad;

        int act = -1;
        if (hurt > 0)
            act = 1;

        float counter = 1;


        /*if (h1 >= 0)
            rad2 = Mathf.PI;
        if (h2 >= 0)
            rad2 = Mathf.PI;
        if (h3 >= 0)
            rad2 = Mathf.PI;
        if (h4 >= 0)
            rad2 = Mathf.PI;
        if (h5 >= 0)
            rad2 = Mathf.PI;
        if (h6 >= 0)
            rad2 = Mathf.PI;
        if (h7 >=0)
            rad2 = Mathf.PI;
        if (h8 >=0)
            rad2 = Mathf.PI;
        if (h9 >= 0)
            rad2 = Mathf.PI;*/

        

        float dis = Vector2.Distance(bodies[0].transform.position, pos.position);
       
        float[] inputValues = {h1, h2, h3, h4, h5, h6, h7, h8, h9, rad2, dis};

        output = net.FireNet(inputValues);


        if (output[0] > 0f)
            bodies[0].angularVelocity = 250f;
        else if (output[0] < 0f)
            bodies[0].angularVelocity = -250f;


       // if (h5 == -1 ) {
            this.net.AddNetFitness(Mathf.Pow((1f / Vector2.Distance(bodies[0].transform.position, pos.position)), 2) * Time.deltaTime);
           
        //}

        this.net.AddTimeLived(Time.deltaTime);

        //if ( h5 == -1 ) { 
            //this.net.AddNetFitness(Mathf.Pow(Time.deltaTime * (Mathf.PI - Mathf.Abs(rad2)),4 ));
        //}


        //else
        //bodies[0].angularVelocity = 0f;
        //rad2 = Mathf.Abs(rad2);

        //if(h1 == -1 && h5 == -1 && h6==-1 && h9==-1)


        /*int[] id = net.GetNetID();
        if(id[0] == 0 && id[1]==0)
        Debug.Log(rad2);*/
        if (output[1] > 0)
            bodies[0].velocity = 2f * dir;
        else
            bodies[0].velocity = -2f * dir;

        //bodies[0].velocity = 2f * dir;

        if (hurt > 0f) {
            hurt -= 0.1f;
            damage -= 5f;
        }

        if (hurt < 0)
            hurt = 0f;
        
        //

        /*Vector2 dir = bodies[0].transform.up;
        Vector2 posi = bodies[0].transform.position;
        Vector2 deltaVector = pos.position - bodies[0].transform.position;
        deltaVector = deltaVector.normalized;

        float rad1 = bodies[0].transform.eulerAngles.z * Mathf.Deg2Rad;
        float dis = Vector2.Distance(posi, pos.position);
        float rad2 = Mathf.Atan2(deltaVector.y, deltaVector.x);
        float rad3 = Mathf.Atan2(deltaVector.y - dir.y, deltaVector.x - dir.x);

        rad2 *= Mathf.Rad2Deg;
        rad2 = 90f - rad2;

        if (rad2 < 0f) {
            rad2 += 360f;
        }
        rad2 = 360 - rad2;

        rad2 -= bodies[0].transform.eulerAngles.z;
        if (rad2 < 0)
            rad2 = 360 + rad2;


        if (rad2 >= 180f) {
            rad2 = 360 - rad2;
        }

        rad2 *= Mathf.Deg2Rad;


        float[] inputValues = {rad2,dis};
        output = net.FireNet(inputValues);

        if (output[0] > 0.66f)
            bodies[0].angularVelocity = 200f ;
        else if (output[0] < -0.66f)
            bodies[0].angularVelocity = -200f ;
        else 
             bodies[0].angularVelocity = 0f;

        if (output[1] > 0.5f)
            bodies[0].velocity = 2f * dir;
        else
            bodies[0].velocity = Vector2.zero;*/



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

        /*if (Mathf.Abs(bodies[0].transform.localPosition.x) > 5f) {
            return true;
        }*/

        /*float failDegree = 10f;
        float pole1AngleDegree = bodies[1].transform.eulerAngles.z;
        if (!(((pole1AngleDegree <= failDegree && pole1AngleDegree >= 0) || (pole1AngleDegree <= 360 && pole1AngleDegree >= (360 - failDegree))))) {
            if(isUp == true)
                return true;
        }*/

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

        //if (net.GetNetID()[0] == 0 && net.GetNetID()[1] == 0)
        //Debug.Log(Time.deltaTime);

        /*if (bodies[0].angularVelocity == 0)
            fit = fit * 2f;*/
        /*if (bodies[0].velocity.magnitude < 0.5f)
            fit = fit / 2f;*/

        /*if (Mathf.Abs(output[0]) > 0.05f)
            this.net.AddNetFitness(-1f);
        if (output[1] < 0f )
            this.net.AddNetFitness(-1f);*/

        
        
    }


    //--Add your own neural net fail code here--//
    //Final fitness calculations
    private void CalculateFitnessOnFinish() {


        float life = this.net.GetTimeLived();
        life = (life / net.GetTestTime()) * 2f;

        float fit = this.net.GetNetFitness();
        fit = Mathf.Pow(fit,life);

        this.net.SetNetFitness(fit);
        //this.net.SetNetFitness(Mathf.Pow((1f / Vector2.Distance(bodies[0].transform.position, pos.position)), life));

        /*float life = this.net.GetNetFitness();
        float factor = (life / net.GetTestTime())*2f;

        float totalDistanceFit = 0;
        for (int i = 1; i < points.Count; i++) {
            float dis = Mathf.Pow(Vector2.Distance(points[i],points[i-1]), factor);
            totalDistanceFit += dis;
        }
        
        //float ratio = totalDistanceFit/life;
       // float newFit = ratio * totalDistanceFit;

        this.net.SetNetFitness(totalDistanceFit);*/

    }

    public void OtherActivity(int type) {
        mutex.WaitOne();

        if(hurt<=0)
            hurt = 1f;

        mutex.Release();
    }

    public NEATNet GetNet() {
        return net;
    }

}

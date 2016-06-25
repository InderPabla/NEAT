using UnityEngine;
using System;
using System.Collections.Generic;
//using System.Threading;

public class Tester : MonoBehaviour
{

    public List<Rigidbody2D> bodies;

    private NEATNet net;
    private bool isActive = false;
    private bool isLoaded = false;
    private const string ON_FINISHED = "OnFinished";
    //private Semaphore mutex; 
    public delegate void TestFinishedEventHandler(object source, EventArgs args);
    public event TestFinishedEventHandler TestFinished;

    List<Vector2> points = new List<Vector2>();
    float damage = 100f;

    public Transform pos;
    bool finished = false;
    void Start()
    {
        //mutex = new Semaphore(1, 1);

        //pos = GameObject.Find("Pos").transform;
        //TakePoint();
        bodies[0].transform.eulerAngles = new Vector3(0f, 0f, UnityEngine.Random.Range(0f,360f));
        //transform.eulerAngles = new Vector3(0f, 0f, UnityEngine.Random.Range(0f, 360f));
        //Invoke("TakePoint", 1f);
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

    float h1 = -1f, h2 = -1f, h3 = -1f, h4 = -1f, h5 = -1f, h6 = -1f, h7 = -1f, h8 = -1f, h9 = -1f, h10 = -1f;
    float[] output;
    float state = 0f;
    float hurt = 0f;
    float bodyRad, leftUpRad1, leftDownRad1, rightUpRad1, rightDownRad1, leftUpRad2, leftDownRad2, rightUpRad2, rightDownRad2,
        jointAngle1, jointAngle2, jointAngle3, jointAngle4, jointAngle5, jointAngle6, jointAngle7, jointAngle8;

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


        //bodies[0].velocity += new Vector2(output[0], 0); //update track velocity with neural net output

        /*float angle = -100f;
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
        if (hit10.collider != null)
        {
            h10 = Vector2.Distance(hit10.point, bodies[0].transform.position) / 2f;
            if (h10 != -1)
                Debug.DrawLine(position10, hit10.point, Color.red, 0.002f);
        }


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

        float dis = Vector2.Distance(bodies[0].transform.position, pos.position);
       
        float[] inputValues = {h1, h2, h3, h4, h5, h6, h7, h8, h9, rad2};

        output = net.FireNet(inputValues);


        if (output[0] > 0f)
            bodies[0].angularVelocity = 250f;
        else if (output[0] < 0f)
            bodies[0].angularVelocity = -250f;

        this.net.AddNetFitness(Mathf.Pow((1f / Vector2.Distance(bodies[0].transform.position, pos.position)), 2) * Time.deltaTime);
           
 
        this.net.AddTimeLived(Time.deltaTime);


        if (output[1] > 0)
            bodies[0].velocity = 2f * dir;
        else
            bodies[0].velocity = 0.1f * dir;

        if (hurt > 0f) {
            hurt -= 0.1f;
            damage -= 5f;
        }

        if (hurt < 0)
            hurt = 0f;*/

       
        if (start == false) {
            UpdateOverTime();
            start = true;
        }
        damage -= Time.deltaTime * 30;
    }

    float stress = 0f;
    public void UpdateOverTime() {
        /*bodyRad = Mathf.Deg2Rad * bodies[0].transform.eulerAngles.z;
        leftUpRad1 = Mathf.Deg2Rad * bodies[1].transform.eulerAngles.z;
        leftDownRad1 = Mathf.Deg2Rad * bodies[2].transform.eulerAngles.z;
        rightUpRad1 = Mathf.Deg2Rad * bodies[3].transform.eulerAngles.z;
        rightDownRad1 = Mathf.Deg2Rad * bodies[4].transform.eulerAngles.z;

        leftUpRad2 = Mathf.Deg2Rad * bodies[5].transform.eulerAngles.z;
        leftDownRad2 = Mathf.Deg2Rad * bodies[6].transform.eulerAngles.z;
        rightUpRad2 = Mathf.Deg2Rad * bodies[7].transform.eulerAngles.z;
        rightDownRad2 = Mathf.Deg2Rad * bodies[8].transform.eulerAngles.z;

       jointAngle1 = bodies[1].GetComponent<HingeJoint2D>().jointAngle * Mathf.Deg2Rad;
       jointAngle2 = bodies[2].GetComponent<HingeJoint2D>().jointAngle * Mathf.Deg2Rad;
       jointAngle3 = bodies[3].GetComponent<HingeJoint2D>().jointAngle * Mathf.Deg2Rad;
       jointAngle4 = bodies[4].GetComponent<HingeJoint2D>().jointAngle * Mathf.Deg2Rad;
       jointAngle5 = bodies[5].GetComponent<HingeJoint2D>().jointAngle * Mathf.Deg2Rad;
       jointAngle6 = bodies[6].GetComponent<HingeJoint2D>().jointAngle * Mathf.Deg2Rad;
       jointAngle7 = bodies[7].GetComponent<HingeJoint2D>().jointAngle * Mathf.Deg2Rad;
       jointAngle8 = bodies[8].GetComponent<HingeJoint2D>().jointAngle * Mathf.Deg2Rad;

       float[] inputValues = { bodyRad, jointAngle1, jointAngle2, jointAngle3, jointAngle4, jointAngle5, jointAngle6, jointAngle7, jointAngle8, sensorTouchLeft1, sensorTouchRight1, sensorTouchLeft2, sensorTouchRight2 };
       //float[] inputValues = { bodyRad, leftUpRad1, leftDownRad1, rightUpRad1, rightDownRad1, leftUpRad2, leftDownRad2, rightUpRad2, rightDownRad2, sensorTouchLeft1, sensorTouchRight1, sensorTouchLeft2, sensorTouchRight2 };
       output = net.FireNet(inputValues);


       float motorSpeed1, motorSpeed2, motorSpeed3, motorSpeed4, motorSpeed5, motorSpeed6, motorSpeed7, motorSpeed8;
       float threshold = 0.75f;
       float motorPower = 500f;

       if (output[0] > threshold)
           motorSpeed1 = motorPower;
       else if (output[0] < -threshold)
           motorSpeed1 = -motorPower;
       else 
           motorSpeed1 = 0f;

       if (output[1] > threshold)
           motorSpeed2 = motorPower;
       else if (output[1] < -threshold)
           motorSpeed2 = -motorPower;
       else
           motorSpeed2 = 0f;

       if (output[2] > threshold)
           motorSpeed3 = motorPower;
       else if (output[2] < -threshold)
           motorSpeed3 = -motorPower;
       else
           motorSpeed3 = 0f;

       if (output[3] > threshold)
           motorSpeed4 = motorPower;
       else if (output[3] < -threshold)
           motorSpeed4 = -motorPower;
       else
           motorSpeed4 = 0f;

       if (output[4] > threshold)
           motorSpeed5 = motorPower;
       else if (output[4] < -threshold)
           motorSpeed5 = -motorPower;
       else
           motorSpeed5 = 0f;

       if (output[5] > threshold)
           motorSpeed6 = motorPower;
       else if (output[5] < -threshold)
           motorSpeed6 = -motorPower;
       else
           motorSpeed6 = 0f;

       if (output[6] > threshold)
           motorSpeed7 = motorPower;
       else if (output[6] < -threshold)
           motorSpeed7 = -motorPower;
       else
           motorSpeed7 = 0f;
       if (output[7] > threshold)
           motorSpeed8 = motorPower;
       else if (output[7] < -threshold)
           motorSpeed8 = -motorPower;
       else
           motorSpeed8 = 0f;

       JointMotor2D motor1 = bodies[1].GetComponent<HingeJoint2D>().motor;
       motor1.motorSpeed = motorSpeed1;
       if (motorSpeed1 == 0)
           bodies[1].GetComponent<HingeJoint2D>().useMotor = false;
       else
           bodies[1].GetComponent<HingeJoint2D>().useMotor = true;
       bodies[1].GetComponent<HingeJoint2D>().motor = motor1;

       JointMotor2D motor2 = bodies[2].GetComponent<HingeJoint2D>().motor;
       motor2.motorSpeed = motorSpeed2;
       if (motorSpeed2 == 0)
           bodies[2].GetComponent<HingeJoint2D>().useMotor = false;
       else
           bodies[2].GetComponent<HingeJoint2D>().useMotor = true;
       bodies[2].GetComponent<HingeJoint2D>().motor = motor2;

       JointMotor2D motor3 = bodies[3].GetComponent<HingeJoint2D>().motor;
       motor3.motorSpeed = motorSpeed3;
       if (motorSpeed3 == 0)
           bodies[3].GetComponent<HingeJoint2D>().useMotor = false;
       else
           bodies[3].GetComponent<HingeJoint2D>().useMotor = true;
       bodies[3].GetComponent<HingeJoint2D>().motor = motor3;

       JointMotor2D motor4 = bodies[4].GetComponent<HingeJoint2D>().motor;
       motor4.motorSpeed = motorSpeed4;
       if (motorSpeed4 == 0)
           bodies[4].GetComponent<HingeJoint2D>().useMotor = false;
       else
           bodies[4].GetComponent<HingeJoint2D>().useMotor = true;
       bodies[4].GetComponent<HingeJoint2D>().motor = motor4;

       JointMotor2D motor5 = bodies[5].GetComponent<HingeJoint2D>().motor;
       motor5.motorSpeed = motorSpeed5;
       if (motorSpeed5 == 0)
           bodies[5].GetComponent<HingeJoint2D>().useMotor = false;
       else
           bodies[5].GetComponent<HingeJoint2D>().useMotor = true;
       bodies[5].GetComponent<HingeJoint2D>().motor = motor5;

       JointMotor2D motor6 = bodies[6].GetComponent<HingeJoint2D>().motor;
       motor6.motorSpeed = motorSpeed6;
       if (motorSpeed6 == 0)
           bodies[6].GetComponent<HingeJoint2D>().useMotor = false;
       else
           bodies[6].GetComponent<HingeJoint2D>().useMotor = true;
       bodies[6].GetComponent<HingeJoint2D>().motor = motor6;

       JointMotor2D motor7 = bodies[7].GetComponent<HingeJoint2D>().motor;
       motor7.motorSpeed = motorSpeed7;
       if (motorSpeed7 == 0)
           bodies[7].GetComponent<HingeJoint2D>().useMotor = false;
       else
           bodies[7].GetComponent<HingeJoint2D>().useMotor = true;
       bodies[7].GetComponent<HingeJoint2D>().motor = motor7;

       JointMotor2D motor8 = bodies[8].GetComponent<HingeJoint2D>().motor;
       motor8.motorSpeed = motorSpeed8;
       if (motorSpeed8 == 0)
           bodies[8].GetComponent<HingeJoint2D>().useMotor = false;
       else
           bodies[8].GetComponent<HingeJoint2D>().useMotor = true;
       bodies[8].GetComponent<HingeJoint2D>().motor = motor8;*/

        float angle = -100f;
        float angleAdd = 22.22f;
        float distance = 10f;
        int ignoreFoodLayer = ~(1 << 8);
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

        Vector2 position1 = bodies[0].transform.position + (0.35f * dir1);
        RaycastHit2D hit1 = Physics2D.Raycast(position1, dir1, distance, ignoreFoodLayer);

        Vector2 position2 = bodies[0].transform.position + (0.35f * dir2);
        RaycastHit2D hit2 = Physics2D.Raycast(position2, dir2, distance, ignoreFoodLayer);

        Vector2 position3 = bodies[0].transform.position + (0.35f * dir3);
        RaycastHit2D hit3 = Physics2D.Raycast(position3, dir3, distance, ignoreFoodLayer);

        Vector2 position4 = bodies[0].transform.position + (0.35f * dir4);
        RaycastHit2D hit4 = Physics2D.Raycast(position4, dir4, distance, ignoreFoodLayer);

        Vector2 position5 = bodies[0].transform.position + (0.35f * dir5);
        RaycastHit2D hit5 = Physics2D.Raycast(position5, dir5, distance, ignoreFoodLayer);

        Vector2 position6 = bodies[0].transform.position + (0.35f * dir6);
        RaycastHit2D hit6 = Physics2D.Raycast(position6, dir6, distance, ignoreFoodLayer);

        Vector2 position7 = bodies[0].transform.position + (0.35f * dir7);
        RaycastHit2D hit7 = Physics2D.Raycast(position7, dir7, distance, ignoreFoodLayer);

        Vector2 position8 = bodies[0].transform.position + (0.35f * dir8);
        RaycastHit2D hit8 = Physics2D.Raycast(position8, dir8, distance, ignoreFoodLayer);

        Vector2 position9 = bodies[0].transform.position + (0.35f * dir9);
        RaycastHit2D hit9 = Physics2D.Raycast(position9, dir9, distance, ignoreFoodLayer);

        Vector3 dir10 = Quaternion.AngleAxis(180f, Vector3.forward) * bodies[0].transform.up;
        Vector2 position10 = bodies[0].transform.position + (0.3f * dir10);
        RaycastHit2D hit10 = Physics2D.Raycast(position10, dir10, distance, ignoreFoodLayer);

        h1 = -1f; h2 = -1f; h3 = -1f; h4 = -1f; h5 = -1f; h6 = -1f; h7 = -1f; h8 = -1f; h9 = -1f; h10 = -1f;
        bool draw = true;
        float hitCreatureAdd = 0f;
        string otherCreatureName = "B";
        if (hit1.collider != null /*&& hit1.collider.name.StartsWith("F")*/)
        {
            h1 = Vector2.Distance(hit1.point, bodies[0].transform.position) / distance;
            if (h1 != -1 && draw == true)
                Debug.DrawLine(position1, hit1.point, Color.red, 0.002f);
        }

        if (hit2.collider != null /*&& hit2.collider.name.StartsWith("F")*/)
        {
            h2 = Vector2.Distance(hit2.point, bodies[0].transform.position) / distance;
            if (h2 != -1 && draw == true)
                Debug.DrawLine(position2, hit2.point, Color.red, 0.002f);
        }

        if (hit3.collider != null /*&& hit3.collider.name.StartsWith("F")*/)
        {
            h3 = Vector2.Distance(hit3.point, bodies[0].transform.position) / distance;
            if (h3 != -1 && draw == true)
                Debug.DrawLine(position3, hit3.point, Color.red, 0.002f);
        }

        if (hit4.collider != null /*&& hit4.collider.name.StartsWith("F")*/)
        {
            h4 = Vector2.Distance(hit4.point, bodies[0].transform.position) / distance;
            if (h4 != -1 && draw == true)
                Debug.DrawLine(position4, hit4.point, Color.red, 0.002f);
        }

        if (hit5.collider != null /*&& hit5.collider.name.StartsWith("F")*/)
        {
            h5 = Vector2.Distance(hit5.point, bodies[0].transform.position) / distance;
            if (h5 != -1 && draw == true)
                Debug.DrawLine(position5, hit5.point, Color.red, 0.002f);
        }

        if (hit6.collider != null /*&& hit6.collider.name.StartsWith("F")*/)
        {
            h6 = Vector2.Distance(hit6.point, bodies[0].transform.position) / distance;
            if (h6 != -1 && draw == true)
                Debug.DrawLine(position6, hit6.point, Color.red, 0.002f);
        }

        if (hit7.collider != null /*&& hit7.collider.name.StartsWith("F")*/)
        {
            h7 = Vector2.Distance(hit7.point, bodies[0].transform.position) / distance;
            if (h7 != -1 && draw == true)
                Debug.DrawLine(position7, hit7.point, Color.red, 0.002f);
        }
        if (hit8.collider != null /*&& hit8.collider.name.StartsWith("F")*/)
        {
            h8 = Vector2.Distance(hit8.point, bodies[0].transform.position) / distance;
            if (h8 != -1 && draw == true)
                Debug.DrawLine(position8, hit8.point, Color.red, 0.002f);
        }
        if (hit9.collider != null /*&& hit9.collider.name.StartsWith("F")*/)
        {
            h9 = Vector2.Distance(hit9.point, bodies[0].transform.position) / distance;
            if (h9 != -1 && draw == true)
                Debug.DrawLine(position9, hit9.point, Color.red, 0.002f);
        }
        if (hit10.collider != null /*&& hit10.collider.name.StartsWith("F")*/)
        {
            h10 = Vector2.Distance(hit10.point, bodies[0].transform.position) / distance;
            if (h10 != -1 && draw == true)
                Debug.DrawLine(position10, hit10.point, Color.red, 0.002f);
        }

        Vector2 dir = bodies[0].transform.up;
        float[] inputValues = { h1, h2, h3, h4, h5, h6, h7, h8, h9, (damage / 100f), bodies[0].transform.eulerAngles.z * Mathf.Deg2Rad };
        output = net.FireNet(inputValues);

        bodies[0].angularVelocity = output[0] * 250f;
        if (output[1] >= 0)
            bodies[0].velocity = dir * output[1] * 5f;
        else
            bodies[0].velocity /= 2f;

        
        /*stress = 0f;
        if (output[2]>0)
            stress = output[2];
        float time = Mathf.Clamp(stress, 0.01f, 0.1f);
        if (time < 0.01f)
            time = 0.01f;*/

        Invoke("UpdateOverTime", 0.08f);
    }



    float sensorTouchLeft1, sensorTouchRight1, sensorTouchLeft2, sensorTouchRight2;
    float fail = 0;
    public void LegTouch(int type) {
        /*if(type == -1)
            sensorTouchLeft1 = -1f;
        else if (type == 1)  
            sensorTouchLeft1 = 1f;
        else if (type == -2)
            sensorTouchLeft2 = -1f;
        else if (type == 2)
            sensorTouchLeft2 = 1f;
        else if (type == -3)
            sensorTouchRight1 = -1f;
        else if (type == 3)
            sensorTouchRight1 = 1f;
        else if (type == -4)
            sensorTouchRight2 = -1f;
        else if (type == 4)
            sensorTouchRight2 = 1f;

        if (type == 0)
            damage = 0;*/
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

        net.AddNetFitness(Time.deltaTime);
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
        //mutex.WaitOne();
        if (type == 0) {
            net.AddNetFitness(0.1f);
            damage = 100f;
            
        }
        if (type == 1) {
            net.SetNetFitness(net.GetNetFitness()*0f);
            OnFinished();
        }
        /* (type == 1)
        {
            damage -= 10f;
        }*/
        //if(hurt<=0)
        //hurt = 1f;

        //mutex.Release();
    }

    public NEATNet GetNet() {
        return net;
    }

}

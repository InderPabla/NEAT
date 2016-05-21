using UnityEngine;
using System.Collections;

public class Testingstuff : MonoBehaviour {

    public Transform pos;
    Rigidbody2D r;

    void Start()
    {
        pos = GameObject.Find("Pos").transform;
        r = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void FixedUpdate () {
        //Debug.Log(transform.up);
        Vector2 dir = transform.up;
        Vector2 posi = transform.position;
        Vector2 deltaVector = pos.position - transform.position;
        deltaVector = deltaVector.normalized;

        float rad1 = transform.eulerAngles.z * Mathf.Deg2Rad;
        float dis = Vector2.Distance(posi, pos.position);
        float rad2 = Mathf.Atan2(deltaVector.y, deltaVector.x);

        rad2 *= Mathf.Rad2Deg;


        rad2 = 90f - rad2;

        if (rad2 < 0f)
        {
            rad2 += 360f;
        }
        rad2 = 360 - rad2;

        rad2 -= transform.eulerAngles.z;
        if (rad2 < 0)
            rad2 = 360+rad2;


        if (rad2 >= 180f)
        {
            rad2 = 360 - rad2;
        }

        //float rad3 = Mathf.Atan2(deltaVector.y - dir.y, deltaVector.x - dir.x);


        Debug.Log(rad2);
    }
}

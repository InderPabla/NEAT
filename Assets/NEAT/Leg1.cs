using UnityEngine;
using System.Collections;

public class Leg1 : MonoBehaviour {
    public int stay = 0;
    public int exit = 0;


    /*void OnCollisionEnter2D(Collision2D coll)
    {
        transform.parent.SendMessage("LegTouch", (object)stay);
    }*/

    void OnCollisionStay2D(Collision2D coll)
    {
        transform.parent.SendMessage("LegTouch", (object)stay);
    }

    void OnCollisionExit2D(Collision2D coll)
    {
        transform.parent.SendMessage("LegTouch", (object)exit);
    }
}

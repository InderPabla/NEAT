using UnityEngine;
using System.Collections;

public class CollsionCheck : MonoBehaviour {
    
    void OnCollisionEnter2D(Collision2D coll)
    {
        //if (coll.collider.name.Contains("G"))
        //transform.parent.SendMessage("OtherActivity", (object)0);
        //else
        //transform.parent.SendMessage("OtherActivity", (object)1);

        //if (coll.collider.name.Contains("G"))
            transform.parent.SendMessage("OnFinished");
    }

    void OnCollisionExit2D(Collision2D coll)
    {
        //if (coll.collider.name.Contains("B"))
        //transform.parent.SendMessage("OtherActivity", (object)2);
    }

    void OnCollisionStay2D(Collision2D coll)
    {
        //if (coll.collider.name.Contains("B"))
        //transform.parent.SendMessage("OtherActivity", (object)3);
    }

}

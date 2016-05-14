using UnityEngine;
using System.Collections;

public class CollsionCheck : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.collider.name.Contains("Ground"))
            transform.parent.SendMessage("OnFinished");
    }

}

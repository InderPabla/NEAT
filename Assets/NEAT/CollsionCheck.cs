using UnityEngine;
using System.Collections;

public class CollsionCheck : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.collider.name.Contains("Ground"))
            transform.parent.SendMessage("OnFinished");
        /*else
            transform.parent.SendMessage("OtherActivity", 1);*/
    }
}

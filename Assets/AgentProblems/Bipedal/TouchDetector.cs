using UnityEngine;
using System.Collections;

public class TouchDetector : MonoBehaviour {
    public float touch = -1;

    void OnCollisionExit2D(Collision2D coll)
    {
        touch = -1;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        touch = 1;

    }

}

using UnityEngine;
using System.Collections;

public class BehaveLike2D : MonoBehaviour {
    Rigidbody body;
	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        
        
        Vector3 velo = body.velocity;
        Vector3 angular = body.angularVelocity;
        Vector3 position = transform.position;
        Vector3 angle = transform.eulerAngles;

        position.z = 0f;
        angle.x = 0f;
        angle.y = 0f;
        angular.x = 0;
        angular.y = 0;
        velo.z = 0;

        body.velocity = velo;
        body.angularVelocity = angular;
        transform.position = position;
        transform.eulerAngles = angle;
    }
}

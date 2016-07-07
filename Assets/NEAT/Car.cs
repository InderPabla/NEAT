using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour {
    public GameObject carPrefab;
    // Use this for initialization

    GameObject[] car = new GameObject[100];
	void Start () {
        float dif = -200f;
        for (int i = 0; i < car.Length; i++) {
            car[i] = (GameObject)Instantiate(carPrefab,new Vector3(dif, 0,0),carPrefab.transform.rotation);
            car[i].transform.parent = transform;
            dif += 5f;
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        for (int i = 0; i < car.Length; i++)
        {
            car[i].transform.position += new Vector3(5f*Time.deltaTime,0f,0f);
        }
    }
}

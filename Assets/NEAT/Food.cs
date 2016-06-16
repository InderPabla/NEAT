﻿using UnityEngine;


public class Food : MonoBehaviour {
    public GameObject foodPrefab;
    float resourceCondition = 0f;
    // Use this for initialization


    void Start () {
        for (int i = 0; i < 250; i++)
        {
            GameObject food = (GameObject)Instantiate(foodPrefab, new Vector3(Random.Range(-25f, 25f), Random.Range(-25f, 25f), 0), foodPrefab.transform.rotation);
            food.transform.parent = transform;
        }
        resourceCondition = Random.Range(0.01f,0.1f);
        Debug.Log(resourceCondition);
        Spawn();

    }

    public void Spawn()
    {
        GameObject food = (GameObject)Instantiate(foodPrefab, new Vector3(Random.Range(-25f, 25f), Random.Range(-25f, 25f), 0), foodPrefab.transform.rotation);
        food.transform.parent = transform;
        Invoke("Spawn", resourceCondition);
    }
	// Update is called once per frame
	void Update () {
	
	}

}

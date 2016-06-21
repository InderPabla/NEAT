﻿using UnityEngine;


public class Food : MonoBehaviour {
    public GameObject foodPrefab;
    float resourceCondition = 0f;
    // Use this for initialization


    void Start () {
        for (int i = 0; i < 250; i++)
        {
            GameObject food = (GameObject)Instantiate(foodPrefab, new Vector3(Random.Range(-29f, 29f), Random.Range(-29f, 29f), 0), foodPrefab.transform.rotation);
            food.transform.parent = transform;
        }
        resourceCondition = Random.Range(0.05f,0.1f);
        Debug.Log(resourceCondition);
        Spawn();

    }

    public void Spawn()
    {
        GameObject food = (GameObject)Instantiate(foodPrefab, new Vector3(Random.Range(-29f, 29f), Random.Range(-29f, 29f), 0), foodPrefab.transform.rotation);
        food.transform.parent = transform;
        Invoke("Spawn", resourceCondition);
    }
	// Update is called once per frame
	void Update () {
	
	}

}
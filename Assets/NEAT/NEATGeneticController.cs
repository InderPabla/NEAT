using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;

public class NEATGeneticController : MonoBehaviour
{

    public GameObject testPrefab;

    //input through unity editor
    public int numberOfInputPerceptrons = 0;
    public int numberOfOutputPerceptrons = 0;
    public int populationSize = 0;
    public float testTime = 0;

    private NEATNet[] nets; //array of neural networks 
    private float[,] finishedNetID;
    private int testCounter; //counter for population testing
    private Semaphore finished; //mutex lock for when test if finished and updating test counter
    private const string ACTIVATE = "Activate";
    private int generationNumber = 0;



    void Start()
    {
        for (int i = 0; i < populationSize; i++)
        {
            GameObject tester = (GameObject)Instantiate(testPrefab, new Vector3(0, 0, 0), testPrefab.transform.rotation);
            tester.name = i + "";
        }
    }

    void Update()
    {

    }

    public void GenerateInitialNets()
    {
        for (int i = 0; i < populationSize; i++)
        {
            nets[i] = new NEATNet(i, 0, numberOfInputPerceptrons, numberOfOutputPerceptrons, testTime);
        }
    }

    public void GeneratePopulation()
    {
        for (int i = 0; i < populationSize; i++)
        {
            GameObject agent = (GameObject)Instantiate(testPrefab, new Vector3(0, 0, 0), testPrefab.transform.rotation);
            agent.name = i + "";
            agent.SendMessage(ACTIVATE, nets[i]);
            agent.GetComponent<Tester>().TestFinished += OnFinished; //suscribe OnFinished to event in Balancer
        }
    }

    public void OnFinished(object source, EventArgs e)
    {
        finished.WaitOne();

        testCounter++;
        if (testCounter == populationSize)
        {
            TestFinished();
        }

        finished.Release();
    }

    public void TestFinished()
    {
        testCounter = 0;
        generationNumber++;
        
    }

    public List<int> GenerateListNumbers(int min, int max)
    {
        List<int> unusedIndicies = new List<int>();
        for (int i = min; i <= max; i++)
        {
            unusedIndicies.Add(i);
        }
        return unusedIndicies;
    }

    public int[,] GenerateRandomUniquePaires(List<int> indicies)
    {
        int[,] paires = new int[indicies.Count / 2, 2];
        int count = indicies.Count / 2;

        for (int i = 0; i < count; i++)
        {
            int index1, index2, item1, item2;

            index1 = UnityEngine.Random.Range(0, indicies.Count);
            item1 = indicies[index1];
            indicies.RemoveAt(index1);

            index2 = UnityEngine.Random.Range(0, indicies.Count);
            item2 = indicies[index2];
            indicies.RemoveAt(index2);

            paires[i, 0] = item1;
            paires[i, 1] = item2;
        }

        return paires;
    }

    public void SortFitness()
    {
        float[] tempFitness = new float[2];
        bool swapped = true;
        int j = 0;
        while (swapped)
        {
            swapped = false;
            j++;
            for (int i = 0; i < populationSize - j; i++)
            {
                if (finishedNetID[i, 1] < finishedNetID[i + 1, 1])
                {
                    tempFitness[0] = finishedNetID[i, 0];
                    tempFitness[1] = finishedNetID[i, 1];

                    finishedNetID[i, 0] = finishedNetID[i + 1, 0];
                    finishedNetID[i, 1] = finishedNetID[i + 1, 1];

                    finishedNetID[i + 1, 0] = tempFitness[0];
                    finishedNetID[i + 1, 1] = tempFitness[1];
                    swapped = true;
                }
            }
        }
    }

    public bool ErrorCheck()
    {
        bool error = false;

        if (numberOfInputPerceptrons <= 0)
        {
            Debug.LogError("Need one or more input perceptrons.");
            error = true;
        }

        if (numberOfOutputPerceptrons <= 0)
        {
            Debug.LogError("Need one or more output perceptrons.");
            error = true;
        }

        if (populationSize <= 0)
        {
            Debug.LogError("Population size must be greater than 0.");
            error = true;
        }

        if (populationSize % 2 != 0 || populationSize % 4 != 0)
        {
            Debug.LogError("Population size must be divisible by 2 and 4.");
            error = true;
        }

        if (testTime <= 0)
        {
            Debug.LogError("Time to test must be greater than 0 seconds.");
            error = true;
        }

        if (error)
        {
            Debug.LogError("One or more issues found. Exiting.");
            return true;
        }
        else
        {
            return false;
        }
    }
}

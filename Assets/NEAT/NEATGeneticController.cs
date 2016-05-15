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
    private float[,] finishedResults;
    private int testCounter; //counter for population testing
    private Semaphore finished; //mutex lock for when test if finished and updating test counter
    private const string ACTIVATE = "Activate";
    private int generationNumber = 0;

    public GameObject netDrawer;

    public List<Transform> bodies;
    public bool worldActivation = false;

    void Start() {
        Application.runInBackground = true;
        if (ErrorCheck() == false) {
            testCounter = 0;
            finished = new Semaphore(1, 1);
            nets = new NEATNet[populationSize];
            finishedResults = new float[populationSize, 2];

            GenerateInitialNets();
            GeneratePopulation();
        }
    }

    void FixedUpdate() {
        if (worldActivation) {
            bodies[0].transform.position += new Vector3(Time.deltaTime/2f, 0, 0);
            bodies[1].transform.position += new Vector3(-Time.deltaTime/2f, 0, 0);
            bodies[2].transform.position += new Vector3(0, Time.deltaTime / 8f, 0);
            bodies[3].transform.position += new Vector3(0, -Time.deltaTime / 8f, 0);
        }
    }

    public void ResetWorld() {
        bodies[0].transform.position = new Vector3(-17.5f, 0, 0);
        bodies[1].transform.position = new Vector3(17.5f, 0, 0);
        bodies[2].transform.position = new Vector3(0, -9.5f, 0);
        bodies[3].transform.position = new Vector3(0, 9.5f, 0);
    }


    public void GenerateInitialNets() {
        for (int i = 0; i < populationSize; i++) {
            nets[i] = new NEATNet(i, 0, numberOfInputPerceptrons, numberOfOutputPerceptrons, testTime);
        }
    }

    public void GeneratePopulation() {
        float height = 10f;
        float width = -16f;
        for (int i = 0; i < populationSize; i++) {
            if (width % 16 == 0 && width!=0)
            {
                height-=4;
                width = -12;
            }

            GameObject tester = (GameObject)Instantiate(testPrefab, new Vector3(width, height, 0), testPrefab.transform.rotation);
            width+=2;
            tester.name = i + "";
            tester.SendMessage(ACTIVATE, nets[i]);
            tester.GetComponent<Tester>().TestFinished += OnFinished; //suscribe OnFinished to event in Balancer
        }
    }

    public void OnFinished(object source, EventArgs e)
    {
        finished.WaitOne();
        int netID = (int)source;

        finishedResults[testCounter, 0] = netID;
        finishedResults[testCounter, 1] = nets[netID].GetNetFitness();

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

        NEATNet[] tempNet = new NEATNet[populationSize];

        SortFitness();
        int bestNetIndex = (int)finishedResults[populationSize - 1, 0];
        Debug.Log("Generation Number: " + generationNumber + ", Best Fitness: " + finishedResults[populationSize - 1, 1]);
        Debug.Log("-----"+nets[bestNetIndex].GetNodeCount() +" "+ nets[bestNetIndex].GetGeneCount());
        //if (generationNumber%10==0)
        // nets[bestNetIndex].PrintDetails();
        netDrawer.SendMessage("DrawNet",nets[bestNetIndex]);

        /*for (int i = 0; i < populationSize; i++) {
            Debug.Log((int)finishedResults[i, 0]+" "+finishedResults[i,1]);
        }*/

        int index = 0;
        for (int i = populationSize/2; i < populationSize; i++) {
            NEATNet net = nets[(int)finishedResults[i, 0]];
            NEATNet net1 = NEATNet.CreateMutateCopy(net);
            //NEATNet net2 = NEATNet.CreateMutateCopy(net);
            tempNet[index] = net1;
            tempNet[index+1] = net;
            index += 2;
        }

        for (int i = 0; i < populationSize; i++) {
            nets[i] = tempNet[i];
            nets[i].SetNetFitness(0);
            nets[i].SetNetID(i);
            nets[i].ClearNodeValues();
        }

        ResetWorld();
        GeneratePopulation();
    }

    public List<int> GenerateListNumbers(int min, int max)
    {
        List<int> unusedIndicies = new List<int>();
        for (int i = min; i <= max; i++) {
            unusedIndicies.Add(i);
        }
        return unusedIndicies;
    }

    public int[,] GenerateRandomUniquePaires(List<int> indicies) {
        int[,] paires = new int[indicies.Count / 2, 2];
        int count = indicies.Count / 2;

        for (int i = 0; i < count; i++) {
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

    public void SortFitness() {
        float[] tempFitness = new float[2];
        bool swapped = true;
        int j = 0;
        while (swapped) {
            swapped = false;
            j++;
            for (int i = 0; i < populationSize - j; i++)
            {
                if (finishedResults[i, 1] > finishedResults[i + 1, 1])
                {
                    tempFitness[0] = finishedResults[i, 0];
                    tempFitness[1] = finishedResults[i, 1];

                    finishedResults[i, 0] = finishedResults[i + 1, 0];
                    finishedResults[i, 1] = finishedResults[i + 1, 1];

                    finishedResults[i + 1, 0] = tempFitness[0];
                    finishedResults[i + 1, 1] = tempFitness[1];
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

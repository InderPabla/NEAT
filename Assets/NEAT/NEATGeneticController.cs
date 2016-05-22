using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections;

public class NEATGeneticController : MonoBehaviour
{

    public GameObject testPrefab;

    //input through unity editor
    public string creatureName = "Creature";
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
    private float timeScale = 0;
    private DatabaseOperation operations;

    public GameObject netDrawer;

    public List<Transform> bodies;
    
    public bool worldActivation = false;

    public bool loadFromDatabase = false;

    private NEATConsultor consultor;

    void Start() {
        Application.runInBackground = true;
        if (ErrorCheck() == false) {
            timeScale = Time.timeScale;
            testCounter = 0;
            finished = new Semaphore(1, 1);
            nets = new NEATNet[populationSize];
            consultor = new NEATConsultor(numberOfInputPerceptrons,numberOfOutputPerceptrons);
            finishedResults = new float[populationSize, 2];
            operations = new DatabaseOperation();


            if (loadFromDatabase == true) {
                StartCoroutine(operations.GetNet(creatureName));
                StartCoroutine(CheckRetrival());
            }
            else {
                GenerateInitialNets();
                GeneratePopulation();
            }
        }
    }

    public IEnumerator CheckRetrival() {
        while(operations.done == false)
            yield return null;

        GenerateCopyNets(new NEATNet(operations.retrieveNet[operations.retrieveNet.Length-1]));
        GeneratePopulation();
    } 

    void FixedUpdate() {
        if (worldActivation) {
            bodies[0].transform.position += new Vector3(Time.deltaTime/2.5f, 0, 0);
            bodies[1].transform.position += new Vector3(-Time.deltaTime/2.5f, 0, 0);
            bodies[2].transform.position += new Vector3(0, Time.deltaTime/8f, 0);
            bodies[3].transform.position += new Vector3(0, -Time.deltaTime/8f, 0);
            //bodies[4].transform.position += new Vector3(Time.deltaTime*1.1f, 0, 0);
            //bodies[5].transform.position += new Vector3(-Time.deltaTime*1.1f, 0, 0);
        }

       if (timeScale <= 1f) {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

               if (hit.collider != null && hit.collider.name.Contains("B"))
               {
                    int id = hit.collider.transform.parent.GetComponent<Tester>().GetNet().GetNetID();
                    netDrawer.SendMessage("DrawNet", nets[id]);
                }

            }
        }
    }

    public void ResetWorld() {
        if (worldActivation) {
            bodies[0].transform.position = new Vector3(-17.5f, 0, 0);
            bodies[1].transform.position = new Vector3(17.5f, 0, 0);
            bodies[2].transform.position = new Vector3(0, -9.5f, 0);
            bodies[3].transform.position = new Vector3(0, 9.5f, 0);
            bodies[4].transform.position = new Vector3(-22, -3f, 0);
            bodies[5].transform.position = new Vector3(22, 3f, 0);
        }
        timeScale = Time.timeScale;
    }


    public void GenerateInitialNets() {
        for (int i = 0; i < populationSize; i++) {
            nets[i] = new NEATNet(consultor,i, 0, numberOfInputPerceptrons, numberOfOutputPerceptrons, testTime);
        }
    }

    public void GenerateCopyNets(NEATNet net) {
        for (int i = 0; i < populationSize; i++) {
            nets[i] = new NEATNet(net);
            nets[i].SetTestTime(testTime);
            nets[i].SetNetFitness(0);
            nets[i].SetNetID(i);
            nets[i].ClearNodeValues();
        }
    }

    public void GeneratePopulation() {
        List<int> numbers = GenerateListNumbers(0, populationSize-1);
        float height = 12f;
        float width = -25f;
        int counter = 1;
        for (int i = 0; i < populationSize; i++) {

            GameObject tester = (GameObject)Instantiate(testPrefab, new Vector3(width,height, 0), testPrefab.transform.rotation);
            tester.name = i+"";

            int randomIndex = UnityEngine.Random.Range(0,numbers.Count);
            tester.SendMessage(ACTIVATE, nets[numbers[randomIndex]]);
            numbers.RemoveAt(randomIndex);
            tester.GetComponent<Tester>().TestFinished += OnFinished; //suscribe OnFinished to event in Balancer


            /*if (width+2.85f > 16 && width > 0) {
                height -= 2;
                width = -16f;
            }
            else
                width += 2.85f;*/

            if (width % 25 == 0 && width>0) {
                width = -25f;
                height +=- 2f;
            }
            else
                width += 5f;
            
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

        if (generationNumber == 100)
        {
            StartCoroutine(operations.SaveNet(nets[bestNetIndex], creatureName));
        }
        
        netDrawer.SendMessage("DrawNet",nets[bestNetIndex]);

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
            nets[i].SetTestTime(testTime);
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

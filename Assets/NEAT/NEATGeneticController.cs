using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class NEATGeneticController : MonoBehaviour
{

    public GameObject testPrefab;

    //input through unity editor
    public string creatureName = "Creature";
    public int numberOfInputPerceptrons = 0;
    public int numberOfOutputPerceptrons = 0;
    public int populationSize = 0;
    public float testTime = 0;

    //private NEATNet[] nets; //array of neural networks 
    private List<List<NEATNet>> species = new List<List<NEATNet>>();

    //private float[,] finishedResults;
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
            //nets = new NEATNet[populationSize];
            consultor = new NEATConsultor(numberOfInputPerceptrons,numberOfOutputPerceptrons, 1f, 1f, 1f, 1);
            //finishedResults = new float[populationSize, 2];
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

        GenerateCopyNets(new NEATNet(operations.retrieveNet[operations.retrieveNet.Length-1],consultor));
        GeneratePopulation();
    } 

    void FixedUpdate() {
        if (worldActivation) {
            bodies[0].transform.position += new Vector3(Time.deltaTime/2f, 0, 0);
            bodies[1].transform.position += new Vector3(-Time.deltaTime/2f, 0, 0);
            bodies[2].transform.position += new Vector3(0, Time.deltaTime/4f, 0);
            bodies[3].transform.position += new Vector3(0, -Time.deltaTime/4f, 0);
        }

       if (timeScale <= 1f) {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

               if (hit.collider != null && hit.collider.name.Contains("B"))
               {
                    //int id = hit.collider.transform.parent.GetComponent<Tester>().GetNet().GetNetID();
                    //netDrawer.SendMessage("DrawNet", nets[id]);
                    int[] id = hit.collider.transform.parent.GetComponent<Tester>().GetNet().GetNetID();
                    netDrawer.SendMessage("DrawNet", species[id[0]][id[1]]);
                }

            }
        }
    }

    public void ResetWorld() {
        if (worldActivation) {
            bodies[0].transform.position = new Vector3(-26f, 0, 0);
            bodies[1].transform.position = new Vector3(26f, 0, 0);
            bodies[2].transform.position = new Vector3(0, -14.5f, 0);
            bodies[3].transform.position = new Vector3(0, 14.5f, 0);
        }
        timeScale = Time.timeScale;
    }


    public void GenerateInitialNets() {
        /*for (int i = 0; i < populationSize; i++) {
            nets[i] = new NEATNet(consultor, i, numberOfInputPerceptrons, numberOfOutputPerceptrons, testTime);
        }*/

        //List<NEATNet> initialSpecies = new List<NEATNet>();
        for (int i = 0; i < populationSize; i++) {
            NEATNet net = new NEATNet(consultor, new int[] {0,i}, numberOfInputPerceptrons, numberOfOutputPerceptrons, testTime);
            for (int j = 0; j <1; j++) {
               // net.Mutate();
            }

            if (species.Count == 0) {
                List<NEATNet> newSpecies = new List<NEATNet>();
                newSpecies.Add(net);
                species.Add(newSpecies);
            }
            else {
                int numberOfSpecies = species.Count;
                int location = -1;
                for (int j = 0; j < numberOfSpecies; j++)  {
                    int numberOfNets = species[j].Count;
                    int randomIndex = UnityEngine.Random.Range(0, numberOfNets);
                    if (NEATNet.SameSpecies(species[j][randomIndex], net)) {
                        location = j;
                        break;
                    }
                }

                if (location == -1) {
                    List<NEATNet> newSpecies = new List<NEATNet>();
                    newSpecies.Add(net);
                    species.Add(newSpecies);
                }
                else {
                    species[location].Add(net);
                }
            } 
        }

        /*int numberOfSpecies1 = species.Count;
        Debug.Log(numberOfSpecies1);
        for (int i = 0; i < numberOfSpecies1; i++) {
            int numberOfNets = species[i].Count;
            Debug.Log(i+" "+numberOfNets);
        }*/
    }

    public void GenerateCopyNets(NEATNet net) {
        /*for (int i = 0; i < populationSize; i++) {
            nets[i] = new NEATNet(net);
            nets[i].SetTestTime(testTime);
            nets[i].SetNetFitness(0);
            nets[i].SetNetID(i);
            nets[i].ClearNodeValues();
        }*/
    }

    public void GeneratePopulation() {
        /*List<int> numbers = GenerateListNumbers(0, populationSize-1);
        float height = 12f;
        float width = -25f;
        for (int i = 0; i < populationSize; i++) {

            GameObject tester = (GameObject)Instantiate(testPrefab, new Vector3(0,0, 0), testPrefab.transform.rotation);
            tester.name = i+"";

            int randomIndex = UnityEngine.Random.Range(0,numbers.Count);
            tester.SendMessage(ACTIVATE, nets[numbers[randomIndex]]);
            numbers.RemoveAt(randomIndex);
            tester.GetComponent<Tester>().TestFinished += OnFinished; //suscribe OnFinished to event in Balancer

            if (width % 25 == 0 && width>0) {
                width = -25f;
                height +=- 2f;
            }
            else
                width += 5f;
            
        }*/

        float height = 10f;
        float width = -20f;
        int numberOfSpecies = species.Count;
        for (int i = 0; i < numberOfSpecies; i++) {
            int numberOfNets = species[i].Count;
            for (int j = 0; j < numberOfNets; j++) {
                CreateIndividual(new Vector3(width, height, 0), species[i][j]);


                if (width % 20 == 0 && width > 0)
                {
                    width = -20f;
                    height += -2f;
                }
                else
                    width += 4f;
            }
        }


    }

    public void CreateIndividual(Vector3 position, NEATNet net) {
        GameObject tester = (GameObject)Instantiate(testPrefab, position, testPrefab.transform.rotation);
        tester.SendMessage(ACTIVATE, net);
        tester.GetComponent<Tester>().TestFinished += OnFinished;
    }

    public void OnFinished(object source, EventArgs e) {
        finished.WaitOne();

        /*int netID = (int)source;
        finishedResults[testCounter, 0] = netID;
        finishedResults[testCounter, 1] = nets[netID].GetNetFitness();*/

        testCounter++;

        if (testCounter == populationSize) {
            TestFinished();
        }

        finished.Release();
    }

    public void TestFinished() {
        /*testCounter = 0;
        generationNumber++;

        NEATNet[] tempNet = new NEATNet[populationSize];

        SortFitness();
        int bestNetIndex = (int)finishedResults[populationSize - 1, 0];
        Debug.Log("Generation Number: " + generationNumber + ", Best Fitness: " + finishedResults[populationSize - 1, 1]);
        Debug.Log("-----"+nets[bestNetIndex].GetNodeCount() +" "+ nets[bestNetIndex].GetGeneCount());

        if (generationNumber == 100) {
            StartCoroutine(operations.SaveNet(nets[bestNetIndex], creatureName));
        }
        
        netDrawer.SendMessage("DrawNet",nets[bestNetIndex]);wwwwwwwwwwwwwwwwww

        int index = 0;

        for (int i = populationSize / 2; i < populationSize; i+=2) {
            NEATNet net1 = nets[(int)finishedResults[i, 0]];
            NEATNet net2 = nets[(int)finishedResults[i+1, 0]];

            NEATNet net3 = NEATNet.Corssover(net1, net2);
            NEATNet net4 = new NEATNet(net3);

            net1.Mutate();
            net2.Mutate();
            net3.Mutate();
            net4.Mutate();

            tempNet[index] = net1;
            tempNet[index + 1] = net2;
            tempNet[index + 2] = net3;
            tempNet[index + 3] = net4;

            index += 4;
        }

        for (int i = 0; i < populationSize; i++) {
            nets[i] = tempNet[i];
            nets[i].SetTestTime(testTime);
            nets[i].SetNetFitness(0);
            nets[i].SetNetID(i);
            nets[i].ClearNodeValues();
        }

        ResetWorld();
        GeneratePopulation();*/



        testCounter = 0;
        generationNumber++;


        List<List<float>> adjustedFitnessSpecies = new List<List<float>>();
        List<int> newPopulationDistribution = new List<int>();
        List<float> populationFitness = new List<float>();

        int numberOfSpecies = species.Count;
        float totalFitness = 0;
        float highestFitness = -1000000;

        for (int i = 0; i < numberOfSpecies; i++) {

            float sharedTotal = 0;
            int numberOfNets = species[i].Count;
            List<float> adjustedFitnessNets = new List<float>();

            /*for (int j = 0; j < numberOfNets1; j++) {
                for (int k = 0; k < numberOfSpecies; k++) {
                    int numberOfNets2 = species[k].Count;
                    for (int l = 0; l < numberOfNets2; l++) {
                       if (i != k && j != l) {
                            sharedTotal += NEATNet.SameSpecies(species[i][j], species[k][l]) == true ? 1 : 0;
                       }
                   }
                }  
            }*/
            for(int j = 0; j < numberOfNets; j++) {
                for (int k = j+1; k < numberOfNets; k++) {
                    if (j != k) {
                        sharedTotal += NEATNet.SameSpecies(species[i][j], species[i][k]) == true ? 1 : 0;
                    }
                }
            }

            if (sharedTotal == 0)
                sharedTotal = 1;

            populationFitness.Add(0);
            for (int j = 0; j < numberOfNets; j++) {
                float fitness = species[i][j].GetNetFitness();

                if (fitness > highestFitness)
                    highestFitness = fitness;
                fitness /= sharedTotal;
                adjustedFitnessNets.Add(fitness);

                totalFitness += fitness;
                populationFitness[i] += fitness;
            }

            if (adjustedFitnessNets.Count > 0) {
                adjustedFitnessSpecies.Add(adjustedFitnessNets);
            }
        }

        Debug.Log("Generation Number: "+generationNumber+", Highest Fitness: "+highestFitness);

        int populationLeft = 0;
        for (int i = 0; i < numberOfSpecies; i++) {
            newPopulationDistribution.Add(0);
            newPopulationDistribution[i] = Mathf.RoundToInt((populationFitness[i] / totalFitness) * populationSize);
            populationLeft += newPopulationDistribution[i];
        }

        for (int i = 0; i < newPopulationDistribution.Count; i++) {
            if (newPopulationDistribution[i] == 0) {
                newPopulationDistribution.RemoveAt(i);
                species.RemoveAt(i);
            }
        }

        populationLeft = populationSize - populationLeft;
        //Debug.Log("Popleft: "+populationLeft);

        if (populationLeft < 0) {
            populationLeft = Mathf.Abs(populationLeft);

            for (int i = 0; i < populationLeft; i++) {
                int randomSpeciesIndex = UnityEngine.Random.Range(0, newPopulationDistribution.Count);
                newPopulationDistribution[randomSpeciesIndex]--;

                if (newPopulationDistribution[randomSpeciesIndex] == 0) {
                    newPopulationDistribution.RemoveAt(randomSpeciesIndex);
                    species.RemoveAt(randomSpeciesIndex);
                }
            }
        }
        else {
            for (int i = 0; i < populationLeft; i++) {
                int randomSpeciesIndex = UnityEngine.Random.Range(0, newPopulationDistribution.Count);
                newPopulationDistribution[randomSpeciesIndex]++;
            }
        }

        List<List<NEATNet>> tempSpecies = species;
        species = new List<List<NEATNet>>();

        
        for (int i =0;i<tempSpecies.Count;i++) {
            if (tempSpecies[i].Count > 1) {
                float[,] fitness = SortedFitnessIndicies(tempSpecies[i]);
                
                for (int j = 0; j < (fitness.GetLength(0)/2 ); j++) {
                    tempSpecies[i][(int)fitness[j, 0]] = null;

                    //Debug.Log(i + " " + (int)fitness[j, 0] + " " + tempSpecies[i].Count+" "+ fitness.GetLength(0)+" "+((fitness.GetLength(0) / 2)));
                    //Debug.Log(i+" "+ (int)fitness[j, 0]+ " "+tempSpecies[i][(int)fitness[j, 0]].GetNetFitness()+" "+tempSpecies[i].Count);
                    //Debug.Log(i + " " + (int)fitness[j, 0]);
                }
            }
        }

        for (int i = 0; i < tempSpecies.Count; i++) {
            for (int j = 0; j < tempSpecies[i].Count; j++) {
                if (tempSpecies[i][j] == null) {
                    tempSpecies[i].RemoveAt(j);
                    j = 0;
                }
            }
        }
        for (int i = 0; i < tempSpecies.Count; i++)
        {
            for (int j = 0; j < tempSpecies[i].Count; j++)
            {
                if (tempSpecies[i][j] == null)
                {
                    tempSpecies[i].RemoveAt(j);
                    j = 0;
                }
            }
        }

        for (int i = 0; i < newPopulationDistribution.Count; i++) {
            int distribution = newPopulationDistribution[i];

            for (int j = 0; j < distribution; j++) {
                int numberOfTempNets = tempSpecies[i].Count;
                NEATNet parent1, parent2, net = null;

                 if (tempSpecies[i].Count == 1) {
                     parent1 = tempSpecies[i][0];
                     net = new NEATNet(parent1);

                 }
                 else if (tempSpecies[i].Count > 1) {

                    float[,] fitness = SortedFitnessIndicies(tempSpecies[i]);
                    int index1 = (int)fitness[fitness.GetLength(0)-1,0]/*UnityEngine.Random.Range(0, numberOfTempNets)*/;
                    int index2 = index1;
                    while (index1 == index2) {

                        index2=  UnityEngine.Random.Range(0, numberOfTempNets);
                    }

                    parent1 = tempSpecies[i][index1];
                    parent2 = tempSpecies[i][index2];
                    net = NEATNet.Corssover(parent1, parent2);

                    /*parent1 = tempSpecies[i][(int)fitness[fitness.GetLength(0)-1,0]];
                    parent2 = tempSpecies[i][(int)fitness[fitness.GetLength(0)-2, 0]];

                    net = NEATNet.Corssover(parent1, parent2);
                    /*if (j < 2)
                    {
                        net = tempSpecies[i][(int)fitness[fitness.GetLength(0) - 1, 0]];
                    }
                    else {
                        parent1 = tempSpecies[i][(int)fitness[fitness.GetLength(0) - 1, 0]];
                        parent2 = tempSpecies[i][(int)fitness[fitness.GetLength(0) - 2, 0]];

                        net = NEATNet.Corssover(parent1, parent2);
                        net.Mutate();
                    }*/
                }


                net.Mutate();
                net.SetNetFitness(0f);
                net.SetTestTime(testTime);
                net.ClearNodeValues();

                if (species.Count == 0)
                {
                    List<NEATNet> newSpecies = new List<NEATNet>();
                    newSpecies.Add(net);
                    species.Add(newSpecies);
                }
                else
                {
                    int numberOfNewSpecies = species.Count;
                    int location = -1;
                    for (int k = 0; k < numberOfNewSpecies; k++)
                    {
                        int numberOfNets = species[k].Count;
                        int randomIndex = UnityEngine.Random.Range(0, numberOfNets);
                        if (NEATNet.SameSpecies(species[k][randomIndex], net))
                        {
                            location = k;
                            break;
                        }
                    }

                    if (location == -1)
                    {
                        List<NEATNet> newSpecies = new List<NEATNet>();
                        newSpecies.Add(net);
                        species.Add(newSpecies);
                    }
                    else
                    {
                        species[location].Add(net);
                    }
                }
            }
        }

        /*int numberOfSpecies1 = species.Count;
        Debug.Log(numberOfSpecies1);
        for (int i = 0; i < numberOfSpecies1; i++)
        {
            int numberOfNets = species[i].Count;
            Debug.Log(i + " " + numberOfNets);
        }
        */
        ResetWorld();
        GeneratePopulation();

        /*int counter = 0;
        for (int i = 0; i < species.Count; i++) {
            Debug.Log(i+" "+species[i].Count);
            counter += species[i].Count;
        }
        Debug.Log(counter);*/

        /*int countDistribution = 0;
        int countSpecies = 0;
        for (int i = 0; i < newPopulationDistribution.Count; i++) {
            Debug.Log(species[i].Count+" => "+newPopulationDistribution[i]);
            countDistribution += newPopulationDistribution[i];
            countSpecies += species[i].Count;
        }
        Debug.Log("Total: "+countDistribution+" "+countSpecies+" "+newPopulationDistribution.Count+" "+species.Count);*/




    }

    public float[,] SortedFitnessIndicies(List<NEATNet> organisums) {
        float[,] sorted = new float [organisums.Count,2];

        for (int i = 0; i < sorted.GetLength(0); i++) {
            sorted[i,0] = i;
            sorted[i,1] = organisums[i].GetNetFitness();
        }

        return SortFitness(sorted);
    }

    public List<int> GenerateListNumbers(int min, int max) {
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

    public float[,] SortFitness(float[,] sort) {
        float[] tempFitness = new float[2];
        bool swapped = true;
        int j = 0;
        while (swapped) {
            swapped = false;
            j++;
            for (int i = 0; i < sort.GetLength(0) - j; i++)
            {
                if (sort[i, 1] > sort[i + 1, 1])
                {
                    tempFitness[0] = sort[i, 0];
                    tempFitness[1] = sort[i, 1];

                    sort[i, 0] = sort[i + 1, 0];
                    sort[i, 1] = sort[i + 1, 1];

                    sort[i + 1, 0] = tempFitness[0];
                    sort[i + 1, 1] = tempFitness[1];
                    swapped = true;
                }
            }
        }

        return sort;
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

using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class NEATGeneticControllerV2 : MonoBehaviour
{

    public GameObject testPrefab;

    //input through unity editor
    public string creatureName = "Creature";
    public int numberOfInputPerceptrons = 0;
    public int numberOfOutputPerceptrons = 0;
    public int populationSize = 0;
    public float testTime = 0;

   
    private List<List<NEATNet>> species = new List<List<NEATNet>>();

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

    void Start()
    {
        Application.runInBackground = true;
        if (ErrorCheck() == false)
        {
            timeScale = Time.timeScale;
            testCounter = 0;
            finished = new Semaphore(1, 1);
            consultor = new NEATConsultor(numberOfInputPerceptrons, numberOfOutputPerceptrons, 1f, 3f, 2f, 3f);
            operations = new DatabaseOperation();


            if (loadFromDatabase == true)
            {
                StartCoroutine(operations.GetNet(creatureName));
                StartCoroutine(CheckRetrival());
            }
            else
            {
                GenerateInitialNets();
                GeneratePopulation();
            }
        }
    }

    public IEnumerator CheckRetrival()
    {
        while (operations.done == false)
            yield return null;

        GenerateCopyNets(new NEATNet(operations.retrieveNet[operations.retrieveNet.Length - 1], consultor));
        GeneratePopulation();
    }

    void FixedUpdate()
    {
        if (worldActivation)
        {
            bodies[0].transform.position += new Vector3(Time.deltaTime / 2f, 0, 0);
            bodies[1].transform.position += new Vector3(-Time.deltaTime / 2f, 0, 0);
            bodies[2].transform.position += new Vector3(0, Time.deltaTime / 4f, 0);
            bodies[3].transform.position += new Vector3(0, -Time.deltaTime / 4f, 0);
        }

        if (timeScale <= 1f)
        {
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

    public void ResetWorld()
    {
        if (worldActivation)
        {
            bodies[0].transform.position = new Vector3(-26f, 0, 0);
            bodies[1].transform.position = new Vector3(26f, 0, 0);
            bodies[2].transform.position = new Vector3(0, -14.5f, 0);
            bodies[3].transform.position = new Vector3(0, 14.5f, 0);
        }
        timeScale = Time.timeScale;
    }


    public void GenerateInitialNets() {
        for (int i = 0; i < populationSize; i++) {
            NEATNet net = new NEATNet(consultor, new int[] { 0, i }, numberOfInputPerceptrons, numberOfOutputPerceptrons, testTime);
            for (int j = 0; j < 1; j++) {
                //net.Mutate(); net.Mutate(); net.Mutate(); net.Mutate();
                net.Mutate(); 
            }

            if (species.Count == 0) {
                List<NEATNet> newSpecies = new List<NEATNet>();
                newSpecies.Add(net);
                species.Add(newSpecies);
            }
            else {
                int numberOfSpecies = species.Count;
                int location = -1;
                for (int j = 0; j < numberOfSpecies; j++) {
                    int numberOfNets = species[j].Count;
                    int randomIndex = UnityEngine.Random.Range(0, numberOfNets);
                    if (NEATNet.SameSpeciesV2(species[j][randomIndex], net)) {
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


    }

    public void GenerateCopyNets(NEATNet net) {

    }

    public void GeneratePopulation() {
        float height = 10f;
        float width = -20f;
        int numberOfSpecies = species.Count;
        for (int i = 0; i < numberOfSpecies; i++)
        {
            int numberOfNets = species[i].Count;
            for (int j = 0; j < numberOfNets; j++)
            {
                CreateIndividual(new Vector3(width, height, 0), species[i][j]);

                if (width % 20 == 0 && width > 0)
                {
                    width = -20f;
                    height += -1f;
                }
                else
                    width += 4f;
            }
        }
    }

    public void CreateIndividual(Vector3 position, NEATNet net)
    {
        GameObject tester = (GameObject)Instantiate(testPrefab, position, testPrefab.transform.rotation);
        tester.SendMessage(ACTIVATE, net);
        tester.GetComponent<Tester>().TestFinished += OnFinished;
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

    public void TestFinished() {
        testCounter = 0;
        generationNumber++;

        float highestFitness = 0f;
        List<List<NEATNet>> bestSpecies = new List<List<NEATNet>>();

        float[,] distribution = new float[species.Count,2];
        float totalSharedFitness = 0f;
        float totalOrganisums = 0f;

        int[] id = new int[2];
        for (int i = 0; i < species.Count; i++) {
            float sharedAmount = 0f;
            distribution[i, 0] = i;
            for (int j = 0; j < species[i].Count; j++) {
                distribution[i,1] += species[i][j].GetNetFitness();
                if (species[i][j].GetNetFitness() > highestFitness) {
                    highestFitness = species[i][j].GetNetFitness();
                    id[0] = i; id[1] = j;
                }
                for (int k = j+1; k < species[i].Count; k++) {
                    sharedAmount += NEATNet.SameSpeciesV2(species[i][j],species[i][k]) == true?1:0;
                }
            }
            if (sharedAmount == 0)
                sharedAmount = 1f;
            distribution[i,1] = distribution[i, 1] / sharedAmount;
            totalSharedFitness += distribution[i,1];

            float[,] sortedFitness = SortedFitnessIndicies(species[i]);
            List<NEATNet> bestOrganisums = new List<NEATNet>();

            for (int j = sortedFitness.GetLength(0) / 2; j < sortedFitness.GetLength(0); j++) {
                bestOrganisums.Add(species[i][(int)sortedFitness[j, 0]]);
            }
            bestSpecies.Add(bestOrganisums);
        }

        Debug.Log("Generation Number: " + generationNumber + ", Highest Fitness: " + highestFitness);
        netDrawer.SendMessage("DrawNet",species[id[0]][id[1]]);

        distribution = SortFitness(distribution);
        for (int i = 0; i < distribution.GetLength(0); i++) {
            distribution[i, 1] = (int)((distribution[i, 1] / totalSharedFitness) * populationSize);
            totalOrganisums += distribution[i, 1];
            //Debug.Log(distribution[i, 0] + " " + i + " " + species[i].Count + " " + distribution[i, 1]);
        }

        for (int i = 0; i < (populationSize - totalOrganisums); i++) {
            int highIndex = distribution.GetLength(0)/2;
            int randomInsertIndex = UnityEngine.Random.Range(highIndex, distribution.GetLength(0));
            distribution[randomInsertIndex, 1]++;
        }

        species = new List<List<NEATNet>>();

        for (int i = 0; i < distribution.GetLength(0); i++) {
            if (distribution[i, 1] > 0) {
                for (int j = 0; j < distribution[i, 1]; j++) {
                    List<NEATNet> bestOrganisums = bestSpecies[(int)distribution[i, 0]];
                    NEATNet net = new NEATNet(bestOrganisums[UnityEngine.Random.Range(0, bestOrganisums.Count)]);
                    if(j< (float)distribution[i, 1]*0.9f)
                        net.Mutate();
                    net.SetNetFitness(0f);
                    net.SetTestTime(testTime);
                    net.ClearNodeValues();

                            if (species.Count == 0) {
                                List<NEATNet> newSpecies = new List<NEATNet>();
                                newSpecies.Add(net);
                                species.Add(newSpecies);
                            }
                            else {
                                int numberOfSpecies = species.Count;
                                int location = -1;
                                for (int k = 0; k < numberOfSpecies; k++) {
                                    int numberOfNets = species[k].Count;
                                    int randomIndex = UnityEngine.Random.Range(0, numberOfNets);
                                    if (NEATNet.SameSpeciesV2(species[k][randomIndex], net)) {
                                        location = k;
                                        break;
                                    }
                                }

                                if (location == -1) {
                                    List<NEATNet> newSpecies = new List<NEATNet>();
                                    newSpecies.Add(net);
                                    net.SetNetID(new int[] {species.Count, 0 });
                                    species.Add(newSpecies);
                                    
                                }
                                else {
                                    net.SetNetID(new int[] {location, species[location].Count});
                                    species[location].Add(net);
                                }      
                            }
                }
            }
        }
        
        GeneratePopulation();
    }

    public float[,] SortedFitnessIndicies(List<NEATNet> organisums) {
        float[,] sorted = new float[organisums.Count, 2];
        for (int i = 0; i < sorted.GetLength(0); i++) {
            sorted[i, 0] = i;
            sorted[i, 1] = organisums[i].GetNetFitness();
        }
        return SortFitness(sorted);
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

    public float[,] SortFitness(float[,] sort)
    {
        float[] tempFitness = new float[2];
        bool swapped = true;
        int j = 0;
        while (swapped)
        {
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

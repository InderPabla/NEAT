using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using ProgressBar;

public class NEATGeneticControllerV2 : MonoBehaviour
{
    //input through unity editor
    public GameObject testPrefab;
    public GameObject progressBar;
    public GameObject netDrawer;
    public GameObject lineGraph;

    public List<Transform> bodies;

    public bool worldActivation = false;
    public bool loadFromDatabase = false;

    public string creatureName = "Creature";

    public int numberOfInputPerceptrons = 0;
    public int numberOfOutputPerceptrons = 0;
    public int populationSize = 0;
    public float testTime = 0;


    private List<List<NEATNet>> species = new List<List<NEATNet>>();

    private Semaphore finished; //mutex lock for when test if finished and updating test counter

    private const string ACTIVATE = "Activate";

    int numberOfGenerationsToRun = 0;
    private int generationNumber = 0;
    private int testCounter; //counter for population testing
    private float timeScale = 0;
   
    private NEATConsultor consultor;
    private DatabaseOperation operations;

    private float[,] colors = new float[100,3];
    private NEATNet bestNet;

    private bool viewMode = false;
    private bool computing = false;
    
    float currentIncrement = 0;

    public GameObject foodMakerPrefab;
    GameObject foodMaker;

    private void Start()
    {

        Application.runInBackground = true;
        SetCameraLocation();
        
        if (ErrorCheck() == false)
        {
            timeScale = Time.timeScale;
            testCounter = 0;
            finished = new Semaphore(1, 1);
            //{0.5f, 1f, 1f, 4f}, {1f, 3f, 2f, 3f}, {0.1f, 2f, 2f, 4f}  works for seeker (non mover) worst to best
            //{1f, 2f, 2f, 2f} works for collision avoidance

            consultor = new NEATConsultor(numberOfInputPerceptrons, numberOfOutputPerceptrons, 0.25f, 2f, 2f, 2f);
            operations = new DatabaseOperation();

            colors[0, 0] = UnityEngine.Random.Range(0f, 1f);
            colors[0, 1] = UnityEngine.Random.Range(0f, 1f);
            colors[0, 2] = UnityEngine.Random.Range(0f, 1f);

            for (int i = 1; i < colors.GetLength(0); i++) {
                bool found = false;

                while (!found)
                {
                    found = true;
                    colors[i, 0] = UnityEngine.Random.Range(0f, 1f);
                    colors[i, 1] = UnityEngine.Random.Range(0f, 1f);
                    colors[i, 2] = UnityEngine.Random.Range(0f, 1f);
                    for (int j = 0; j < i; j++)
                    {
                        if (!(Mathf.Abs(((colors[i, 0] + colors[i, 1] + colors[i, 2]) - (colors[j, 0] + colors[j, 1] + colors[j, 2]))) >= 0.005f))
                        {
                            found = false;
                        }
                    }
                }
            }

            lineGraph.GetComponent<LineGraphDrawer>().CalibrateGraph(200f,50f,0.5f, 0.04f,0.15f,0.15f);

            //OneGeneration();

            /*if (loadFromDatabase == true)
            {
                StartCoroutine(operations.GetNet(creatureName));
                StartCoroutine(CheckRetrival());
            }
            else
            {
                GenerateInitialNets();
                GeneratePopulation();
            }*/
        }
    }

    public void ActionGeneration(int gens) {
        if (numberOfGenerationsToRun == 0 && species.Count>0) {
            progressBar.GetComponent<ProgressRadialBehaviour>().Value = 0;
            computing = true;
            currentIncrement = 100f / gens;
            numberOfGenerationsToRun = gens;
            GeneratePopulation();
        }    
    }

    public void ActionViewCurrent() {
        if (numberOfGenerationsToRun == 0) {
            viewMode = true;
            computing = true;
            SetCameraLocation();
            GeneratePopulation();
        }
    }

    public void ActionBest() {
        if (numberOfGenerationsToRun == 0) {
            species = new List<List<NEATNet>>();
            List<NEATNet> newSpecies = new List<NEATNet>();
            for (int i = 0; i < populationSize; i++) {
                newSpecies.Add(new NEATNet(bestNet));
            }
            species.Add(newSpecies);
        }
    }

    public void ActionCreateNew() {
        GenerateInitialNets();
    }

    public void ActionSetTimeScale(float timeScale) {
        Time.timeScale = timeScale;
    }

    public void ActionSaveCurrent()
    {
        StartCoroutine(operations.SaveNet(bestNet,creatureName));
    }

    private void SetCameraLocation() {
        if (viewMode == true) {
            Camera.main.transform.position = new Vector3(0, 0, -100);
        }
        else {
            Camera.main.transform.position = new Vector3(0, transform.position.y, -100);
        }
    } 


    private IEnumerator CheckRetrival() {
        while (operations.done == false)
            yield return null;

        GenerateCopyNets(new NEATNet(operations.retrieveNet[operations.retrieveNet.Length - 1], consultor));
        GeneratePopulation();
    }

    private void FixedUpdate() {
        

        if (timeScale <= 1f) {
            if (Input.GetMouseButtonDown(0)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
                if (hit.collider != null && hit.collider.name.Contains("B")) {
                    int[] id = hit.collider.transform.parent.GetComponent<Tester>().GetNet().GetNetID();
                    netDrawer.SendMessage("DrawNet", species[id[0]][id[1]]);
                }
            }
        }

        if (computing == true) {
            if (worldActivation == true) {
                /*bodies[0].transform.localPosition += new Vector3(-Time.deltaTime / 1f, 0, 0);
                bodies[1].transform.localPosition += new Vector3(Time.deltaTime / 1f, 0, 0);
                bodies[2].transform.localPosition += new Vector3(0, Time.deltaTime / 4f, 0);
                bodies[3].transform.localPosition += new Vector3(0, -Time.deltaTime / 4f, 0);*/

            
            }
        }

        
    }


    private void DeleteWorld() {
        if (worldActivation)
        {
            Destroy(foodMaker);
        }
    }
    private void ResetWorld() {
        if (worldActivation) {
            /*bodies[0].transform.localPosition = new Vector3(65f, 0, 0);
            bodies[1].transform.localPosition = new Vector3(-65f, 0, 0);
            bodies[2].transform.localPosition = new Vector3(0, -50f, 0);
            bodies[3].transform.localPosition = new Vector3(0, 50f, 0);*/
            foodMaker = Instantiate(foodMakerPrefab);
        }
        
    }


    private void GenerateInitialNets() {
        generationNumber = 0;
        testCounter = 0;
        species = new List<List<NEATNet>>();

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

    private void GenerateCopyNets(NEATNet net) {

    }

    private void GeneratePopulation() {
        ResetWorld();

        List<int[]> allID = new List<int[]>();
        float height = 25f;
        float width = -25f;
        int numberOfSpecies = species.Count;

        for (int i = 0; i < numberOfSpecies; i++) {
            int numberOfNets = species[i].Count;
            for (int j = 0; j < numberOfNets; j++) {
                allID.Add(new int[] {i,j});
            }
        }

        for (int i = 0; i < numberOfSpecies; i++) {
            int numberOfNets = species[i].Count;

            for (int j = 0; j < numberOfNets; j++) {

                int randomIndex = UnityEngine.Random.Range(0, allID.Count);
                int[] randomId = allID[randomIndex];
                allID.RemoveAt(randomIndex);
                Color color = new Color(colors[randomId[0], 0], colors[randomId[0], 1], colors[randomId[0], 2]);

                CreateIndividual(new Vector3(width, height, 0), species[randomId[0]][randomId[1]], color, randomId);

                //CreateIndividual(new Vector3(width, height, 0), species[i][j],color, new int[] { i,j});

                if (width % 25 == 0 && width > 0) {
                    width = -25f;
                    height += -5f;
                }
                else
                    width += 5f;
            }
        }
    }

    private void CreateIndividual(Vector3 position, NEATNet net, Color color, int[] id) {
        GameObject tester = (GameObject)Instantiate(testPrefab, position, testPrefab.transform.rotation);
        tester.name = id[0] + "_" + id[1];
        tester.SendMessage(ACTIVATE, net);
        tester.transform.GetChild(0).GetComponent<Renderer>().material.color = color;
        tester.GetComponent<Tester>().TestFinished += OnFinished;
    }

    private void OnFinished(object source, EventArgs e) {
        finished.WaitOne();

        testCounter++;
        //Debug.Log(testCounter);
        if (testCounter == populationSize) {
            testCounter++;
            DeleteWorld();
            Invoke("TestFinished",1f);
            //TestFinished();
        }

        finished.Release();
    }

    private void TestFinished() {
        testCounter = 0;
        if (numberOfGenerationsToRun > 0) { 
            generationNumber++;

            float highestFitness = 0f;
            List<List<NEATNet>> bestSpecies = new List<List<NEATNet>>();

            float[,] distribution = new float[species.Count,2];
            float totalSharedFitness = 0f;
            float totalOrganisums = 0f;
            int[] bestIndex = new int[2];

            for (int i = 0; i < species.Count; i++) {
                float sharedAmount = 0f;
                distribution[i, 0] = i;
                for (int j = 0; j < species[i].Count; j++) {
                    distribution[i,1] += species[i][j].GetNetFitness();
                    if (species[i][j].GetNetFitness() > highestFitness) {
                        highestFitness = species[i][j].GetNetFitness();
                        bestIndex[0] = i; bestIndex[1] = j;
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

            distribution = SortFitness(distribution);

            bestNet = new NEATNet(species[bestIndex[0]][bestIndex[1]]);
            lineGraph.GetComponent<LineGraphDrawer>().PlotData(highestFitness);

            Debug.Log("Generation Number: " + generationNumber + ", Highest Fitness: " + highestFitness+", Delta: "+consultor.GetDeltaThreshold());
            netDrawer.SendMessage("DrawNet",bestNet);
            
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
                        NEATNet net = null;

                        if (j > (float)distribution[i, 1] * 0.1f) {
                            NEATNet organisum1 = bestOrganisums[UnityEngine.Random.Range(0, bestOrganisums.Count)];

                            float random = UnityEngine.Random.Range(1f,100f);
                            float powerNeeded = Mathf.Log(bestOrganisums.Count-1,100);
                            float logIndex = Mathf.Abs(((bestOrganisums.Count - 1) -Mathf.Pow(random, powerNeeded)));
                       
                            NEATNet organisum2 = bestOrganisums[(int)logIndex];
                       
                            net = NEATNet.Corssover(bestOrganisums[UnityEngine.Random.Range(0, bestOrganisums.Count)], bestOrganisums[UnityEngine.Random.Range(0, bestOrganisums.Count)]);
                            //net = NEATNet.Corssover(organisum1, organisum2);
                            net.Mutate();
                        }
                        else {
                            net = new NEATNet(bestOrganisums[UnityEngine.Random.Range(0, bestOrganisums.Count)]);
                        }

                        /*float random = UnityEngine.Random.Range(1f, 100f);
                        float powerNeeded = Mathf.Log(bestOrganisums.Count - 1, 100);
                        float logIndex = Mathf.Abs(((bestOrganisums.Count - 1) - Mathf.Pow(random, powerNeeded)));
                        net = bestOrganisums[(int)logIndex];
                        //net = new NEATNet(bestOrganisums[UnityEngine.Random.Range(0, bestOrganisums.Count)]);
                        if (j> (float)distribution[i, 1]*0.1f)
                            net.Mutate();*/

                        net.SetNetFitness(0f);
                        net.SetTimeLived(0f);
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
        } //number of generation > 0
        /*float deltaThresh = consultor.GetDeltaThreshold();

        if (species.Count < 10)
            consultor.SetDeltaThreshold(deltaThresh-(deltaThresh*0.5f));
        else
            consultor.SetDeltaThreshold(deltaThresh + (deltaThresh * 0.5f));*/

        if (numberOfGenerationsToRun > 0) {
            numberOfGenerationsToRun--;
            progressBar.GetComponent<ProgressRadialBehaviour>().IncrementValue(currentIncrement);
            GeneratePopulation();
        }
        else {
            viewMode = false;
            computing = false;
            SetCameraLocation();
        }
        //ResetWorld();
    }

    private float[,] SortedFitnessIndicies(List<NEATNet> organisums) {
        float[,] sorted = new float[organisums.Count, 2];
        for (int i = 0; i < sorted.GetLength(0); i++) {
            sorted[i, 0] = i;
            sorted[i, 1] = organisums[i].GetNetFitness();
        }
        return SortFitness(sorted);
    }

    private float[,] SortFitness(float[,] sort) {
        float[] tempFitness = new float[2];
        bool swapped = true;
        int j = 0;
        while (swapped) {
            swapped = false;
            j++;
            for (int i = 0; i < sort.GetLength(0) - j; i++) {
                if (sort[i, 1] > sort[i + 1, 1]) {
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

    private bool ErrorCheck() {
        bool error = false;

        if (numberOfInputPerceptrons <= 0) {
            Debug.LogError("Need one or more input perceptrons.");
            error = true;
        }

        if (numberOfOutputPerceptrons <= 0) {
            Debug.LogError("Need one or more output perceptrons.");
            error = true;
        }

        if (populationSize <= 0) {
            Debug.LogError("Population size must be greater than 0.");
            error = true;
        }

        if (populationSize % 2 != 0 || populationSize % 4 != 0) {
            Debug.LogError("Population size must be divisible by 2 and 4.");
            error = true;
        }

        if (testTime <= 0) {
            Debug.LogError("Time to test must be greater than 0 seconds.");
            error = true;
        }

        if (error) {
            Debug.LogError("One or more issues found. Exiting.");
            return true;
        }
        else {
            return false;
        }
    }
}

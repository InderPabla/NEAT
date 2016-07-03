using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections;
using ProgressBar;

/// <summary>
/// This is a general purpose genetic controller, which handles generational computation if a given tester, specification calculation and visual display and mechanics of the panel system.
/// </summary>
public class NEATGeneticControllerV2 : MonoBehaviour {

    //All public variables are inputs are from the editor
    public GameObject testPrefab; //Prebaf of the object to test   
    public GameObject progressBar; //Progress bar to showcase amount finished
    public GameObject netDrawer; //Draws neural network 
    public GameObject lineGraph; //Draws graph information and displays text based information
    public GameObject arrow; //Shows user where they are pointing (only used when for recording video). 
    public GameObject enviromentPrefab; //Prefab of the enviroment in which the tester is tested

    public int numberOfInputPerceptrons = 0; //Number of input perceptrons of neural network (including bias)
    public int numberOfOutputPerceptrons = 0; //Number of output perceptrons 
    public int populationSize = 0; //Population traning size

    public float testTime = 10f; //Time to test each generation
    public float elite = 0.1f; //Elite from each species
    public float deltaThreshold = 1f; //2 different organism with delta less than this threshold are considered to be in the same species
    public float disjointCoefficient = 1f; //how much affect disjoint genes have on delta calculation (disjoint means genes that go not line up) 
    public float excessCoefficient = 1f; //how much affect excess genes have on delta calculation (like excess, but outside the other species innovation number) 
    public float averageWeightDifferenceCoefficient = 1f; //how much affect the absolute average weight differences have on delta calculation

    public bool worldActivation = false; //Creating enviroment on each generation

    public string networkName = "Creature"; //Name of the network (used for saving to and retrieving from database

    private GameObject enviroment; //Instantiated enviroment prefab
    
    private Semaphore finished; //mutex lock for when test if finished and updating test counter

    private NEATConsultor consultor; //Handles consultor genome sequence

    private NEATNet bestNet; //Best network from current computed generation

    private DatabaseOperation operations; //Handles databse operation of saving and retrieving neural network

    private List<List<NEATNet>> species = new List<List<NEATNet>>(); //List of list of species (There can be multiple species in a population)
   
    private const string ACTIVATE = "Activate"; //Activate fucntion name in Tester class

    private int numberOfGenerationsToRun = 0; //Number of generation to run, chosen from the panel
    private int generationNumber = 0; //Current generation number
    private int testCounter; //Counter for population testing, goes from 0 to population size

    private float timeScale = 0; //Current time scale
    private float currentIncrement = 0; //Current increment rate for progress bar
    private float[,] colors = new float[100, 3]; //Species color indexing (up to 100 species)

    private bool viewMode = false; //If user is in view mode
    private bool computing = false; //If a generation is currently computing
    private bool load = false; //load from database or create random neural network

    /// <summary>
    /// Setting up initial systems
    /// </summary>
    private void Start() {
        Application.runInBackground = true; //Run application in background

        SetCameraLocation(); //Sets camera location to panel 
  
        if (ErrorCheck() == false) { //Make sure all parameters given are valid

            Time.timeScale = 1f; //Time scale set to 1x
            timeScale = Time.timeScale; 

            testCounter = 0; 
            finished = new Semaphore(1, 1); //initialize a binary semaphore and set it to unlocked

            //{0.5f, 1f, 1f, 4f}, {1f, 3f, 2f, 3f}, {0.1f, 2f, 2f, 4f}  works for seeker (non mover) worst to best
            //{1f, 2f, 2f, 2f} works for collision avoidance

            operations = new DatabaseOperation(); //initialize database oepration 

            GenerateSpeciesColor(); //Create random color for each species

            lineGraph.GetComponent<LineGraphDrawer>().CalibrateGraph(200f,50f,0.5f, 0.04f,0.15f,0.15f); //Calibrate line graph
        }
    }

    /// <summary>
    /// Generating random colors to identify each species 
    /// </summary>
    private void GenerateSpeciesColor() {
        //Set color for the first species
        colors[0, 0] = UnityEngine.Random.Range(0f, 1f);
        colors[0, 1] = UnityEngine.Random.Range(0f, 1f);
        colors[0, 2] = UnityEngine.Random.Range(0f, 1f);

        for (int i = 1; i < colors.GetLength(0); i++)  { // run through all other colors in the array
            bool found = false; //found a new color within threshold

            while (!found) { //loop till found is true
                found = true; // suppose found

                //create random color
                colors[i, 0] = UnityEngine.Random.Range(0f, 1f);
                colors[i, 1] = UnityEngine.Random.Range(0f, 1f);
                colors[i, 2] = UnityEngine.Random.Range(0f, 1f);
 
                for (int j = 0; j < i; j++) { //run through all other found colors to make sure this new found color is atleast a certain threshold away  
                    if (!(Mathf.Abs(((colors[i, 0] + colors[i, 1] + colors[i, 2]) - (colors[j, 0] + colors[j, 1] + colors[j, 2]))) >= 0.005f)) { //if new color is not a certain threshold away then not found
                        found = false;
                    }
                }
            }
        }

    }

    /// <summary>
    /// Start computation for number of generations given
    /// </summary>
    /// <param name="gens">Number of generation to compute</param>
    public void ActionGeneration(int gens) {    
        if (numberOfGenerationsToRun == 0 && species.Count > 0) { //If not computing and species exist then start
            progressBar.GetComponent<ProgressRadialBehaviour>().Value = 0; //progress bar set to 0%
            computing = true; //computing
            currentIncrement = 100f / gens; //increment of progress bar per generation
            numberOfGenerationsToRun = gens; //number of generation to run
            GeneratePopulation(); //Generate population with neural networks
            lineGraph.GetComponent<LineGraphDrawer>().DisplayActionInformation("Action: Running " + gens + " generations"); //information for the user on action performed
        }
        else { //Currently simulation is running
            lineGraph.GetComponent<LineGraphDrawer>().DisplayActionInformation("Action: Simulation currently running");  //information for the user on action performed
        }
          
    }

    /// <summary>
    /// View current neural networks 
    /// </summary>
    public void ActionViewCurrent() {
        if (numberOfGenerationsToRun == 0) { //if not computing
            viewMode = true; //view mode
            computing = true; //computing
            SetCameraLocation(); //change camera location to view 
            GeneratePopulation(); //Generate population with neural networks
        }
    }

    /// <summary>
    /// Take the best creature from previous generation and use it to populate the next generation
    /// </summary>
    public void ActionBest() {
        if (numberOfGenerationsToRun == 0) { //if not computing
            species = new List<List<NEATNet>>(); //create an empty population list
            List<NEATNet> newSpecies = new List<NEATNet>(); //create new species

            for (int i = 0; i < populationSize; i++) { //populate new species
                newSpecies.Add(new NEATNet(bestNet));
            }
            species.Add(newSpecies); //add new species to the species list
        }
    }

    /// <summary>
    /// Generating neural networks and consultor to test
    /// </summary>
    public void ActionCreateNew() {
        if (load == false) { //if loading from database is fasle
            consultor = new NEATConsultor(numberOfInputPerceptrons, numberOfOutputPerceptrons, deltaThreshold, disjointCoefficient, excessCoefficient, averageWeightDifferenceCoefficient);  //create a consultor with perceptron and coefficients
            GenerateInitialNets(); //generate standard NEAT nets
            lineGraph.GetComponent<LineGraphDrawer>().DisplayActionInformation("Action: New neural networks created"); //information for the user on action performed
        }
        else { //loading from database is true
            consultor = new NEATConsultor(operations.retrieveNet[operations.retrieveNet.Length-1], deltaThreshold, disjointCoefficient, excessCoefficient, averageWeightDifferenceCoefficient); //create a consultor with NEAT packet retrieved from database and coefficients
            NEATNet net = new NEATNet(operations.retrieveNet[operations.retrieveNet.Length-1], consultor); //create a net from NEAT packet retrieved from database and consultor 

            species = new List<List<NEATNet>>(); //create an empty population list
            List<NEATNet> newSpecies = new List<NEATNet>(); //create new species
             
            for (int i = 0; i < populationSize; i++) { //populate new species
                NEATNet netCopy = new NEATNet(net); //create a deep copy with net retrieved from database
                //reset copied stats
                netCopy.SetNetFitness(0f);
                netCopy.SetTimeLived(0f);
                netCopy.SetTestTime(testTime);
                netCopy.ClearNodeValues();
                newSpecies.Add(netCopy); //add copy to new species 
            }
            species.Add(newSpecies); //add new species to species list
            lineGraph.GetComponent<LineGraphDrawer>().DisplayActionInformation("Action: Neural networks copied from sample"); //information for the user on action performed
        }
    }

    /// <summary>
    /// Change time scale to make simulation compute faster
    /// </summary>
    /// <param name="timeScale">The new time scale to change to</param>
    public void ActionSetTimeScale(float timeScale) {
        this.timeScale = timeScale; //set time scale to parameter (when checking time scale this local copy will be used rather than the global copy)
        Time.timeScale = timeScale; //set time scale to parameter
        lineGraph.GetComponent<LineGraphDrawer>().DisplayActionInformation("Action: Time changed to "+ timeScale); //information for the user on action performed
    }

    /// <summary>
    /// Save best neural network to database
    /// </summary>
    public void ActionSaveCurrent() {
        StartCoroutine(operations.SaveNet(bestNet, networkName)); //start coroutine to save network
    }

    /// <summary>
    /// Retrieve all neural networks from database given a network name
    /// </summary>
    public void ActionDatabaseLoad()  {
        StartCoroutine(operations.GetNet(networkName)); //start coroutine to retrieve networks
        load = true; //loading from database is true
    }

    /// <summary>
    /// Change camera location based on view mode
    /// </summary>
    private void SetCameraLocation() {
        if (viewMode == true) { //view mode is true
            Camera.main.transform.position = new Vector3(0, 0, -100); //go to view location
        }
        else { //view mode is false
            Camera.main.transform.position = new Vector3(0, transform.position.y, -100); //go to panel location
        }
    } 

    /// <summary>
    /// Updating once per frame to display arrow
    /// </summary>
    private void Update() {
        if (timeScale <= 2) { //if time scale is less than or equal to 2x
            //move arrow to mouse location
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = -1f; 
            arrow.transform.position = worldPos;
        }
    }

    /// <summary>
    /// Destroy the enviroment on each generational simulation finish
    /// </summary>
    private void DeleteWorld() {
        if (worldActivation == true) { //if world activation is allowed
            Destroy(enviroment); //destroy enviroment
        }
    }

    /// <summary>
    /// Create an enviroment on each generational simulation start
    /// </summary>
    private void ResetWorld() {
        if (worldActivation == true) { //if world activation is allowed
            enviroment = (GameObject)Instantiate(enviromentPrefab); //instantiate enviroment
        }
    }

    /// <summary>
    /// Generate initial neural network, where all inputs perceptrons are connected to all output perceptrons. 
    /// Each of these nerual networks is mutated one time for slight change.
    /// And then they are paired up into species.
    /// </summary>
    private void GenerateInitialNets() {
        generationNumber = 0; //0 since simulaiton has no been started yet
        testCounter = 0; //none have been tested
        species = new List<List<NEATNet>>(); //create an empty population list

        for (int i = 0; i < populationSize; i++) { //run through population size
            NEATNet net = new NEATNet(consultor, new int[] { 0, i }, numberOfInputPerceptrons, numberOfOutputPerceptrons, testTime); //create net with consultor, perceptron information and test time
            net.Mutate(); //mutate once for diversity

            if (species.Count == 0) { //if nothing exists yet
                List<NEATNet> newSpecies = new List<NEATNet>(); //create a new species
                newSpecies.Add(net); //add net to this new species
                species.Add(newSpecies); //add new species to species list
            }
            else { //if at least one species exists 
                int numberOfSpecies = species.Count; //save species count
                int location = -1; //-1 means no species match found,  0 >= will mean a species match has been found at a given index

                for (int j = 0; j < numberOfSpecies; j++) { //run through species count
                    int numberOfNets = species[j].Count; //number of organisum in species at index j
                    int randomIndex = UnityEngine.Random.Range(0, numberOfNets); //pick a random network within this species

                    if (NEATNet.SameSpeciesV2(species[j][randomIndex], net)) { //check if new network and random network belong to the same species
                        location = j; //new species can be added to index j
                        break; //break out of loop
                    }
                }

                if (location == -1) { //if no species found
                    //create new species and add to that
                    List<NEATNet> newSpecies = new List<NEATNet>();
                    newSpecies.Add(net);
                    species.Add(newSpecies);
                }
                else { //found species that match this network
                    species[location].Add(net); //add net to the matched species list
                }
            }
        }
    }

    /// <summary>
    /// Generate tester gameobjects given the species list
    /// </summary>
    private void GeneratePopulation() {
        ResetWorld(); //reset world

        List<int[]> allID = new List<int[]>(); //2D id

        //width and height used for placing gameobjects
        float height = 25f; 
        float width = -25f;
        int numberOfSpecies = species.Count; //save species count

        //create id's based on number of species in the species list
        for (int i = 0; i < numberOfSpecies; i++) { 
            int numberOfNets = species[i].Count;
            for (int j = 0; j < numberOfNets; j++) {
                allID.Add(new int[] {i,j});
            }
        }

        for (int i = 0; i < numberOfSpecies; i++) { //run through species count
            int numberOfNets = species[i].Count; //number of species at i

            for (int j = 0; j < numberOfNets; j++) { //run through all species at i
                //pick a random id, save it locally and remove it from the id list
                int randomIndex = UnityEngine.Random.Range(0, allID.Count); 
                int[] randomId = allID[randomIndex]; 
                allID.RemoveAt(randomIndex);

                //create gameobject given location, network address, color and id value
                Color color = new Color(colors[randomId[0], 0], colors[randomId[0], 1], colors[randomId[0], 2]);
                CreateIndividual(new Vector3(width, height, 0), species[randomId[0]][randomId[1]], color, randomId);

                //update withd and height location
                if (width % 25 == 0 && width > 0) {
                    width = -25f;
                    height += -5f;
                }
                else
                    width += 5f;
            }
        }
    }

    /// <summary>
    /// Create a gameobject to test
    /// </summary>
    /// <param name="position">Position to place the object in world space</param>
    /// <param name="net">Network to activate within this gameobject</param>
    /// <param name="color">Color of this gameobject</param>
    /// <param name="id">ID of this gameobject</param>
    private void CreateIndividual(Vector3 position, NEATNet net, Color color, int[] id) {
        GameObject tester = (GameObject)Instantiate(testPrefab, position, testPrefab.transform.rotation); //Instantiate gameobject with prebaf
        tester.name = id[0] + "_" + id[1]; //set tester name to match id
        tester.SendMessage(ACTIVATE, net); //activate tester with net
        tester.transform.GetChild(0).GetComponent<Renderer>().material.color = color; //gave tester a color
        tester.GetComponent<Tester>().TestFinished += OnFinished; //subscribe to a delegate to know when a tester is finished its simulation
    }

    /// <summary>
    /// Delegate will inform this event method when tester object finishs its simulation
    /// </summary>
    /// <param name="source">NOT USED</param>
    /// <param name="ev">NOT USED</param>
    private void OnFinished(object source, EventArgs ev) {
        finished.WaitOne(); //mutex lock since test counter is a shared resource

        testCounter++; //increment counter 
        if (testCounter == populationSize) { //if all tests are finished
            DeleteWorld(); //delete enviroment
            Invoke("TestFinished",1f); //wait 1 second to make sure all gameobjects that need to be deleted are deleted
        }

        finished.Release(); //release lock
    }

    /// <summary>
    /// Calculate next size of species population using explicit fitness sharing distribution and breed previous generation asexually (elite) and sexually.
    /// </summary>
    private void TestFinished() {
        testCounter = 0; //reset test counter

        if (numberOfGenerationsToRun > 0) { //if computing
            generationNumber++; // increment generation number

            float highestFitness = 0f; //highest fitness to saved
            List<List<NEATNet>> bestSpecies = new List<List<NEATNet>>(); //50% best networks from each species list

            float[,] distribution = new float[species.Count,2]; //population distribution for species in the next generation
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
            lineGraph.GetComponent<LineGraphDrawer>().PlotData(highestFitness, "Generation Number: " + generationNumber + ", Highest Fitness: " + highestFitness + ", Delta: " + consultor.GetDeltaThreshold());
            netDrawer.SendMessage("DrawNet",bestNet);
            
            for (int i = 0; i < distribution.GetLength(0); i++) {
                distribution[i, 1] = (int)((distribution[i, 1] / totalSharedFitness) * populationSize);
                totalOrganisums += distribution[i, 1];
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

                        if (j > (float)distribution[i, 1] * elite) {
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
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="organisums"></param>
    /// <returns></returns>
    private float[,] SortedFitnessIndicies(List<NEATNet> organisums) {
        float[,] sorted = new float[organisums.Count, 2];
        for (int i = 0; i < sorted.GetLength(0); i++) {
            sorted[i, 0] = i;
            sorted[i, 1] = organisums[i].GetNetFitness();
        }
        return SortFitness(sorted);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sort"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
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

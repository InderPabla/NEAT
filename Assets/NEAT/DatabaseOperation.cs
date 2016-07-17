using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

/// <summary>
/// Provides insert and retrieve operations from the local database. 
/// User can save a neural network and retrieve all neural networks with a given name.
/// </summary>
public class DatabaseOperation  {


    private string insertPage = "http://localhost:8000/NEAT/insert_genotype.php?server=localhost&username=root&password=123&database=neat"; //insert php script
    private string retrievePage = "http://localhost:8000/NEAT/retrieve_genotype.php?server=localhost&username=root&password=123&database=neat"; //retrieve php script

    public NEATPacket[] retrieveNet; //packets of retrieved nets
    public WWW web; //used to connect to server side 
    public bool done = false; //to check if nets have been retrieved

    /// <summary>
    /// Save a given neural network into the databse by called the insert_genotype.php script with appropriate neural network information
    /// </summary>
    /// <param name="net">Neural network to save</param>
    /// <param name="name">Name of the neural network</param>
    /// <returns>Returns web information on executed url</returns>
    public IEnumerator SaveNet(NEATNet net,string name) {
        string page = insertPage; //insert page

        NEATConsultor consultor = net.GetConsultor(); //get consultor

        string genome = net.GetGenomeString(); //convert neural network genome to string
        string consultorGenome = consultor.GetGenomeString(); //convert master consultor genome to string

        int nodeTotal = net.GetNodeCount(); //node total to save
        int nodeInputs = net.GetNumberOfInputNodes(); //inputs to save
        int nodeOutputs = net.GetNumberOfOutputNodes(); //outputs to save
        int geneTotal = net.GetGeneCount(); //neural network gene count to save
        int genomeTotal = consultor.GetGeneCount(); //consultor genome gene count to save

        float fitness = net.GetNetFitness(); //net fitness to save

        page +=
            "&creature_name=" + name +
            "&creature_fitness=" + fitness +
            "&node_total=" + nodeTotal +
            "&node_inputs=" + nodeInputs +
            "&node_outputs=" + nodeOutputs +
            "&gene_total=" + geneTotal +
            "&genome_total=" + genomeTotal +
            "&genome=" + genome +
            "&consultor_genome=" + consultorGenome; //create insert page url

        web = new WWW(page); //run page
        yield return web; //return page when finished execution (retruns success echo if inserted corrected)
    }

    /// <summary>
    /// Retrieves all neural networks with the given name in JSON markup form
    /// </summary>
    /// <param name="name">Name of the neural network to retrieve</param>
    /// <returns>Returns web information on executed url</returns>
    public IEnumerator GetNet(string name) {
        string page = retrievePage; //retrieve page

        page +=
            "&creature_name=" + name; //create retrieve page url 

        done = false; //set to fasle to wait for web to return

        web = new WWW(page); //run page
        yield return web; //wait for web to execute url
        JsonParser(web.text); //parse given web text with JSON parser

        done = true; //retrieved is true
    }

    /// <summary>
    /// JSON parsing a given echo from retrieve_genotype.php script.
    /// Convert all JSON to NEAT packets to be converted to neural networks.
    /// </summary>
    /// <param name="jsonText">JSON string to parse</param>
    public void JsonParser(string jsonText) {
        JsonReader reader  = new JsonReader(jsonText); //read json text
        JsonData data = JsonMapper.ToObject(reader); //convert json text to json data

        List<NEATPacket> netPacketList = JsonMapper.ToObject<List<NEATPacket>>(data.ToJson());  //obtain each individual json object and map them to NEAT packets
        retrieveNet = netPacketList.ToArray(); //convert list of NEAT pakcets to an array
    }

}
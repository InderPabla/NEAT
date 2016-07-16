using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

/// <summary>
/// 
/// </summary>
public class DatabaseOperation  {


    private string insertPage = "http://localhost:8000/NEAT/insert_genotype.php?server=localhost&username=root&password=123&database=neat";
    private string retrievePage = "http://localhost:8000/NEAT/retrieve_genotype.php?server=localhost&username=root&password=123&database=neat";

    public NEATPacket[] retrieveNet;
    public WWW web;
    public bool done = false;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="net"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public IEnumerator SaveNet(NEATNet net,string name) {
        string page = insertPage;

        NEATConsultor consultor = net.GetConsultor();

        string genome = net.GetGenomeString();
        string consultorGenome = consultor.GetGenomeString();

        int nodeTotal = net.GetNodeCount();
        int nodeInputs = net.GetNumberOfInputNodes();
        int nodeOutputs = net.GetNumberOfOutputNodes();
        int geneTotal = net.GetGeneCount();
        int genomeTotal = consultor.GetGeneCount();

        float fitness = net.GetNetFitness();

        page +=
            "&creature_name=" + name +
            "&creature_fitness=" + fitness +
            "&node_total=" + nodeTotal +
            "&node_inputs=" + nodeInputs +
            "&node_outputs=" + nodeOutputs +
            "&gene_total=" + geneTotal +
            "&genome_total=" + genomeTotal +
            "&genome=" + genome +
            "&consultor_genome=" + consultorGenome;
        Debug.Log(page);
        web = new WWW(page);
        yield return web;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IEnumerator GetNet(string name) {
        string page = retrievePage;

        page +=
            "&creature_name=" + name;

        done = false;
        web = new WWW(page);
        yield return web;
        done = true;

        JsonParser(web.text);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jsonText"></param>
    public void JsonParser(string jsonText) {
        JsonReader reader  = new JsonReader(jsonText);
        JsonData data = JsonMapper.ToObject(reader);

        List<NEATPacket> netPacketList = JsonMapper.ToObject<List<NEATPacket>>(data.ToJson());
        retrieveNet = netPacketList.ToArray();
    }

}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class DatabaseOperation  {


    private string insertPage = "http://localhost:8000/NEAT/insert_genotype.php?server=localhost&username=root&password=123&database=neat";
    private string retrievePage = "http://localhost:8000/NEAT/retrieve_genotype.php?server=localhost&username=root&password=123&database=neat";

    public NEATPacket[] retrieveNet;
    public WWW web;
    public bool done = false;

    public DatabaseOperation()
    {

    }

    public IEnumerator SaveNet(NEATNet net,string name) {
        string page = insertPage;

        string genome = net.GetGenomeString();
        int nodeTotal = net.GetNodeCount();
        int nodeInputs = net.GetNumberOfInputNodes();
        int nodeOutputs = net.GetNumberOfOutputNodes();
        int geneTotal = net.GetGeneCount();

        page +=
            "&creature_name=" + name +
            "&node_total=" + nodeTotal +
            "&node_inputs=" + nodeInputs +
            "&node_outputs=" + nodeOutputs +
            "&gene_total=" + geneTotal +
            "&genome=" + genome;

        web = new WWW(page);
        yield return web;
    }

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

    public void JsonParser(string jsonText) {
        JsonReader reader  = new JsonReader(jsonText);
        JsonData data = JsonMapper.ToObject(reader);

        List<NEATPacket> netPacketList = JsonMapper.ToObject<List<NEATPacket>>(data.ToJson());
        retrieveNet = netPacketList.ToArray();
    }

}
<?php
	// This script is used to retrieve neural network information from the database

	// Packets of information to compile and send 
	class NEATPacket 
	{
		public $creature_id = 0; //id of the neural network
		public $creature_name  = ""; //name of the neural network
		public $creature_fitness  = 0; //fitness of the neural network
		public $node_total = 0; //total number of nodes in the neural network
		public $node_inputs= 0; //number of inputs in the neural network
		public $node_outputs = 0; //number of outputs in the neural network
		public $gene_total = 0; //number of genes that make up this neural network
		public $genome_total = 0; //total number of genes in the consultor genome
		public $genome = ""; //genome compiled into a string
		public $consultor_genome = ""; //consultor genome compiled into a string
	}

	$connection = new mysqli($_GET["server"], $_GET["username"], $_GET["password"]); //connect with given server IP, username and passwork

	if(mysqli_select_db($connection,$_GET["database"])) //connect to the databse
	{
		$node_table = "node"; //node table to retrieve genotype from
		
		$creature_name = $_GET["creature_name"]; //neural network name
		
		$node_retrieve_query = "SELECT * FROM $node_table WHERE creature_name = '$creature_name'";  //sql query 
		$result = mysqli_query($connection, $node_retrieve_query); //get all neural network information with the given name
		
		$stack = array(); //stack to add packets
		while ($row = mysqli_fetch_array($result))  //while row exists
		{ 					
			$neat = new NEATPacket(); //initilize pack
			
			$neat->creature_id =  intval($row[0]); //add id
			$neat->creature_name = $row[1]; //add name
			$neat->creature_fitness=  floatval($row[2]); //add fitness
			$neat->node_total=  intval($row[3]); //add total number of nodes
			$neat->node_inputs = intval($row[4]); //add inputs
			$neat->node_outputs = intval($row[5]); //add outputs
			$neat->gene_total = intval($row[6]); //add total gene count
			$neat->genome_total = intval($row[7]); //add total consultor gene count
			$neat->genome = $row[8]; //add string genome
			$neat->consultor_genome = $row[9]; //add string consultor genome
			
			array_push($stack, $neat ); //add packet to array
		}
		
		$JSONdata = json_encode($stack); //convert stack array to JSON storage type
		echo $JSONdata; //echo JSON data
	}
	else //unable to connect
	{
		echo "Databse does not exists"; //database error
	}
?>
<?php
	// This script is used to save neural network information into the database
	$connection = new mysqli($_GET["server"], $_GET["username"], $_GET["password"]); //connect with given server IP, username and passwork

	if(mysqli_select_db($connection,$_GET["database"])) //connect to the databse
	{
		$node_table = "node"; //node table to retrieve genotype from
		
		$creature_name = $_GET["creature_name"]; //name of the neural network 
		$creature_fitness = $_GET["creature_fitness"]; //fitness of the neural network
		$node_total = $_GET["node_total"]; //total number of nodes in the neural network
		$node_inputs = $_GET["node_inputs"]; //number of inputs in the neural network
		$node_outputs = $_GET["node_outputs"]; //number of outputs in the neural network
		$gene_total = $_GET["gene_total"]; //number of genes that make up this neural network
		$genome_total = $_GET["genome_total"];  //total number of genes in the consultor genome
		$genome = $_GET["genome"]; //genome compiled into a string
		$consultor_genome = $_GET["consultor_genome"]; //consultor genome compiled into a string

		//sql query to save the neural network
		$node_insert_query = "INSERT INTO $node_table (creature_name, creature_fitness, node_total, node_inputs, node_outputs, gene_total,genome_total,genome,consultor_genome) VALUES ('$creature_name', $creature_fitness, $node_total, $node_inputs, $node_outputs, $gene_total,$genome_total, '$genome','$consultor_genome')";
		
		$result = mysqli_query($connection, $node_insert_query); //save neural network

		if($result === true) //result is true
		{
			echo "SUCCESS"; //print success
		}	
		
	}
	else //error on connecting to database
	{
		echo "Databse does not exists"; //database error
	}
?>
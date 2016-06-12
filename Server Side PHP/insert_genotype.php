<?php
	$connection = new mysqli($_GET["server"], $_GET["username"], $_GET["password"]);

	if(mysqli_select_db($connection,$_GET["database"]))
	{
		$node_table = "node";
		
		$creature_name = $_GET["creature_name"];
		$node_total = $_GET["node_total"];
		$node_inputs = $_GET["node_inputs"];
		$node_outputs = $_GET["node_outputs"];
		$gene_total = $_GET["gene_total"];
		$genome_total = $_GET["genome_total"];
		$genome = $_GET["genome"];
		$consultor_genome = $_GET["consultor_genome"];
		
		
		$node_insert_query = "INSERT INTO $node_table (creature_name, node_total, node_inputs, node_outputs, gene_total,genome_total,genome,consultor_genome) VALUES ('$creature_name', $node_total, $node_inputs, $node_outputs, $gene_total,$genome_total, '$genome','$consultor_genome')";
		
		$result = mysqli_query($connection, $node_insert_query);
		
		if($result === true)
		{
			echo "SUCCESS";
		}	
	}
	else
	{
		echo "Databse does not exists";
	}
?>
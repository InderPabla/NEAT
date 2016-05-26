<?php
	class NEATPacket 
	{
		public $creature_id = 0;
		public $creature_name  = "";
		public $node_total = 0;
		public $node_inputs= 0;
		public $node_outputs = 0;
		public $gene_total = 0;
		public $genome_total = 0;
		public $genome = "";
		public $consultor_genome = "";
	}

	$connection = new mysqli($_GET["server"], $_GET["username"], $_GET["password"]);

	if(mysqli_select_db($connection,$_GET["database"]))
	{
		$node_table = "node";
		
		$creature_name = $_GET["creature_name"];
		
		$node_retrieve_query = "SELECT * FROM $node_table WHERE creature_name = '$creature_name'";
		$result = mysqli_query($connection, $node_retrieve_query);
		
		$stack = array();
		while ($row = mysqli_fetch_array($result)) 
		{ 					
			$neat = new NEATPacket();
			
			$neat->creature_id =  intval($row[0]);
			$neat->creature_name = $row[1];
			$neat->node_total=  intval($row[2]);
			$neat->node_inputs = intval($row[3]);
			$neat->node_outputs = intval($row[4]);
			$neat->gene_total = intval($row[5]);
			$neat->genome_total = intval($row[6]);
			$neat->genome = $row[7];
			$neat->consultor_genome = $row[8];
			
			array_push($stack, $neat );
		}
		
		$JSONdata = json_encode($stack);
		echo $JSONdata;
	}
	else
	{
		echo "Databse does not exists";
	}
?>
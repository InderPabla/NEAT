<?php
	$connection = new mysqli($_GET["server"], $_GET["username"], $_GET["password"]);

	if(mysqli_select_db($connection,$_GET["database"]))
	{
		$node_table = "node";
		
		$creature_id = $_GET["creature_id"];
		$creature_name = $_GET["creature_name"];
		
		$node_retrieve_query = "SELECT * FROM $node_table WHERE creature_name = '$creature_name'";
		
		$result = mysqli_query($connection, $node_retrieve_query);
	}
	else
	{
		echo "Databse does not exists";
	}
?>
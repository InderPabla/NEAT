<?php
	$connection = new mysqli($_GET["server"], $_GET["username"], $_GET["password"]);
	echo "SUCCESS";
?>
<?php

$con = mysqli_connect("localhost", "********", "********", '********');
if (mysqli_connect_errno()){
  echo "Failed to connect to MySQL: " . mysqli_connect_error();
}

$x = intval($_GET['x']);
$y = intval($_GET['y']);
$txt = mysqli_real_escape_string($con, str_replace('|', ' ', ($_GET['text'] ?: '')));
$floor = intval($_GET['floor'] ?: 0);
$pri = intval($_GET['priority'] ?: 0);
$elevator = intval($_GET['elevator'] ?: 0);
$floors = mysqli_real_escape_string($con, str_replace('|', ' ', ($_GET['floors'] ?: '')));

//mysqli_select_db($con,"marker_list");
$sql = "INSERT INTO hospital_project.marker_list (ID, X, Y, Text, Floor, Displaying, Elevator, Floors) VALUES (NULL, '".$x."', '".$y."', '".$txt."', '".$floor."', '".$pri."', '".$elevator."', '".$floors."');";
//$sql="SELECT * FROM marker_list";

$result = mysqli_query($con,$sql);


mysqli_close($con);

echo 'Done';
?>
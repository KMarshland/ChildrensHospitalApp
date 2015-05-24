<?php

//echo $floors;

$con = mysqli_connect("localhost", "********", "********", '********');
if (mysqli_connect_errno()){
  echo "Failed to connect to MySQL: " . mysqli_connect_error();
}

$q = intval($_GET['q']);
$txt = mysqli_real_escape_string($con, str_replace('|', ' ', ($_GET['text'] ?: '')));
$pri = intval($_GET['priority'] ?: 0);
$elevator = intval($_GET['elevator'] ?: 0);
$floors = mysqli_real_escape_string($con, str_replace('|', ' ', ($_GET['floors'] ?: '')));

//mysqli_select_db($con,"marker_list");
$sql = "UPDATE hospital_project.marker_list SET Text = '".$txt."' WHERE marker_list.ID = ".$q.";";
$result = mysqli_query($con,$sql);

$sql = "UPDATE hospital_project.marker_list SET Displaying = ".$pri." WHERE marker_list.ID = ".$q.";";
$result = mysqli_query($con,$sql);

$sql = "UPDATE hospital_project.marker_list SET Elevator = ".$elevator." WHERE marker_list.ID = ".$q.";";
$result = mysqli_query($con,$sql);

$sql = "UPDATE hospital_project.marker_list SET Floors = '".$floors."' WHERE marker_list.ID = ".$q.";";
$result = mysqli_query($con,$sql);
//echo $result;

mysqli_close($con);

//echo $sql;
?>
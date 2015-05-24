<?php

$con = mysqli_connect("localhost", "********", "********", '********');
if (mysqli_connect_errno()){
  echo "Failed to connect to MySQL: " . mysqli_connect_error();
}

//mysqli_select_db($con,"marker_list");
//$sql="SELECT * FROM marker_list WHERE ID = ".$q."";
$sql="SELECT * FROM marker_list";

$result = mysqli_query($con,$sql);

while($row = mysqli_fetch_array($result)){
  echo $row['ID'].'|'.$row['Text'].'|'.$row['X'].'|'.$row['Y'].'|'.$row['Displaying'].'|'.$row['Floor'].'|'.$row['Elevator'].'|'.$row['Floors'];
  echo '<br />';
}
//echo "</table>";

mysqli_close($con);
?>
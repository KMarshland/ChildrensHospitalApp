<?php
$q = intval($_GET['q']);

$con = mysqli_connect("localhost", "********", "********", '********');
if (mysqli_connect_errno()){
  echo "Failed to connect to MySQL: " . mysqli_connect_error();
}

//mysqli_select_db($con,"marker_list");
$sql="SELECT * FROM marker_list WHERE ID = ".$q."";
//$sql="SELECT * FROM marker_list";

$result = mysqli_query($con,$sql);




/*echo "<table border='1'>
<tr>
<th>X</th>
<th>Y</th>
<th>Text</th>
</tr>";*/

while($row = mysqli_fetch_array($result)){
  echo $q.'|'.$row['Text'].'|'.$row['X'].'|'.$row['Y'].'|'.$row['Displaying'].'|'.$row['Floor'].'|'.$row['Elevator'].'|'.$row['Floors'];
  /*echo "<tr>";
  echo "<td>" . $row['X'] . "</td>";
  echo "<td>" . $row['Y'] . "</td>";
  echo "<td>" . $row['Text'] . "</td>";
  echo "</tr>";*/
}
//echo "</table>";

mysqli_close($con);
?>
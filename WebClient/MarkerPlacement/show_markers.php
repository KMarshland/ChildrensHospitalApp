<?php

$con = mysqli_connect("localhost", "********", "********", '********');
if (mysqli_connect_errno()){
  echo "Failed to connect to MySQL: " . mysqli_connect_error();
}

//mysqli_select_db($con,"marker_list");
//$sql="SELECT * FROM marker_list WHERE ID = ".$q."";
$sql="SELECT * FROM marker_list";

$result = mysqli_query($con,$sql);

//echo var_dump($result);

echo "<table border='1'>
<tr>
<th>Text</th>
<th>X</th>
<th>Y</th>
<th>Displaying</th>
<th>Floor</th>
<th>Is Elevator</th>
<th>On Floors</th>
</tr>";

while($row = mysqli_fetch_array($result))
  {
  echo "<tr>";
  echo "<td>" . $row['Text'] . "</td>";
  echo "<td>" . $row['X'] . "</td>";
  echo "<td>" . $row['Y'] . "</td>";
  echo "<td>" . $row['Displaying'] . "</td>";
  echo "<td>" . $row['Floor'] . "</td>";
  echo "<td>" . $row['Elevator'] . "</td>";
  echo "<td>" . $row['Floors'] . "</td>";
  echo "</tr>";
  }
echo "</table>";

mysqli_close($con);
?>
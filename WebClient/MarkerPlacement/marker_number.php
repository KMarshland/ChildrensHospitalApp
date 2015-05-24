<?php

$con = mysqli_connect("localhost", "********", "********", '********');
if (mysqli_connect_errno()){
  echo "Failed to connect to MySQL: " . mysqli_connect_error();
}

$sql="SELECT MAX(ID) FROM marker_list";

$result = mysqli_query($con,$sql);
while($row = mysqli_fetch_array($result)){
    echo $row[0];
}

mysqli_close($con);
?>
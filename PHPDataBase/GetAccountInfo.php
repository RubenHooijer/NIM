<?php 

include 'DatabaseConnection.php';

$name = $_POST["username"];
$_name = $conn->real_escape_string($name);

$query = "SELECT wins, games 
FROM accounts 
WHERE username='$_name'";

$result = $conn->query($query);

if ($result->num_rows > 0){
    while($row = $result->fetch_assoc()) {
        echo $row["wins"] . "_" . $row["games"];
    }
} else {
    http_response_code(500);
}

if ($result && mysqli_num_rows($result) <= 0){
    http_response_code(500);
    die("Error (" . $errornr . ") " . $error);
}

$conn->close();

?>
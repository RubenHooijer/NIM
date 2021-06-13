<?php 

include 'DatabaseConnection.php';

$name = $_POST["username"];
$_name = $conn->real_escape_string($name);

$password = $_POST["password"];
$_password = $conn->real_escape_string($password);

$query = "INSERT INTO accounts (username, password) VALUES ('$_name', '$_password')";

if (!($result = $conn->query($query))){
    http_response_code(500);
    die("Error (" . $errornr . ") " . $error);
}
    
$conn->close();

?>
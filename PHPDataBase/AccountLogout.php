<?php 

include 'DatabaseConnection.php';

$name = $_POST["username"];
$_name = $conn->real_escape_string($name);

$query = "UPDATE accounts
SET isLoggedIn = null
WHERE username='$_name'";

if (!($result = $conn->query($query))){
    http_response_code(500);
    die("Error (" . $errornr . ") " . $error);
}

$conn->close();

?>
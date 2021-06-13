<?php 

include 'DatabaseConnection.php';

$name = $_POST["username"];
$_name = $conn->real_escape_string($name);

$isWin = $_POST["isWin"];
$_isWin = (int)$conn->real_escape_string($isWin);

$query = "UPDATE accounts
SET 
wins= wins + $isWin,
games= games + 1
WHERE username='$_name'";


if (!($result = $conn->query($query))){
    echo "Error (" . $errornr . ") " . $error;
    http_response_code(500);
    die("Error (" . $errornr . ") " . $error);
}
    
$conn->close();

?>
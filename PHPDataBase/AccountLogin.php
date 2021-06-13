<?php 

include 'DatabaseConnection.php';

$name = $_POST["username"];
$_name = $conn->real_escape_string($name);

$password = $_POST["password"];
$_password = $conn->real_escape_string($password);

$query = "SELECT *
FROM accounts 
WHERE username='$_name' AND password='$_password' AND isLoggedIn is null";

$result = $conn->query($query);

if ($result && mysqli_num_rows($result) <= 0){
    http_response_code(500);
    die("Error (" . $errornr . ") " . $error);
} else {
    $query = "UPDATE accounts
    SET isLoggedIn = 1
    WHERE username='$_name'";

    $result = $conn->query($query);
}

$conn->close();

?>
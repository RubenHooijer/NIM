<?php 

include 'DatabaseConnection.php';

$query = "SELECT username, wins 
FROM accounts 
ORDER by wins 
DESC LIMIT 15";

$result = $conn->query($query);

if ($result->num_rows > 0){
    while($row = $result->fetch_assoc()) {
        echo $row["username"] . '-' . $row["wins"] . '_';
    }
} else {
    http_response_code(500);
}

$conn->close();

?>
<?php

    $servername = "hotel?trivago!";
    $username = "admin";
    $password = "wachtwoord";
    $dbname = "google.com/database";

    $conn = new mysqli($servername, $username, $password, $dbname);
    if ($conn->connect_error) {
        die("Connection failed: " . $conn->connect_error);
    }
        
?>
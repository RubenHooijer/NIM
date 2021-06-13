-- phpMyAdmin SQL Dump
-- version 4.9.4
-- https://www.phpmyadmin.net/
--
-- Host: localhost
-- Gegenereerd op: 13 jun 2021 om 22:25
-- Serverversie: 10.2.32-MariaDB
-- PHP-versie: 5.5.14

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `rubenhooijer`
--

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `accounts`
--

DROP TABLE IF EXISTS `accounts`;
CREATE TABLE `accounts` (
  `username` tinytext DEFAULT NULL,
  `password` tinytext NOT NULL,
  `wins` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `games` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `isLoggedIn` tinyint(1) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Gegevens worden geëxporteerd voor tabel `accounts`
--

INSERT INTO `accounts` (`username`, `password`, `wins`, `games`, `isLoggedIn`) VALUES
('miauw', 'woef', 4, 5, NULL),
('kijkuitwaarjeje', 'wachtwoordenaangeeft', 6, 9, NULL),
('HenryPotter', 'abracadabra', 0, 0, NULL);

--
-- Indexen voor geëxporteerde tabellen
--

--
-- Indexen voor tabel `accounts`
--
ALTER TABLE `accounts`
  ADD UNIQUE KEY `username` (`username`(24));
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

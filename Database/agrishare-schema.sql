/*
SQLyog Ultimate v12.2.5 (64 bit)
MySQL - 5.7.21-log : Database - agrishare
*********************************************************************
*/

/*!40101 SET NAMES utf8 */;

/*!40101 SET SQL_MODE=''*/;

/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
/*Table structure for table `BookingUsers` */

DROP TABLE IF EXISTS `BookingUsers`;

CREATE TABLE `BookingUsers` (
  `Id` int(11) NOT NULL,
  `BookingId` int(11) NOT NULL,
  `Name` varchar(256) DEFAULT NULL,
  `Telephone` varchar(16) DEFAULT NULL,
  `Ratio` decimal(10,3) NOT NULL,
  `VerificationCode` varchar(8) DEFAULT NULL,
  `VerificationCodeExpiry` datetime NOT NULL,
  `StatusId` smallint(6) NOT NULL COMMENT 'Enum: Pending, Verified, Paid',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `BookingId` (`BookingId`),
  CONSTRAINT `bookingusers_ibfk_1` FOREIGN KEY (`BookingId`) REFERENCES `Bookings` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `Bookings` */

DROP TABLE IF EXISTS `Bookings`;

CREATE TABLE `Bookings` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ForId` smallint(6) NOT NULL COMMENT 'Enum: Me, Friend, Group',
  `UserId` int(11) NOT NULL,
  `ListingId` int(11) NOT NULL,
  `ServiceId` int(11) NOT NULL,
  `Location` varchar(256) DEFAULT NULL,
  `Latitude` decimal(11,8) NOT NULL DEFAULT '0.00000000',
  `Longitude` decimal(11,8) NOT NULL DEFAULT '0.00000000',
  `Quantity` decimal(10,3) NOT NULL DEFAULT '0.000',
  `Distance` decimal(10,3) NOT NULL,
  `IncludeFuel` tinyint(1) NOT NULL DEFAULT '0',
  `StartDate` date NOT NULL,
  `EndDate` date NOT NULL,
  `Price` decimal(10,3) NOT NULL DEFAULT '0.000',
  `StatusId` smallint(6) NOT NULL DEFAULT '0' COMMENT 'Enum: Pending, Approved, Declined, In Progress, Complete',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `UserId` (`UserId`),
  KEY `ListingId` (`ListingId`),
  KEY `bookings_ibfk_3` (`ServiceId`),
  CONSTRAINT `bookings_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `bookings_ibfk_2` FOREIGN KEY (`ListingId`) REFERENCES `Listings` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `bookings_ibfk_3` FOREIGN KEY (`ServiceId`) REFERENCES `Services` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Categories` */

DROP TABLE IF EXISTS `Categories`;

CREATE TABLE `Categories` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ParentId` int(11) DEFAULT NULL,
  `Title` varchar(256) DEFAULT NULL,
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Config` */

DROP TABLE IF EXISTS `Config`;

CREATE TABLE `Config` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Key` varchar(256) DEFAULT NULL,
  `Value` varchar(1024) DEFAULT NULL,
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Counters` */

DROP TABLE IF EXISTS `Counters`;

CREATE TABLE `Counters` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Event` varchar(16) DEFAULT NULL,
  `Category` varchar(16) DEFAULT NULL,
  `Date` date NOT NULL,
  `Hits` int(11) NOT NULL DEFAULT '0',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Faqs` */

DROP TABLE IF EXISTS `Faqs`;

CREATE TABLE `Faqs` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Question` varchar(4096) DEFAULT NULL,
  `Answer` text,
  `SortOrder` int(11) NOT NULL DEFAULT '0',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Listings` */

DROP TABLE IF EXISTS `Listings`;

CREATE TABLE `Listings` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) NOT NULL,
  `CategoryId` int(11) NOT NULL,
  `Title` varchar(256) DEFAULT NULL,
  `Description` varchar(2048) DEFAULT NULL,
  `Location` varchar(256) DEFAULT NULL,
  `Latitude` decimal(11,8) NOT NULL DEFAULT '0.00000000',
  `Longitude` decimal(11,8) NOT NULL DEFAULT '0.00000000',
  `Brand` varchar(256) DEFAULT NULL,
  `HorsePower` int(11) DEFAULT NULL,
  `Year` int(11) DEFAULT NULL,
  `Mobile` tinyint(1) NOT NULL DEFAULT '0',
  `ConditionId` smallint(6) NOT NULL DEFAULT '0',
  `GroupServices` tinyint(1) NOT NULL DEFAULT '0',
  `Photos` text,
  `AverageRating` decimal(10,3) NOT NULL DEFAULT '0.000',
  `RatingCount` int(11) NOT NULL DEFAULT '0',
  `StatusId` smallint(6) NOT NULL DEFAULT '0',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `UserId` (`UserId`),
  KEY `CategoryId` (`CategoryId`),
  CONSTRAINT `listings_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `listings_ibfk_2` FOREIGN KEY (`CategoryId`) REFERENCES `Categories` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Log` */

DROP TABLE IF EXISTS `Log`;

CREATE TABLE `Log` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `User` varchar(256) DEFAULT NULL,
  `Title` varchar(1024) DEFAULT NULL,
  `Description` longtext,
  `LevelId` smallint(6) NOT NULL DEFAULT '0',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=45 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Notifications` */

DROP TABLE IF EXISTS `Notifications`;

CREATE TABLE `Notifications` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) NOT NULL,
  `BookingId` int(11) DEFAULT NULL,
  `Title` varchar(1024) DEFAULT NULL,
  `TypeId` smallint(6) NOT NULL DEFAULT '0',
  `StatusId` smallint(6) NOT NULL DEFAULT '0',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `UserId` (`UserId`),
  KEY `BookingId` (`BookingId`),
  CONSTRAINT `notifications_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `notifications_ibfk_2` FOREIGN KEY (`BookingId`) REFERENCES `Bookings` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Pages` */

DROP TABLE IF EXISTS `Pages`;

CREATE TABLE `Pages` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ParentId` int(11) DEFAULT NULL,
  `TypeId` smallint(6) NOT NULL DEFAULT '0',
  `Title` varchar(256) DEFAULT NULL,
  `Description` varchar(160) DEFAULT NULL,
  `Content` longtext,
  `Icon` varchar(64) DEFAULT NULL,
  `UrlPath` varchar(256) DEFAULT NULL,
  `Greedy` tinyint(1) NOT NULL DEFAULT '0',
  `TemplatePath` varchar(1024) DEFAULT NULL,
  `ShowInMenu` tinyint(1) NOT NULL DEFAULT '1',
  `SortOrder` int(11) NOT NULL DEFAULT '1',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `cmspages_ibfk_1` (`ParentId`),
  CONSTRAINT `pages_ibfk_1` FOREIGN KEY (`ParentId`) REFERENCES `Pages` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Ratings` */

DROP TABLE IF EXISTS `Ratings`;

CREATE TABLE `Ratings` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ListingId` int(11) NOT NULL,
  `UserId` int(11) NOT NULL,
  `Comments` varchar(1024) DEFAULT NULL,
  `Rating` decimal(10,1) NOT NULL DEFAULT '0.0',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `ListingId` (`ListingId`),
  KEY `UserId` (`UserId`),
  CONSTRAINT `ratings_ibfk_1` FOREIGN KEY (`ListingId`) REFERENCES `Listings` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `ratings_ibfk_2` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Services` */

DROP TABLE IF EXISTS `Services`;

CREATE TABLE `Services` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ListingId` int(11) NOT NULL,
  `Mobile` tinyint(1) NOT NULL DEFAULT '0',
  `TotalVolume` decimal(10,3) NOT NULL DEFAULT '0.000',
  `QuantityUnitId` smallint(6) NOT NULL DEFAULT '0',
  `TimeUnitId` smallint(6) NOT NULL DEFAULT '0',
  `DistanceUnitId` smallint(6) NOT NULL DEFAULT '0',
  `MinimumQuantity` decimal(10,3) NOT NULL DEFAULT '0.000',
  `MaximumDistance` decimal(10,3) NOT NULL DEFAULT '0.000',
  `PricePerQuantityUnit` decimal(10,3) NOT NULL DEFAULT '0.000',
  `FuelPerQuantityUnit` decimal(10,3) NOT NULL DEFAULT '0.000',
  `TimePerQuantityUnit` decimal(10,3) NOT NULL DEFAULT '0.000',
  `PricePerDistanceUnit` decimal(10,3) NOT NULL DEFAULT '0.000',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `ListingId` (`ListingId`),
  CONSTRAINT `services_ibfk_1` FOREIGN KEY (`ListingId`) REFERENCES `Listings` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Templates` */

DROP TABLE IF EXISTS `Templates`;

CREATE TABLE `Templates` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `TypeId` smallint(6) NOT NULL DEFAULT '0',
  `Title` varchar(256) DEFAULT NULL,
  `HTML` longtext,
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Transactions` */

DROP TABLE IF EXISTS `Transactions`;

CREATE TABLE `Transactions` (
  `Id` int(11) NOT NULL,
  `UserId` int(11) NOT NULL,
  `BookingId` int(11) NOT NULL,
  `BookingUserId` int(11) DEFAULT NULL,
  `Reference` varchar(256) DEFAULT NULL,
  `Amount` decimal(10,3) NOT NULL DEFAULT '0.000',
  `StatusId` smallint(6) NOT NULL DEFAULT '0',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `UserId` (`UserId`),
  KEY `BookingId` (`BookingId`),
  KEY `BookingUserId` (`BookingUserId`),
  CONSTRAINT `transactions_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `transactions_ibfk_2` FOREIGN KEY (`BookingId`) REFERENCES `Bookings` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `transactions_ibfk_3` FOREIGN KEY (`BookingUserId`) REFERENCES `bookingusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Users` */

DROP TABLE IF EXISTS `Users`;

CREATE TABLE `Users` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `FirstName` varchar(256) DEFAULT NULL,
  `LastName` varchar(256) DEFAULT NULL,
  `EmailAddress` varchar(1024) DEFAULT NULL,
  `Telephone` varchar(16) DEFAULT NULL,
  `GenderId` smallint(6) NOT NULL DEFAULT '0',
  `DateOfBirth` date DEFAULT NULL,
  `Password` varchar(1024) DEFAULT NULL,
  `Salt` varchar(512) DEFAULT NULL,
  `AuthToken` varchar(1024) DEFAULT NULL,
  `FailedLoginAttempts` int(11) NOT NULL DEFAULT '0',
  `VerificationCode` varchar(8) DEFAULT NULL,
  `VerificationCodeExpiry` datetime DEFAULT NULL,
  `NotificationPreferences` smallint(6) NOT NULL DEFAULT '0',
  `InterestId` smallint(6) NOT NULL COMMENT 'Enum: Seeking, Offering',
  `SignalRConnectionId` varchar(256) DEFAULT NULL,
  `StatusId` smallint(6) NOT NULL DEFAULT '0' COMMENT 'Enum: Pending, Verified',
  `RoleList` varchar(1024) DEFAULT NULL,
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=10001 DEFAULT CHARSET=utf8mb4;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- MySQL Script generated by MySQL Workbench
-- Sun Mar  9 23:40:07 2025
-- Model: New Model    Version: 1.0
-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------
-- -----------------------------------------------------
-- Schema move_smart
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema move_smart
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `move_smart` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci ;
USE `move_smart` ;

-- -----------------------------------------------------
-- Table `move_smart`.`users`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `move_smart`.`users` (
  `UserID` INT NOT NULL AUTO_INCREMENT,
  `NationalNo` CHAR(14) NOT NULL,
  `Name` VARCHAR(150) CHARACTER SET 'utf8mb3' NOT NULL,
  `Role` ENUM('SuperUser','HospitalManager', 'GeneralManager', 'GeneralSupervisor', 'PatrolsSupervisor', 'WorkshopSupervisor', 'AdministrativeSupervisor') NOT NULL,
  `AccessRight` INT NOT NULL,
  `Password` VARCHAR(200) CHARACTER SET 'utf8mb3' NOT NULL,
  PRIMARY KEY (`UserID`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `move_smart`.`applications`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `move_smart`.`applications` (
  `ApplicationID` INT NOT NULL AUTO_INCREMENT,
  `CreationDate` DATE NOT NULL,
  `Status` ENUM('Confirmed', 'Rejected', 'Pending', 'Cancelled') NOT NULL,
  `ApplicationType` ENUM('JobOrder','MissionNote', 'SparePartWithdrawApplication', 'ConsumableWithdrawApplication', 'SparePartPurchaseOrder', 'ConsumablePurchaseOrder', 'MaintenanceApplication') NOT NULL,
  `ApplicationDescription` VARCHAR(2000) CHARACTER SET 'utf8mb3' NOT NULL,
  `CreatedByUserID` INT NOT NULL,
  PRIMARY KEY (`ApplicationID`),
  INDEX `FK_Applications_ApplicationTypes` (`ApplicationType` ASC) VISIBLE,
  INDEX `FK_Applications_Users` (`CreatedByUserID` ASC) VISIBLE,
  CONSTRAINT `FK_Applications_Users`
    FOREIGN KEY (`CreatedByUserID`)
    REFERENCES `move_smart`.`users` (`UserID`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `move_smart`.`vehicles`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `move_smart`.`vehicles` (
  `VehicleID` SMALLINT NOT NULL AUTO_INCREMENT,
  `BrandName` VARCHAR(50) CHARACTER SET 'utf8mb3' NOT NULL,
  `ModelName` VARCHAR(50) CHARACTER SET 'utf8mb3' NOT NULL,
  `PlateNumbers` VARCHAR(7) NOT NULL,
  `VehicleType` ENUM('SingleCab', 'DoubleCab', 'Truck', 'Sedan', 'Microbus', 'Minibus', 'Bus', 'Ambulance') NOT NULL,
  `AssociatedHospital` VARCHAR(50) CHARACTER SET 'utf8mb3' NOT NULL,
  `AssociatedTask` VARCHAR(100) CHARACTER SET 'utf8mb3' NOT NULL,
  `Status` ENUM('Available', 'Working', 'BrokenDown') NOT NULL,
  `TotalKilometersMoved` INT NOT NULL,
  `FuelType` ENUM('Gasoline', 'Diesel', 'NaturalGase') NOT NULL,
  `FuelConsumptionRate` TINYINT NOT NULL,
  `OilConsumptionRate` TINYINT NOT NULL,
  PRIMARY KEY (`VehicleID`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `move_smart`.`buses`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `move_smart`.`buses` (
  `BusID` TINYINT NOT NULL AUTO_INCREMENT,
  `Capacity` TINYINT NOT NULL,
  `AvailableSpace` TINYINT NOT NULL,
  `VehicleID` SMALLINT NOT NULL,
  PRIMARY KEY (`BusID`),
  INDEX `FK_Buses_Vehicles` (`VehicleID` ASC) VISIBLE,
  CONSTRAINT `FK_Buses_Vehicles`
    FOREIGN KEY (`VehicleID`)
    REFERENCES `move_smart`.`vehicles` (`VehicleID`))
ENGINE = InnoDB
AUTO_INCREMENT = 3
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `move_smart`.`vehicleconsumables`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `move_smart`.`vehicleconsumables` (
  `ConsumableID` TINYINT NOT NULL AUTO_INCREMENT,
  `ConsumableName` VARCHAR(100) CHARACTER SET 'utf8mb3' NOT NULL,
  `validityLength` INT NOT NULL,
  `Quantity` SMALLINT NOT NULL,
  PRIMARY KEY (`ConsumableID`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `move_smart`.`consumablespurchaseorders`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `move_smart`.`consumablespurchaseorders` (
  `OrderID` INT NOT NULL AUTO_INCREMENT,
  `ApplicationID` INT NOT NULL,
  `RequiredItem` TINYINT NOT NULL,
  `RequiredQuantity` SMALLINT NOT NULL,
  `ApprovedByGeneralSupervisor` BIT(1) NOT NULL,
  `ApprovedByGeneralManager` BIT(1) NOT NULL,
  PRIMARY KEY (`OrderID`),
  INDEX `FK_ConsumablesPurchaseOrders_Applications` (`ApplicationID` ASC) VISIBLE,
  INDEX `FK_ConsumablesPurchaseOrders_VehicleConsumables` (`RequiredItem` ASC) VISIBLE,
  CONSTRAINT `FK_ConsumablesPurchaseOrders_Applications`
    FOREIGN KEY (`ApplicationID`)
    REFERENCES `move_smart`.`applications` (`ApplicationID`),
  CONSTRAINT `FK_ConsumablesPurchaseOrders_VehicleConsumables`
    FOREIGN KEY (`RequiredItem`)
    REFERENCES `move_smart`.`vehicleconsumables` (`ConsumableID`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `move_smart`.`maintenanceapplications`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `move_smart`.`maintenanceapplications` (
  `MaintenanceApplicationID` INT NOT NULL AUTO_INCREMENT,
  `ApplicationID` INT NOT NULL,
  `VehicleID` SMALLINT NOT NULL,
  `ApprovedByGeneralSupervisor` BIT(1) NOT NULL,
  `ApprovedByGeneralManager` BIT(1) NOT NULL,
  PRIMARY KEY (`MaintenanceApplicationID`),
  INDEX `FK_MaintenanceApplications_Applications` (`ApplicationID` ASC) VISIBLE,
  INDEX `FK_MaintenanceApplications_Vehicles` (`VehicleID` ASC) VISIBLE,
  CONSTRAINT `FK_MaintenanceApplications_Applications`
    FOREIGN KEY (`ApplicationID`)
    REFERENCES `move_smart`.`applications` (`ApplicationID`),
  CONSTRAINT `FK_MaintenanceApplications_Vehicles`
    FOREIGN KEY (`VehicleID`)
    REFERENCES `move_smart`.`vehicles` (`VehicleID`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `move_smart`.`maintenance`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `move_smart`.`maintenance` (
  `MaintenanceID` INT NOT NULL AUTO_INCREMENT,
  `MaintenanceDate` DATE NOT NULL,
  `Description` VARCHAR(1000) CHARACTER SET 'utf8mb3' NOT NULL,
  `MaintenanceApplicationID` INT NOT NULL,
  PRIMARY KEY (`MaintenanceID`),
  INDEX `FK_Maintenance_MaintenanceApplications` (`MaintenanceApplicationID` ASC) VISIBLE,
  CONSTRAINT `FK_Maintenance_MaintenanceApplications`
    FOREIGN KEY (`MaintenanceApplicationID`)
    REFERENCES `move_smart`.`maintenanceapplications` (`MaintenanceApplicationID`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `move_smart`.`consumablesreplacements`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `move_smart`.`consumablesreplacements` (
  `ReplacementID` INT NOT NULL AUTO_INCREMENT,
  `MaintenanceID` INT NOT NULL,
  `ConsumableID` TINYINT NOT NULL,
  PRIMARY KEY (`ReplacementID`),
  INDEX `FK_ConsumablesReplacements_Maintenance` (`MaintenanceID` ASC) VISIBLE,
  INDEX `FK_ConsumablesReplacements_VehicleConsumables` (`ConsumableID` ASC) VISIBLE,
  CONSTRAINT `FK_ConsumablesReplacements_Maintenance`
    FOREIGN KEY (`MaintenanceID`)
    REFERENCES `move_smart`.`maintenance` (`MaintenanceID`),
  CONSTRAINT `FK_ConsumablesReplacements_VehicleConsumables`
    FOREIGN KEY (`ConsumableID`)
    REFERENCES `move_smart`.`vehicleconsumables` (`ConsumableID`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `move_smart`.`consumableswithdrawapplications`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `move_smart`.`consumableswithdrawapplications` (
  `WithdrawApplicationID` INT NOT NULL AUTO_INCREMENT,
  `ApplicationID` INT NOT NULL,
  `ConsumableID` TINYINT NOT NULL,
  `VehicleID` SMALLINT NOT NULL,
  `ApprovedByGeneralSupervisor` BIT(1) NOT NULL,
  `ApprovedByGeneralManager` BIT(1) NOT NULL,
  PRIMARY KEY (`WithdrawApplicationID`),
  INDEX `FK_ConsumablesWithdrawApplications_Applications` (`ApplicationID` ASC) VISIBLE,
  INDEX `FK_ConsumablesWithdrawApplications_VehicleConsumables` (`ConsumableID` ASC) VISIBLE,
  INDEX `FK_ConsumablesWithdrawApplications_Vehicles` (`VehicleID` ASC) VISIBLE,
  CONSTRAINT `FK_ConsumablesWithdrawApplications_Applications`
    FOREIGN KEY (`ApplicationID`)
    REFERENCES `move_smart`.`applications` (`ApplicationID`),
  CONSTRAINT `FK_ConsumablesWithdrawApplications_VehicleConsumables`
    FOREIGN KEY (`ConsumableID`)
    REFERENCES `move_smart`.`vehicleconsumables` (`ConsumableID`),
  CONSTRAINT `FK_ConsumablesWithdrawApplications_Vehicles`
    FOREIGN KEY (`VehicleID`)
    REFERENCES `move_smart`.`vehicles` (`VehicleID`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `move_smart`.`drivers`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `move_smart`.`drivers` (
  `DriverID` INT NOT NULL AUTO_INCREMENT,
  `NationalNo` CHAR(14) NOT NULL,
  `Name` VARCHAR(150) CHARACTER SET 'utf8mb3' NOT NULL,
  `Phone` CHAR(11) NOT NULL,
  `Status` ENUM('Available', 'Absent', 'Working') NOT NULL,
  `VehicleID` SMALLINT NOT NULL,
  PRIMARY KEY (`DriverID`),
  INDEX `FK_Drivers_Vehicle` (`VehicleID` ASC) VISIBLE,
  CONSTRAINT `FK_Drivers_Vehicle`
    FOREIGN KEY (`VehicleID`)
    REFERENCES `move_smart`.`vehicles` (`VehicleID`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `move_smart`.`employees`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `move_smart`.`employees` (
  `EmployeeID` INT NOT NULL AUTO_INCREMENT,
  `NationalNo` CHAR(14) NOT NULL,
  `Name` VARCHAR(150) CHARACTER SET 'utf8mb3' NOT NULL,
  `JobTitle` VARCHAR(50) CHARACTER SET 'utf8mb3' NOT NULL,
  `Phone` CHAR(11) NOT NULL,
  PRIMARY KEY (`EmployeeID`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `move_smart`.`joborders`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `move_smart`.`joborders` (
  `OrderID` INT NOT NULL AUTO_INCREMENT,
  `ApplicationID` INT NOT NULL,
  `VehicleID` SMALLINT NOT NULL,
  `DriverID` INT NOT NULL,
  `OrderStartDate` DATE NOT NULL,
  `OrderEndDate` DATE NULL DEFAULT NULL,
  `OrderStartTime` TIME NOT NULL,
  `OrderEndTime` TIME NULL DEFAULT NULL,
  `Destination` VARCHAR(100) CHARACTER SET 'utf8mb3' NOT NULL,
  `KilometersCounterBeforeOrder` INT NOT NULL,
  `KilometersCounterAfterOrder` INT NULL DEFAULT NULL,
  PRIMARY KEY (`OrderID`),
  INDEX `FK_JobOrders_Drivers` (`DriverID` ASC) VISIBLE,
  INDEX `FK_Orders_Applications` (`ApplicationID` ASC) VISIBLE,
  INDEX `FK_Orders_Vehicles` (`VehicleID` ASC) VISIBLE,
  CONSTRAINT `FK_JobOrders_Drivers`
    FOREIGN KEY (`DriverID`)
    REFERENCES `move_smart`.`drivers` (`DriverID`),
  CONSTRAINT `FK_Orders_Applications`
    FOREIGN KEY (`ApplicationID`)
    REFERENCES `move_smart`.`applications` (`ApplicationID`),
  CONSTRAINT `FK_Orders_Vehicles`
    FOREIGN KEY (`VehicleID`)
    REFERENCES `move_smart`.`vehicles` (`VehicleID`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `move_smart`.`missionsnotes`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `move_smart`.`missionsnotes` (
  `NoteID` INT NOT NULL AUTO_INCREMENT,
  `ApplicationID` INT NOT NULL,
  PRIMARY KEY (`NoteID`),
  INDEX `FK_MissionsNotes_Applications` (`ApplicationID` ASC) VISIBLE,
  CONSTRAINT `FK_MissionsNotes_Applications`
    FOREIGN KEY (`ApplicationID`)
    REFERENCES `move_smart`.`applications` (`ApplicationID`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `move_smart`.`missions`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `move_smart`.`missions` (
  `MissionID` INT NOT NULL AUTO_INCREMENT,
  `MissionNoteID` INT NOT NULL,
  `MissionStartDate` DATE NOT NULL,
  `MissionEndDate` DATE NOT NULL,
  `Destination` VARCHAR(1000) NOT NULL,
  `CreatedByUser` INT NOT NULL,
  PRIMARY KEY (`MissionID`),
  INDEX `FK_Missions_MissionsNotes` (`MissionNoteID` ASC) VISIBLE,
  CONSTRAINT `FK_Missions_MissionsNotes`
    FOREIGN KEY (`MissionNoteID`)
    REFERENCES `move_smart`.`missionsnotes` (`NoteID`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `move_smart`.`missionsjoborders`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `move_smart`.`missionsjoborders` (
  `MissionJobOrderID` INT NOT NULL AUTO_INCREMENT,
  `MissionID` INT NOT NULL,
  `JobOrderID` INT NOT NULL,
  PRIMARY KEY (`MissionJobOrderID`),
  INDEX `FK_MissionsJobOrders_JobOrders` (`JobOrderID` ASC) VISIBLE,
  INDEX `FK_MissionsJobOrders_Missions` (`MissionID` ASC) VISIBLE,
  CONSTRAINT `FK_MissionsJobOrders_JobOrders`
    FOREIGN KEY (`JobOrderID`)
    REFERENCES `move_smart`.`joborders` (`OrderID`),
  CONSTRAINT `FK_MissionsJobOrders_Missions`
    FOREIGN KEY (`MissionID`)
    REFERENCES `move_smart`.`missions` (`MissionID`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `move_smart`.`missionsvehicles`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `move_smart`.`missionsvehicles` (
  `MissionVehicleID` INT NOT NULL AUTO_INCREMENT,
  `MissionID` INT NOT NULL,
  `VehicleID` SMALLINT NOT NULL,
  PRIMARY KEY (`MissionVehicleID`),
  INDEX `FK_MissionsVehicles_Missions` (`MissionID` ASC) VISIBLE,
  INDEX `FK_MissionsVehicles_Vehicles` (`VehicleID` ASC) VISIBLE,
  CONSTRAINT `FK_MissionsVehicles_Missions`
    FOREIGN KEY (`MissionID`)
    REFERENCES `move_smart`.`missions` (`MissionID`),
  CONSTRAINT `FK_MissionsVehicles_Vehicles`
    FOREIGN KEY (`VehicleID`)
    REFERENCES `move_smart`.`vehicles` (`VehicleID`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `move_smart`.`patrols`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `move_smart`.`patrols` (
  `PatrolID` SMALLINT NOT NULL AUTO_INCREMENT,
  `Description` VARCHAR(2000) NOT NULL,
  `MovingAt` TIME NOT NULL,
  `ApproximatedTime` SMALLINT NOT NULL,
  `BusID` TINYINT NOT NULL,
  PRIMARY KEY (`PatrolID`),
  INDEX `FK_Patrols_Buses` (`BusID` ASC) VISIBLE,
  CONSTRAINT `FK_Patrols_Buses`
    FOREIGN KEY (`BusID`)
    REFERENCES `move_smart`.`buses` (`BusID`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `move_smart`.`patrolsubscriptions`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `move_smart`.`patrolsubscriptions` (
  `SubscriptionID` INT NOT NULL AUTO_INCREMENT,
  `EmployeeID` INT NOT NULL,
  `PatrolID` SMALLINT NOT NULL,
  `TransportationSubscriptionStatus` ENUM('Valid', 'Expired', 'Canceled', 'Unsubscriped') NOT NULL,
  PRIMARY KEY (`SubscriptionID`),
  INDEX `PatrolID_idx` (`PatrolID` ASC) VISIBLE,
  INDEX `FK_PatrolSubscriptions_Employees_idx` (`EmployeeID` ASC) VISIBLE,
  CONSTRAINT `FK_PatrolSubscriptions_Employees`
    FOREIGN KEY (`EmployeeID`)
    REFERENCES `move_smart`.`employees` (`EmployeeID`),
  CONSTRAINT `FK_PatrolSubscriptions_Patrols`
    FOREIGN KEY (`PatrolID`)
    REFERENCES `move_smart`.`patrols` (`PatrolID`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `move_smart`.`spareparts`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `move_smart`.`spareparts` (
  `SparePartID` SMALLINT NOT NULL AUTO_INCREMENT,
  `PartName` VARCHAR(100) CHARACTER SET 'utf8mb3' NOT NULL,
  `ValidityLength` INT NOT NULL,
  `Quantity` SMALLINT NOT NULL,
  PRIMARY KEY (`SparePartID`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `move_smart`.`sparepartspurchaseorders`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `move_smart`.`sparepartspurchaseorders` (
  `OrderID` INT NOT NULL AUTO_INCREMENT,
  `ApplicationID` INT NOT NULL,
  `RequiredItem` SMALLINT NOT NULL,
  `RequiredQuantity` SMALLINT NOT NULL,
  `ApprovedByGeneralSupervisor` BIT(1) NOT NULL,
  `ApprovedByGeneralManager` BIT(1) NOT NULL,
  PRIMARY KEY (`OrderID`),
  INDEX `FK_SparePartsPurchaseOrders_Applications` (`ApplicationID` ASC) VISIBLE,
  INDEX `FK_SparePartsPurchaseOrders_SpareParts` (`RequiredItem` ASC) VISIBLE,
  CONSTRAINT `FK_SparePartsPurchaseOrders_Applications`
    FOREIGN KEY (`ApplicationID`)
    REFERENCES `move_smart`.`applications` (`ApplicationID`),
  CONSTRAINT `FK_SparePartsPurchaseOrders_SpareParts`
    FOREIGN KEY (`RequiredItem`)
    REFERENCES `move_smart`.`spareparts` (`SparePartID`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `move_smart`.`sparepartsreplacements`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `move_smart`.`sparepartsreplacements` (
  `ReplacementID` INT NOT NULL AUTO_INCREMENT,
  `MaintenanceID` INT NOT NULL,
  `SparePartID` SMALLINT NOT NULL,
  PRIMARY KEY (`ReplacementID`),
  INDEX `FK_SparePartsReplacements_Maintenance` (`MaintenanceID` ASC) VISIBLE,
  INDEX `FK_SparePartsReplacements_SpareParts` (`SparePartID` ASC) VISIBLE,
  CONSTRAINT `FK_SparePartsReplacements_Maintenance`
    FOREIGN KEY (`MaintenanceID`)
    REFERENCES `move_smart`.`maintenance` (`MaintenanceID`),
  CONSTRAINT `FK_SparePartsReplacements_SpareParts`
    FOREIGN KEY (`SparePartID`)
    REFERENCES `move_smart`.`spareparts` (`SparePartID`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `move_smart`.`sparepartswithdrawapplications`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `move_smart`.`sparepartswithdrawapplications` (
  `WithdrawApplicationID` INT NOT NULL AUTO_INCREMENT,
  `ApplicationID` INT NOT NULL,
  `SparePartID` SMALLINT NOT NULL,
  `VehicleID` SMALLINT NOT NULL,
  `ApprovedByGeneralSupervisor` BIT(1) NOT NULL,
  `ApprovedByGeneralManager` BIT(1) NOT NULL,
  PRIMARY KEY (`WithdrawApplicationID`),
  INDEX `FK_SparePartsWithdrawApplications_Applications` (`ApplicationID` ASC) VISIBLE,
  INDEX `FK_SparePartsWithdrawApplications_SpareParts` (`SparePartID` ASC) VISIBLE,
  INDEX `FK_SparePartsWithdrawApplications_Vehicles` (`VehicleID` ASC) VISIBLE,
  CONSTRAINT `FK_SparePartsWithdrawApplications_Applications`
    FOREIGN KEY (`ApplicationID`)
    REFERENCES `move_smart`.`applications` (`ApplicationID`),
  CONSTRAINT `FK_SparePartsWithdrawApplications_SpareParts`
    FOREIGN KEY (`SparePartID`)
    REFERENCES `move_smart`.`spareparts` (`SparePartID`),
  CONSTRAINT `FK_SparePartsWithdrawApplications_Vehicles`
    FOREIGN KEY (`VehicleID`)
    REFERENCES `move_smart`.`vehicles` (`VehicleID`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `move_smart`.`vacations`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `move_smart`.`vacations` (
  `VacationID` INT NOT NULL AUTO_INCREMENT,
  `StartDate` DATE NOT NULL,
  `EndDate` DATE NOT NULL,
  `VacationOwnerID` INT NOT NULL,
  `SubstituteDriverID` INT NOT NULL,
  PRIMARY KEY (`VacationID`),
  INDEX `FK_Vacations_Drivers_SubstituteDriver` (`SubstituteDriverID` ASC) VISIBLE,
  INDEX `FK_Vacations_Drivers_VacationOwner` (`VacationOwnerID` ASC) VISIBLE,
  CONSTRAINT `FK_Vacations_Drivers_SubstituteDriver`
    FOREIGN KEY (`SubstituteDriverID`)
    REFERENCES `move_smart`.`drivers` (`DriverID`),
  CONSTRAINT `FK_Vacations_Drivers_VacationOwner`
    FOREIGN KEY (`VacationOwnerID`)
    REFERENCES `move_smart`.`drivers` (`DriverID`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;

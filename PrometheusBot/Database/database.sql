-- ----------------------------------------------------------------------------
-- MySQL Workbench Migration
-- Migrated Schemata: PrometheusDB
-- Source Schemata: PrometheusDB
-- Created: Fri Mar 26 18:41:19 2021
-- Workbench Version: 8.0.22
-- ----------------------------------------------------------------------------

SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------------------------------------------------------
-- Schema PrometheusDB
-- ----------------------------------------------------------------------------
DROP SCHEMA IF EXISTS `PrometheusDB` ;
CREATE SCHEMA IF NOT EXISTS `PrometheusDB` DEFAULT CHARACTER SET utf8 ;

-- ----------------------------------------------------------------------------
-- Table PrometheusDB.settings
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `PrometheusDB`.`settings` (
  `setting_name` VARCHAR(45) NOT NULL,
  `ids` VARCHAR(64) NOT NULL,
  `value` VARCHAR(2048) NOT NULL,
  PRIMARY KEY (`setting_name`, `ids`),
  UNIQUE INDEX `id_UNIQUE` (`ids` ASC) VISIBLE)
ENGINE = InnoDB;
SET FOREIGN_KEY_CHECKS = 1;

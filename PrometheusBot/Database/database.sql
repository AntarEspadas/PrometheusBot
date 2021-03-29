-- ----------------------------------------------------------------------------
-- MySQL Workbench Migration
-- Migrated Schemata: PrometheusDB
-- Source Schemata: PrometheusDB
-- Created: Mon Mar 29 00:33:31 2021
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

-- ----------------------------------------------------------------------------
-- Table PrometheusDB.reactions
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `PrometheusDB`.`reactions` (
  `guild_id` BIGINT UNSIGNED NOT NULL,
  `id` VARCHAR(25) NOT NULL,
  `text_trigger` VARCHAR(100) NULL,
  `text_response` VARCHAR(1000) NULL,
  `anywhere` TINYINT NOT NULL DEFAULT 0,
  `weight` INT NOT NULL DEFAULT 1,
  PRIMARY KEY (`guild_id`, `id`))
ENGINE = InnoDB;

-- ----------------------------------------------------------------------------
-- View PrometheusDB.anywhere_reactions
-- ----------------------------------------------------------------------------
USE `PrometheusDB`;
CREATE  OR REPLACE VIEW `anywhere_reactions` AS
select guild_id, text_trigger, text_response, weight
from reactions
where anywhere;
SET FOREIGN_KEY_CHECKS = 1;

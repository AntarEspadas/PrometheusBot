-- ----------------------------------------------------------------------------
-- MySQL Workbench Migration
-- Migrated Schemata: PrometheusDB
-- Source Schemata: PrometheusDB
-- Created: Wed Mar 24 01:10:27 2021
-- Workbench Version: 8.0.22
-- ----------------------------------------------------------------------------

SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------------------------------------------------------
-- Schema PrometheusDB
-- ----------------------------------------------------------------------------
DROP SCHEMA IF EXISTS `PrometheusDB` ;
CREATE SCHEMA IF NOT EXISTS `PrometheusDB` DEFAULT CHARACTER SET utf8 ;

-- ----------------------------------------------------------------------------
-- Table PrometheusDB.setting_info
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `PrometheusDB`.`setting_info` (
  `id` VARCHAR(45) NOT NULL,
  `permission_role` TINYINT NOT NULL,
  `validation_regex` VARCHAR(100) NOT NULL,
  `raw_error_message` VARCHAR(100) NOT NULL,
  `scope` ENUM('Guild', 'User', 'Both') NOT NULL,
  `visible` TINYINT NOT NULL,
  `type` VARCHAR(100) NOT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB;

-- ----------------------------------------------------------------------------
-- Table PrometheusDB.user
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `PrometheusDB`.`user` (
  `id` VARCHAR(25) NOT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB;

-- ----------------------------------------------------------------------------
-- Table PrometheusDB.guild
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `PrometheusDB`.`guild` (
  `id` INT NOT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB;

-- ----------------------------------------------------------------------------
-- Table PrometheusDB.Channel
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `PrometheusDB`.`Channel` (
  `id` INT NOT NULL,
  `guild_id` INT NOT NULL,
  PRIMARY KEY (`id`, `guild_id`),
  INDEX `fk_Channel_guild1_idx` (`guild_id` ASC) VISIBLE,
  CONSTRAINT `fk_Channel_guild1`
    FOREIGN KEY (`guild_id`)
    REFERENCES `PrometheusDB`.`guild` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

-- ----------------------------------------------------------------------------
-- Table PrometheusDB.global_user_setting
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `PrometheusDB`.`global_user_setting` (
  `user_id` VARCHAR(25) NOT NULL,
  `setting_info_id` VARCHAR(45) NOT NULL,
  `value` VARCHAR(256) NULL,
  PRIMARY KEY (`user_id`, `setting_info_id`),
  INDEX `fk_global_user_setting_setting1_idx` (`setting_info_id` ASC) VISIBLE,
  CONSTRAINT `fk_global_user_setting_user`
    FOREIGN KEY (`user_id`)
    REFERENCES `PrometheusDB`.`user` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_global_user_setting_setting1`
    FOREIGN KEY (`setting_info_id`)
    REFERENCES `PrometheusDB`.`setting_info` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

-- ----------------------------------------------------------------------------
-- Table PrometheusDB.guild_user_setting
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `PrometheusDB`.`guild_user_setting` (
  `guild_id` INT NOT NULL,
  `user_id` VARCHAR(25) NOT NULL,
  `setting_info_id` VARCHAR(45) NOT NULL,
  `value` VARCHAR(256) NULL,
  PRIMARY KEY (`guild_id`, `user_id`, `setting_info_id`),
  INDEX `fk_guild_user_setting_user1_idx` (`user_id` ASC) VISIBLE,
  INDEX `fk_guild_user_setting_setting_info1_idx` (`setting_info_id` ASC) VISIBLE,
  CONSTRAINT `fk_guild_user_setting_guild1`
    FOREIGN KEY (`guild_id`)
    REFERENCES `PrometheusDB`.`guild` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_guild_user_setting_user1`
    FOREIGN KEY (`user_id`)
    REFERENCES `PrometheusDB`.`user` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_guild_user_setting_setting_info1`
    FOREIGN KEY (`setting_info_id`)
    REFERENCES `PrometheusDB`.`setting_info` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

-- ----------------------------------------------------------------------------
-- Table PrometheusDB.guild_channel_setting
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `PrometheusDB`.`guild_channel_setting` (
  `Channel_id` INT NOT NULL,
  `Channel_guild_id` INT NOT NULL,
  `setting_info_id` VARCHAR(45) NOT NULL,
  `value` VARCHAR(256) NULL,
  PRIMARY KEY (`Channel_id`, `Channel_guild_id`, `setting_info_id`),
  INDEX `fk_guild_channel_setting_setting_info1_idx` (`setting_info_id` ASC) VISIBLE,
  CONSTRAINT `fk_guild_channel_setting_Channel1`
    FOREIGN KEY (`Channel_id` , `Channel_guild_id`)
    REFERENCES `PrometheusDB`.`Channel` (`id` , `guild_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_guild_channel_setting_setting_info1`
    FOREIGN KEY (`setting_info_id`)
    REFERENCES `PrometheusDB`.`setting_info` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;
SET FOREIGN_KEY_CHECKS = 1;

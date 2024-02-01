/*
 Navicat MySQL Data Transfer

 Source Server         : local
 Source Server Type    : MySQL
 Source Server Version : 50651
 Source Host           : localhost:3306
 Source Schema         : work

 Target Server Type    : MySQL
 Target Server Version : 50651
 File Encoding         : 65001

 Date: 01/02/2024 14:43:49
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for work_daddy
-- ----------------------------
DROP TABLE IF EXISTS `work_daddy`;
CREATE TABLE `work_daddy`  (
  `hash` varchar(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `content` varchar(1024) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `link` varchar(511) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `date` datetime(0) NOT NULL,
  `type` int(4) UNSIGNED NOT NULL DEFAULT 0,
  PRIMARY KEY (`hash`) USING BTREE,
  INDEX `content`(`content`(255)) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Compact;

-- ----------------------------
-- Table structure for work_hotlist
-- ----------------------------
DROP TABLE IF EXISTS `work_hotlist`;
CREATE TABLE `work_hotlist`  (
  `hash` varchar(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `content` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `platform` int(10) UNSIGNED NOT NULL,
  `date` datetime(0) NOT NULL,
  `link` varchar(511) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`hash`) USING BTREE,
  INDEX `content`(`content`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Compact;

-- ----------------------------
-- Table structure for work_keyword
-- ----------------------------
DROP TABLE IF EXISTS `work_keyword`;
CREATE TABLE `work_keyword`  (
  `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `word` varchar(63) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `type` int(8) UNSIGNED NOT NULL,
  `extend` varchar(63) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 267 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Compact;

-- ----------------------------
-- Table structure for work_media
-- ----------------------------
DROP TABLE IF EXISTS `work_media`;
CREATE TABLE `work_media`  (
  `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` varchar(63) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `domain` varchar(127) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `name`(`name`) USING BTREE,
  INDEX `domain`(`domain`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 67 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Compact;

-- ----------------------------
-- Table structure for work_news
-- ----------------------------
DROP TABLE IF EXISTS `work_news`;
CREATE TABLE `work_news`  (
  `hash` varchar(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `link` varchar(511) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `medium` int(10) UNSIGNED NOT NULL,
  `title` varchar(511) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `date` datetime(0) NOT NULL,
  `status` tinyint(255) UNSIGNED NOT NULL,
  `tags` varchar(127) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `keyword` varchar(63) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`hash`) USING BTREE,
  INDEX `title`(`title`(255)) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Compact;

-- ----------------------------
-- Table structure for work_platform
-- ----------------------------
DROP TABLE IF EXISTS `work_platform`;
CREATE TABLE `work_platform`  (
  `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` varchar(63) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 13 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Compact;

SET FOREIGN_KEY_CHECKS = 1;

CREATE TABLE `Users` (
  `id` BIGINT PRIMARY KEY AUTO_INCREMENT,
  `userId` varchar (256) NOT NULL DEFAULT '',
  `platform` int NOT NULL,
  `nickName` varchar (256) NULL DEFAULT '',
  `ticketId` int NULL,
  `messageCount` int NOT NULL DEFAULT 0,
  `totalPay` int NOT NULL DEFAULT 0,
  `totalPayGuest` int NOT NULL DEFAULT 0,
  `joinTimestamp` datetime NULL,
  `lastTimestamp` datetime NULL,
  `createdUtc` datetime NULL,
  `updatedUtc` datetime NULL,
  unique `userIdPlatform` USING btree (`userId`, `platform`)
) ENGINE = innodb DEFAULT CHARACTER SET = "utf8mb4" COLLATE = "utf8mb4_0900_ai_ci" ROW_FORMAT = Compressed
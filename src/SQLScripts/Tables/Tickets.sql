CREATE TABLE `Tickets` (
  `id` INT PRIMARY KEY AUTO_INCREMENT,
  `code` varchar (50) NOT NULL DEFAULT '',
  `platform` int NULL,
  `event` int NULL,
  `ticketType` int NOT NULL,
  `isDistributed` boolean NOT NULL,
  `isClaimed` boolean NOT NULL,
  `isActivated` boolean NOT NULL,
  `createdUtc` datetime NOT NULL,
  `updatedUtc` datetime NOT NULL,
  unique `code` USING btree (`code`)
) ENGINE = innodb DEFAULT CHARACTER SET = "utf8mb4" COLLATE = "utf8mb4_0900_ai_ci" ROW_FORMAT = Compressed
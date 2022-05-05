CREATE TABLE `H5Profiles` (
  `id` BIGINT PRIMARY KEY AUTO_INCREMENT,
  `role` varchar (256) NOT NULL DEFAULT '',
  `items` varchar (256) NOT NULL DEFAULT '',
  `title` varchar (256) NOT NULL DEFAULT '',
  `ticketPlatform` int NOT NULL DEFAULT 0,
  `livePlatform` int NOT NULL DEFAULT 0,
  `nickName` varchar (256) NULL DEFAULT '',
  `ticketId` int NULL,
  `createdUtc` datetime NULL,
  `updatedUtc` datetime NULL,
  unique `nickNameLivePlatform` USING btree (`nickName`, `livePlatform`)
) ENGINE = innodb DEFAULT CHARACTER SET = "utf8mb4" COLLATE = "utf8mb4_0900_ai_ci" ROW_FORMAT = Compressed
CREATE TABLE `Sessions` (
  `id` bigint NOT NULL,
  `anchorId` varchar (256) NOT NULL DEFAULT '',
  `sessionId` varchar (256) NOT NULL DEFAULT '',
  `isActive` boolean NOT NULL,
  `createdUtc` datetime NOT NULL,
  `updatedUtc` datetime NOT NULL,
  PRIMARY KEY (`id`),
  unique `sessionId` USING btree (`sessionId`)
) ENGINE = innodb DEFAULT CHARACTER SET = "utf8mb4" COLLATE = "utf8mb4_0900_ai_ci" ROW_FORMAT = Compressed
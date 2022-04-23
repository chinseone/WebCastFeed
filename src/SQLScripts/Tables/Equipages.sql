CREATE TABLE `Equipages` (
  `id` INT PRIMARY KEY AUTO_INCREMENT,
  `userId` bigint NOT NULL,
  `name` varchar (256) NOT NULL DEFAULT '',
  `createdUtc` datetime NOT NULL,
  `updatedUtc` datetime NOT NULL,
  index `userId` USING btree (`userId`)
) ENGINE = innodb DEFAULT CHARACTER SET = "utf8mb4" COLLATE = "utf8mb4_0900_ai_ci" ROW_FORMAT = Compressed
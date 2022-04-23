USE xiugou_local;

CREATE TABLE IF NOT EXISTS Users (
    id BIGINT PRIMARY KEY AUTO_INCREMENT,
    userId VARCHAR(256) NOT NULL,
    platform INT NOT NULL,
    nickName VARCHAR(256),
    ticketId INT,
    messageCount INT NOT NULL,
    totalPay INT NOT NULL,
    totalPayGuest INT NOT NULL,
    joinTimestamp DATETIME NOT NULL,
    LastTimestamp DATETIME NOT NULL,
    createdUtc DATETIME NOT NULL,
    updatedUtc DATETIME NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci ROW_FORMAT=COMPRESSED;

# INDEX userIdPlatform(userId, platform)
SET @x := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS WHERE TABLE_NAME = 'Users' AND INDEX_NAME = 'userIdPlatform' AND TABLE_SCHEMA = DATABASE());
SET @y := 'Alter Table Users ADD UNIQUE Index userIdPlatform(userId, platform);';
SET @sql := if( @x > 0, 'select ''Index code exists.''', @y);
PREPARE stmt FROM @sql;
EXECUTE stmt;
USE xiugou_local;

CREATE TABLE IF NOT EXISTS Tickets (
    id INT PRIMARY KEY AUTO_INCREMENT,
    code VARCHAR(50) NOT NULL,
    platform INT,
    event INT,
    ticketType INT NOT NULL,
    isDistributed BOOLEAN NOT NULL,
    isClaimed BOOLEAN NOT NULL,
    isActivated BOOLEAN NOT NULL,
    createdUtc DATETIME NOT NULL,
    updatedUtc DATETIME NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci ROW_FORMAT=COMPRESSED;

# INDEX code(code)
SET @x := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS WHERE TABLE_NAME = 'Tickets' AND INDEX_NAME = 'code' AND TABLE_SCHEMA = DATABASE());
SET @y := 'Alter Table Tickets ADD UNIQUE Index code(code);';
SET @sql := if( @x > 0, 'select ''Index code exists.''', @y);
PREPARE stmt FROM @sql;
EXECUTE stmt;
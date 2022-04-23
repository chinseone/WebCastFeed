USE xiugou_local;

CREATE TABLE IF NOT EXISTS Equipages (
    id INT PRIMARY KEY AUTO_INCREMENT,
    userId BIGINT NOT NULL,
    name VARCHAR(256),
    createdUtc DATETIME NOT NULL,
    updatedUtc DATETIME NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci ROW_FORMAT=COMPRESSED;

# INDEX userId(userId)
SET @x := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS WHERE TABLE_NAME = 'Equipages' AND INDEX_NAME = 'userId' AND TABLE_SCHEMA = DATABASE());
SET @y := 'Alter Table Equipages ADD Index userId(userId);';
SET @sql := if( @x > 0, 'select ''Index code exists.''', @y);
PREPARE stmt FROM @sql;
EXECUTE stmt;
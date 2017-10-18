CREATE TABLE `Config` (
    ConfigID VARCHAR(50) NOT NULL,
    Value TEXT,
    PRIMARY KEY (ConfigID)
);

CREATE TABLE `Projects` (
    ProjectID int NOT NULL AUTO_INCREMENT,
    Name VARCHAR(255) NOT NULL,
    ShortDesc TEXT NOT NULL,
    LongDesc TEXT NOT NULL,
    UnityOrgID VARCHAR(255) NOT NULL,
    UnityProjectID VARCHAR(255) NOT NULL,
    GDocFolderID VARCHAR(255) NOT NULL,
    PRIMARY KEY (ProjectID)
);

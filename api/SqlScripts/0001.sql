
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
BEGIN
    CREATE TABLE Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Email NVARCHAR(255) NOT NULL,
        Password NVARCHAR(1000) NOT NULL,
        WasPasswordHashed BIT NOT NULL,
        Birthday DATE NOT NULL,
        PublicFigure NVARCHAR(255) NULL,
        IsAdmin BIT NOT NULL,
    );
END

CREATE TABLE[dbo].[Customer](
    [Id]       INT            IDENTITY(1, 1) NOT NULL,
    [Name]     NVARCHAR(50)  NOT NULL,
    [Password] NVARCHAR(50)  NOT NULL,
    [tel]      NVARCHAR(15)  NOT NULL,
    [Email]    NVARCHAR(50)  NOT NULL,
    [guid]     NVARCHAR(100) NOT NULL,
    [Active]   NVARCHAR(100) NULL
);

CREATE TABLE[dbo].[Que](
    [id]         INT      IDENTITY(1000, 1) NOT NULL,
    [UserId]     INT      NOT NULL,
    [FromDate]   DATETIME NOT NULL,
    [ToDate]     DATETIME NOT NULL,
    [QueType]    INT      NOT NULL,
    [EmployeeId] INT      NOT NULL,
    [CustomerId] INT      NOT NULL
);

CREATE TABLE[dbo].[Users](
    [Id]         INT            IDENTITY(1, 1) NOT NULL,
    [User]       NVARCHAR(50)  NOT NULL,
    [Email]      NVARCHAR(50)  NOT NULL,
    [Tel]        NVARCHAR(15)  NOT NULL,
    [BizName]    NVARCHAR(50)  NOT NULL,
    [Password]   NVARCHAR(50)  NOT NULL,
    [BizNameEng] NVARCHAR(50)  NOT NULL,
    [Adrress]    NVARCHAR(100) NOT NULL,
    [City]       NVARCHAR(50)  NOT NULL,
    [Country]    NVARCHAR(50)  NOT NULL,
    [CreditCard] NVARCHAR(50)  NOT NULL,
    [guid]       NVARCHAR(50)  NOT NULL,
    [Active]     NVARCHAR(50)  NULL
);

CREATE TABLE[dbo].[UsersActivitiesTypes](
    [Id]             INT            IDENTITY(1, 1) NOT NULL,
    [UserId]         INT            NOT NULL,
    [Name]           NVARCHAR(100) NOT NULL,
    [ActiveDuration] INT            NOT NULL
);

CREATE TABLE[dbo].[UsersActivity](
    [EmployeeId]         INT           IDENTITY(1, 1) NOT NULL,
    [UserId]             INT           NOT NULL,
    [EmployeeName]       NVARCHAR(50) NOT NULL,
    [ActiveDay]          TINYINT       NOT NULL,
    [ActiveHourFrom]     NVARCHAR(10) NOT NULL,
    [ActiveHourTo]       NVARCHAR(10) NOT NULL,
    [ActiveHourFromNone] NVARCHAR(10) NOT NULL,
    [ActiveHourToNone]   NVARCHAR(10) NOT NULL
);

CREATE TABLE[dbo].[UsersToCusomers](
    [UserId]     INT NOT NULL,
    [CustomerId] INT NOT NULL
);

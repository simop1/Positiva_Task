CREATE TABLE [dbo].[Users] (
    [UserID]      INT           IDENTITY (1, 1) NOT NULL,
    [FirstName]      NVARCHAR (50) NOT NULL,
	[LastName]       NVARCHAR (50) NOT NULL,
    [UserName]		 NVARCHAR (50) NOT NULL,
    [Email] NCHAR(50) NOT NULL, 
    [UserPassword] NCHAR(50) NOT NULL, 
	[DateOfBirth] datetime2 NOT NULL, 
    PRIMARY KEY CLUSTERED ([UserID] ASC)
)
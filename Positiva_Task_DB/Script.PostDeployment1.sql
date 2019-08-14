/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
MERGE INTO Users AS Target
USING (VALUES 
        (1, 'Admin', 'Admin', 'Admin', 'test@gmail.com', 'Positiva123', '1993-06-14')
)
AS Source (UserID, FirstName, LastName, UserName, Email, UserPassword, DateOfBirth)
ON Target.UserID = Source.UserID
WHEN NOT MATCHED BY TARGET THEN
INSERT (FirstName, LastName, UserName, Email, UserPassword, DateOfBirth)
VALUES (FirstName, LastName, UserName, Email, UserPassword, DateOfBirth);
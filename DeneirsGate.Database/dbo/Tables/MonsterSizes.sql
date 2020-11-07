CREATE TABLE [dbo].[MonsterSizes]
(
    [SizeKey] UNIQUEIDENTIFIER NOT NULL,
    [Name]    NVARCHAR (50)    NOT NULL,
    [SortOrder] INT NOT NULL, 
    PRIMARY KEY CLUSTERED ([SizeKey] ASC)
)

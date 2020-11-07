CREATE TABLE [dbo].[MonsterTypes]
(
    [TypeKey] UNIQUEIDENTIFIER NOT NULL,
    [Name]    NVARCHAR (50)    NOT NULL,
    PRIMARY KEY CLUSTERED ([TypeKey] ASC)
)

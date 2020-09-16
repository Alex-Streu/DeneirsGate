CREATE TABLE [dbo].[MagicItemTypes]
(
    [TypeKey] UNIQUEIDENTIFIER NOT NULL,
    [Name]    NVARCHAR (50)    NOT NULL,
    PRIMARY KEY CLUSTERED ([TypeKey] ASC)
)

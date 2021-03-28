CREATE TABLE [dbo].[SettlementLocations]
(
    [LocationKey]   UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [SettlementKey] UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [SortOrder]     INT NOT NULL,
    [X]             FLOAT NULL,
    [Y]             FLOAT NULL
    PRIMARY KEY CLUSTERED ([LocationKey] ASC)
)

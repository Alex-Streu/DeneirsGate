CREATE TABLE [dbo].[Dungeons]
(
    [DungeonKey]    UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Name]          NVARCHAR (150)   NOT NULL,
	[Description]    NVARCHAR(MAX)   NULL
    PRIMARY KEY CLUSTERED ([DungeonKey] ASC)
)

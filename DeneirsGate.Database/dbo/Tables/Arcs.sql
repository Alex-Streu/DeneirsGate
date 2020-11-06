CREATE TABLE [dbo].[Arcs]
(
    [ArcKey]        UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
	[CampaignKey]   UNIQUEIDENTIFIER NOT NULL,
    [Name]          NVARCHAR (150)   NOT NULL,
	[Description]   NVARCHAR (MAX)   NULL,
    [Map]           NVARCHAR (150)   NULL,
    [IsActive] BIT NOT NULL DEFAULT 0,
    PRIMARY KEY CLUSTERED ([ArcKey] ASC)
)

﻿CREATE TABLE [dbo].[DungeonLogs]
(
	[CampaignKey] UNIQUEIDENTIFIER NOT NULL, 
	[LogKey] UNIQUEIDENTIFIER NOT NULL, 
    [DungeonKey] UNIQUEIDENTIFIER NOT NULL
	CONSTRAINT [PK_DungeonLogs] PRIMARY KEY NONCLUSTERED ([CampaignKey], [LogKey], [DungeonKey])
)

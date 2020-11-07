﻿CREATE TABLE [dbo].[QuestEventLogs]
(
	[LogKey] UNIQUEIDENTIFIER NOT NULL, 
    [EventKey] UNIQUEIDENTIFIER NOT NULL
	CONSTRAINT [PK_QuestEventLogs] PRIMARY KEY NONCLUSTERED ([LogKey], [EventKey])
)

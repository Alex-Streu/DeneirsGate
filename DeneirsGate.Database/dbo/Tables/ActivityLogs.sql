﻿CREATE TABLE [dbo].[ActivityLogs]
(
    [LogKey]        UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
	[ArcKey]   UNIQUEIDENTIFIER NOT NULL,
	[DateLogged] DATETIME NOT NULL,
	[Log] NVARCHAR(MAX),
	[Type] INT NOT NULL DEFAULT 0
    PRIMARY KEY CLUSTERED ([LogKey] ASC)
)

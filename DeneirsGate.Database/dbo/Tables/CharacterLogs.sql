﻿CREATE TABLE [dbo].[CharacterLogs]
(
	[LogKey] UNIQUEIDENTIFIER NOT NULL, 
    [CharacterKey] UNIQUEIDENTIFIER NOT NULL
	CONSTRAINT [PK_CharacterLogs] PRIMARY KEY NONCLUSTERED ([LogKey], [CharacterKey])
)

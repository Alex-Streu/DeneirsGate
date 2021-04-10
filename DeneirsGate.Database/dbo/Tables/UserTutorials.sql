﻿CREATE TABLE [dbo].[UserTutorials]
(
	[UserKey] UNIQUEIDENTIFIER NOT NULL,
	[TutorialKey] UNIQUEIDENTIFIER NOT NULL,
	[IsComplete] BIT NOT NULL DEFAULT 0,
	[LastStep] INT NOT NULL
	CONSTRAINT [PK_UserTutorials] PRIMARY KEY NONCLUSTERED ([UserKey], [TutorialKey]) DEFAULT 0 
)
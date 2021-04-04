﻿CREATE TABLE [dbo].[Tutorials]
(
	[TutorialKey] UNIQUEIDENTIFIER NOT NULL,
	[Route] NVARCHAR(100) NOT NULL,
	[Name] NVARCHAR(50) NOT NULL
    PRIMARY KEY CLUSTERED ([TutorialKey] ASC)
)

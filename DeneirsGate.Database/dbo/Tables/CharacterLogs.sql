CREATE TABLE [dbo].[CharacterLogs]
(
	[CampaignKey] UNIQUEIDENTIFIER NOT NULL, 
	[LogKey] UNIQUEIDENTIFIER NOT NULL, 
    [CharacterKey] UNIQUEIDENTIFIER NOT NULL
	CONSTRAINT [PK_CharacterLogs] PRIMARY KEY NONCLUSTERED ([CampaignKey], [LogKey], [CharacterKey])
)

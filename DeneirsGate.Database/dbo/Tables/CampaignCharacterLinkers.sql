CREATE TABLE [dbo].[CampaignCharacterLinkers]
(
	[CampaignKey] UNIQUEIDENTIFIER NOT NULL, 
    [CharacterKey] UNIQUEIDENTIFIER NOT NULL,
	[UserKey] UNIQUEIDENTIFIER NOT NULL
	CONSTRAINT [PK_CampaignCharacterLinkers] PRIMARY KEY NONCLUSTERED ([CampaignKey], [CharacterKey]), 
    [IsUser] BIT NOT NULL DEFAULT 0
)

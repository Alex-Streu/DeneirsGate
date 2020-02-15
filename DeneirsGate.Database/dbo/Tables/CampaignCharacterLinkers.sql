CREATE TABLE [dbo].[CampaignCharacterLinkers]
(
	[CampaignKey] UNIQUEIDENTIFIER NOT NULL, 
    [CharacterKey] UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT [PK_CampaignCharacterLinkers] PRIMARY KEY NONCLUSTERED ([CampaignKey], [CharacterKey])
)

CREATE TABLE [dbo].[CampaignPlayerLinkers]
(
	[CampaignKey] UNIQUEIDENTIFIER NOT NULL, 
    [PlayerKey] UNIQUEIDENTIFIER NOT NULL,
	[CharacterKey] UNIQUEIDENTIFIER NOT NULL, 
    CONSTRAINT [PK_CampaignPlayerLinkers] PRIMARY KEY NONCLUSTERED ([CampaignKey], [PlayerKey], [CharacterKey])
)

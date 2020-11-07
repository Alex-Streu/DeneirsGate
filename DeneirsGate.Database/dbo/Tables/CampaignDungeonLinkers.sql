CREATE TABLE [dbo].[CampaignDungeonLinkers]
(
	[CampaignKey] UNIQUEIDENTIFIER NOT NULL, 
    [DungeonKey] UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT [PK_CampaignDungeonLinkers] PRIMARY KEY NONCLUSTERED ([CampaignKey], [DungeonKey])
)

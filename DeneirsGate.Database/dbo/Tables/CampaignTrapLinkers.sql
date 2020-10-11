CREATE TABLE [dbo].[CampaignTrapLinkers]
(
	[CampaignKey] UNIQUEIDENTIFIER NOT NULL, 
    [TrapKey] UNIQUEIDENTIFIER NOT NULL
	CONSTRAINT [PK_CampaignTrapLinkers] PRIMARY KEY NONCLUSTERED ([CampaignKey], [TrapKey])
)

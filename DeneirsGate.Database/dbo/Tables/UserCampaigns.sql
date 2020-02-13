CREATE TABLE [dbo].[UserCampaigns]
(
	[CampaignKey] UNIQUEIDENTIFIER NOT NULL, 
    [UserKey] UNIQUEIDENTIFIER NOT NULL, 
    [IsOwner] BIT NOT NULL,
	CONSTRAINT [PK_UserCampaigns] PRIMARY KEY NONCLUSTERED ([CampaignKey], [UserKey], [IsOwner]) 
)

CREATE TABLE [dbo].[Settlements]
(
	[SettlementKey] UNIQUEIDENTIFIER NOT NULL,
    [CampaignKey] UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(50) NOT NULL, 
    [Description] NVARCHAR(MAX) NULL, 
    [Map] NVARCHAR(250) NULL
    PRIMARY KEY CLUSTERED ([SettlementKey] ASC)
)

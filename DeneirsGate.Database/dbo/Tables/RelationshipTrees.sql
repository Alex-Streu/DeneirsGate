CREATE TABLE [dbo].[RelationshipTrees]
(
    [TreeKey]       UNIQUEIDENTIFIER NOT NULL,
	[CampaignKey]   UNIQUEIDENTIFIER NOT NULL,
    [Name]          NVARCHAR (50)    NOT NULL,
    PRIMARY KEY CLUSTERED ([TreeKey] ASC)
)

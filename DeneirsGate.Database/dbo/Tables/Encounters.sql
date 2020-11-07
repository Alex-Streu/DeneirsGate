CREATE TABLE [dbo].[Encounters]
(
	[EncounterKey] UNIQUEIDENTIFIER NOT NULL,
	[Name] NVARCHAR(150) NOT NULL,
	[Description] NVARCHAR(MAX),
	[RewardSummary] NVARCHAR(MAX),
    PRIMARY KEY CLUSTERED ([EncounterKey] ASC)
)

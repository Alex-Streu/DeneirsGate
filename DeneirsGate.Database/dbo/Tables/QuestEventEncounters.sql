CREATE TABLE [dbo].[QuestEventEncounters]
(
	[EventKey] UNIQUEIDENTIFIER NOT NULL, 
    [EncounterKey] UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT [PK_QuestEventEncounters] PRIMARY KEY NONCLUSTERED ([EventKey], [EncounterKey])
)

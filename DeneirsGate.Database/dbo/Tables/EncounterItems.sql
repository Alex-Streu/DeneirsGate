CREATE TABLE [dbo].[EncounterItems]
(
	[EncounterKey] UNIQUEIDENTIFIER NOT NULL,
	[ItemKey] UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT [PK_EncounterItems] PRIMARY KEY NONCLUSTERED ([EncounterKey], [ItemKey]) 
)

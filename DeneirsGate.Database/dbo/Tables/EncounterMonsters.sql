CREATE TABLE [dbo].[EncounterMonsters]
(
	[EncounterKey] UNIQUEIDENTIFIER NOT NULL,
	[MonsterKey] UNIQUEIDENTIFIER NOT NULL,
	[Count] INT NOT NULL DEFAULT 1,
	CONSTRAINT [PK_EncounterMonsters] PRIMARY KEY NONCLUSTERED ([EncounterKey], [MonsterKey]) 
)

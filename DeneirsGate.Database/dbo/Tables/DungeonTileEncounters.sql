CREATE TABLE [dbo].[DungeonTileEncounters]
(
	[TileKey] UNIQUEIDENTIFIER NOT NULL, 
    [EncounterKey] UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT [PK_DungeonTileEncounters] PRIMARY KEY NONCLUSTERED ([TileKey], [EncounterKey])
)

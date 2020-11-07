CREATE TABLE [dbo].[ArcCharacterLinkers]
(
	[ArcKey] UNIQUEIDENTIFIER NOT NULL, 
    [CharacterKey] UNIQUEIDENTIFIER NOT NULL
	CONSTRAINT [PK_ArcCharacterLinkers] PRIMARY KEY NONCLUSTERED ([ArcKey], [CharacterKey]), 
)

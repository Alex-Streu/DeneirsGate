CREATE TABLE [dbo].[RelationshipTreeCharacters]
(
	[TreeKey]       UNIQUEIDENTIFIER NOT NULL,
	[TierKey]       UNIQUEIDENTIFIER NOT NULL,
	[CharacterKey]  UNIQUEIDENTIFIER NOT NULL,
	[IsShallow]     BIT              NOT NULL DEFAULT 0,
	[Name]          NVARCHAR(50)     NULL, 
	[Backstory]     NVARCHAR(MAX)    NULL, 
	CONSTRAINT [PK_RelationshipTreeCharacters] PRIMARY KEY NONCLUSTERED ([TreeKey], [CharacterKey]), 
)
